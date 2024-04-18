using FplBot.Api.Team;
using FplBot.Models;
using System.Text;

namespace FplBot.Services
{
    public class AttachmentService(
        IFplService fplService,
        IPersistenceService persistenceService,
        Logging.ILogger logger) : IAttachmentService
    {
        private readonly IFplService fplService = fplService;
        private readonly IPersistenceService persistenceService = persistenceService;
        private readonly Logging.ILogger logger = logger;

        /// <summary>
        /// Generates the current overall standings for this league
        /// </summary>
        /// <returns>A StringBuilder object with the current standings in plaintext format</returns>
        public async Task<StringBuilder> GenerateStandingsTable()
        {
            var currentWeekStandings = await this.fplService.GetCurrentWeekStandings();
            var weeklyResults = await this.fplService.GetWeeklyResult();

            var fplLeague = await this.fplService.GetLeagueAsync();
            var eventId = await this.persistenceService.GetEventIdAsync();
            var currentEvent = await this.fplService.GetCurrentEventAsync();

            StringBuilder standings = new StringBuilder();

            const int dashPadding = 49;
            int longestTeamName = currentWeekStandings.Max(team => team.Value.Name.Length);

            standings.AppendLine($"Standings for {fplLeague.League.Name} after {currentEvent.Name}:");
            standings.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));
            standings.AppendLine($"Rank Chg. PW   Overall  Team{string.Empty.PadLeft(longestTeamName - 3)}  GW  Total   TT   TmVal");
            standings.AppendLine("".PadLeft(longestTeamName + dashPadding, '-')).AppendLine();

            foreach (var team in weeklyResults.OrderBy(x => x.CurrentWeekPosition))
            {
                // if this is the first gameweek, there was no rank last week so we'll return -1 and print out -- for movement
                string currentRank = team.CurrentWeekPosition.ToString().PadRight(2);
                string previousRank = eventId == 1 ? "--" : team.PreviousWeekPosition.ToString().PadRight(2);
                string overallRank = TextUtilities.FormatRank(team.OverallRank).PadRight(6);
                string teamName = team.Name.PadRight(longestTeamName);
                string points = team.TotalPoints.ToString().PadLeft(4);
                string totalTransfers = team.TotalTransfers.ToString().PadLeft(3);
                string gameweekPoints = team.GameWeekPoints.ToString().PadLeft(3);
                string teamValue = team.TeamValue.ToString("N1").PadLeft(5);

                string movement;

                if (currentRank == previousRank || team.PreviousWeekPosition < 0)
                {
                    movement = "--";
                }
                else if (team.CurrentWeekPosition > team.PreviousWeekPosition)
                {
                    movement = "dn";
                }
                else
                {
                    movement = "up";
                }

                standings.AppendLine($"{currentRank}   {movement}   {previousRank}   {overallRank}   {teamName}  {gameweekPoints}   {points}  {totalTransfers}   {teamValue}");
            }

            standings.AppendLine();
            standings.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));
            standings.AppendLine($"Rank:    Current rank in league {fplLeague.League.Name}");
            standings.AppendLine("Chg.:    Movement in league compared to previous week");
            standings.AppendLine("PW:      Previous week rank in league");
            standings.AppendLine("Overall: Rank amongst all players in FPL");
            standings.AppendLine("GW:      Game week points");
            standings.AppendLine("Total:   Point sum of all game weeks");
            standings.AppendLine("TT:      Total transfers");
            standings.AppendLine("TmVal:   Team value (including bank)");

            this.logger.Log(standings.ToString());

            return standings;
        }

        /// <summary>
        /// Generates the total weekly wins everyone has had
        /// </summary>
        /// <returns></returns>
        public async Task<StringBuilder> GenerateTotalWeeklyWins()
        {
            this.logger.Log("Calculating total weekly wins...");
            var currentWeekStandings = await this.fplService.GetCurrentWeekStandings();
            var fplTeams = await this.fplService.GetTeamsAsync();

            var eventId = await this.persistenceService.GetEventIdAsync();

            StringBuilder result = new StringBuilder();

            const int dashPadding = 53;
            int longestTeamName = currentWeekStandings.Max(team => team.Value.Name.Length);
            int skippedCoronaWeeks = 0;

            result.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));
            result.AppendLine($"Team{string.Empty.PadLeft(longestTeamName - 3)}  EffW  Excl  Shrd   wDDR   Gameweek win(s)");
            result.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));

            for (int index = 1; index <= eventId; index++)
            {
                var weeklyResult = fplTeams
                    .Where(t => t.Value.Current.Any(te => te.Event == index))
                    .Select(
                        team =>
                        {
                            ApiTeamEvents history = team.Value.Current.Find(e => e.Event == index);

                            var teamResult = new WeeklyResult()
                            {
                                Id = team.Key,
                                Name = fplTeams[team.Key].Name,
                                HitsTakenCost = history.EventTransfersCost.Value,
                                ScoreBeforeHits = history.Points.Value,
                            };

                            return teamResult;
                        })
                    .OrderByDescending(x => x.Points);

                long topScore = weeklyResult.Max(t => t.Points);
                long topBeforeHits = weeklyResult.Max(t => t.ScoreBeforeHits);

                if (topScore == 0)
                {
                    // Top score of 0 probably means everyone is in quarantine
                    skippedCoronaWeeks++;
                    continue;
                }

                IEnumerable<WeeklyResult> winners = weeklyResult
                    .Where(x => x.Points == topScore)
                    .ToList();

                IEnumerable<WeeklyResult> foiledTeams = weeklyResult
                    .Where(X => topBeforeHits >= topScore)                 // Bail out if there weren't any teams with score high enough to trigger DDR
                    .Where(x => x.HitsTakenCost > 0)                       // Filter out any teams that didn't take any hits
                    .Where(x => x.ScoreBeforeHits == topBeforeHits)        // Find the player(s) that had the top score before hits were taken
                    .Where(x => x.Points < topScore)                       // Make sure the score was lower than topScore, otherwise they won the week despite DDR
                    .ToList();

                foreach (var winner in winners)
                {
                    fplTeams[winner.Id].TotalWeeklyWins += 1f / (float)winners.Count();
                    fplTeams[winner.Id].WinWeeks.Add(index > 38 ? $"{(index - skippedCoronaWeeks).ToString()}+" : index.ToString());

                    if (winners.Count() > 1)
                    {
                        fplTeams[winner.Id].TotalWeeklySharedWins++;
                    }
                    else
                    {
                        fplTeams[winner.Id].TotalWeeklySingleWins++;
                    }
                }

                foreach (var foiledTeam in foiledTeams)
                {
                    fplTeams[foiledTeam.Id].FoiledByDanDaviesRule++;
                }
            }

            foreach (var team in fplTeams.OrderByDescending(x => x.Value.TotalWeeklySingleWins).ThenByDescending(x => x.Value.TotalWeeklySharedWins))
            {
                string teamName = team.Value.Name.PadRight(longestTeamName);
                string wins = team.Value.TotalWeeklyWins.ToString("0.##").PadLeft(4);
                string singleWins = team.Value.TotalWeeklySingleWins.ToString().PadLeft(2);
                string sharedWins = team.Value.TotalWeeklySharedWins.ToString().PadLeft(2);
                string foiledByDanDaviesRule = team.Value.FoiledByDanDaviesRule.ToString().PadLeft(2);
                string weeks = team.Value.WinWeeks.Count() > 0 ? team.Value.WinWeeks.Aggregate((x, y) => $"{x.ToString()}, {y}") : string.Empty;

                result.AppendLine($"{teamName}   {wins}    {singleWins}    {sharedWins}     {foiledByDanDaviesRule}   {weeks}");
            }

            result.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));

            result.AppendLine("EffW: Effective wins");
            result.AppendLine("Excl: # of time team won a gameweek exclusively");
            result.AppendLine("Shrd: # of shared wins of a gameweek ");
            result.AppendLine("wDDR: # of times a team would have won if not for the Dan Davies Rule");

            this.logger.Log(result.ToString());

            return result;
        }
    }
}
