using Flurl.Http;
using FplBot.Api.Summary;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FplBot
{
    /// <summary>
    /// A class that's responsible for connecting to and processing the FPL API
    /// </summary>
    internal class Fpl
    {
        private readonly string FplRootUri = "https://fantasy.premierleague.com/drf/bootstrap-static";
        private readonly string LeagueStandingsUri = "https://fantasy.premierleague.com/drf/leagues-classic-standings/{0}";
        private readonly string TeamUri = "https://fantasy.premierleague.com/drf/entry/{0}/history";
        private readonly string PicksUri = "https://fantasy.premierleague.com/drf/entry/{0}/event/{1}/picks";
        private readonly string PlayersUri = "https://fantasy.premierleague.com/drf/elements";
        private readonly string PlayerSummaryUrI = "https://fantasy.premierleague.com/drf/element-summary/{0}";
        private readonly Dictionary<long, Api.Team.FplTeam> fplTeam;
        private readonly Dictionary<long, Api.Picks.FplPicks> fplPicks;
        private readonly Dictionary<long, FplPlayerSummary> fplPlayerSummaries;
        private readonly Dictionary<long, Api.Player.FplPlayer> fplCaptains;
        private readonly int currentEventId;
        IOrderedEnumerable<TeamWeeklyResult> weeklyResults;
        IOrderedEnumerable<KeyValuePair<long, Api.Team.FplTeam>> lastWeekStandings;
        IOrderedEnumerable<KeyValuePair<long, Api.Team.FplTeam>> currentWeekStandings;
        private Api.Root.FplRoot fplRoot;
        private Api.Season.FplSeason fplSeason;
        private Api.Root.Event currentEvent;
        private Dictionary<long, Api.Player.FplPlayer> fplPlayers;

        /// <summary>
        /// Initializes a new instance of the Fpl class
        /// </summary>
        /// <param name="gameweekToProcess"></param>
        internal Fpl(int gameweekToProcess)
        {
            this.currentEventId = gameweekToProcess;
            this.fplPlayers = new Dictionary<long, Api.Player.FplPlayer>();
            this.fplTeam = new Dictionary<long, Api.Team.FplTeam>();
            this.fplPicks = new Dictionary<long, Api.Picks.FplPicks>();
            this.fplPlayerSummaries = new Dictionary<long, FplPlayerSummary>();
            this.fplCaptains = new Dictionary<long, Api.Player.FplPlayer>();
        }

        /// <summary>
        /// Processes the information from the FPL API
        /// </summary>
        /// <returns></returns>
        internal async Task<StringBuilder> Process()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"Beep boop! I am a robot. This is your weekly FPL update.").AppendLine();

            this.fplRoot = await this.FplRootUri.GetJsonAsync<Api.Root.FplRoot>().ConfigureAwait(false);
            this.currentEvent = fplRoot.Events.Find(x => x.Id == this.currentEventId);

            if (this.currentEvent != null && this.currentEvent.Finished.Value && this.currentEvent.DataChecked.Value)
            {
                await this.LoadApiDataAsync();

                this.CalculateWeeklyRank();

                string topResults = this.CalculateTop();
                string bottomResults = this.CalculateBottom();
                string averageResults = this.CalculateAverages();
                string captains = await this.CalculateCaptains();
                string movement = this.currentEventId > 1 ? this.CalculateTopAndBottomStandings() : string.Empty;
                string pointsBenched = this.CalculatePointsOnBench();
                string hitsTaken = this.PrintHitsTaken();
                string chipsUsed = this.CalculateChipsUsed();

                result.AppendLine(topResults);
                result.AppendLine(bottomResults);
                result.AppendLine(captains);

                if (!string.IsNullOrEmpty(movement))
                {
                    result.AppendLine(movement);
                }

                result.AppendLine("Notable news:").AppendLine();

                result.Append("- ").AppendLine(averageResults);
                result.Append("- ").AppendLine(pointsBenched);
                result.Append("- ").AppendLine(hitsTaken);
                result.Append("- ").AppendLine(chipsUsed);

                result.AppendLine().AppendLine("Your friendly FPL bot will return next gameweek with another update.");

                return result;
            }

            return null;
        }

        /// <summary>
        /// Generates the current overall standings for this league
        /// </summary>
        /// <returns>A StringBuilder object with the current standings in plaintext format</returns>
        internal StringBuilder GenerateStandingsTable()
        {
            if (this.currentWeekStandings == null) return null;

            StringBuilder standings = new StringBuilder();

            // We're trying to do some padding here, but it still looks wonky in a lot of email clients
            // Might move this to a txt attachment instead
            int longestTeamName = this.currentWeekStandings.Max(x => x.Value.Entry.Name.Length);
            int index = 1;

            standings.AppendLine("Current standings:").AppendLine(); ;

            foreach (var team in this.currentWeekStandings)
            {
                standings.AppendLine($"{index.ToString().PadLeft(2)} \t{team.Value.Entry.Name.PadRight(longestTeamName)}\t{team.Value.History.Find(x => x.Event == this.currentEventId).TotalPoints.ToString().PadLeft(4)}");
                index++;
            }

            return standings;
        }

        /// <summary>
        /// Loads all the data from the FPL API to keep in memory
        /// </summary>
        /// <returns></returns>
        private async Task LoadApiDataAsync()
        {
            var playerTask = this.PlayersUri.GetJsonAsync<List<Api.Player.FplPlayer>>();
            var leagueTask = string.Format(this.LeagueStandingsUri, ConfigurationManager.AppSettings["leagueId"]).GetJsonAsync<Api.Season.FplSeason>();

            await Task.WhenAll(playerTask, leagueTask).ConfigureAwait(false);

            this.fplPlayers = playerTask.Result.ToDictionary(y => y.Id.Value);
            this.fplSeason = leagueTask.Result;

            foreach (Api.Season.Result standing in this.fplSeason.Standings.Results.AsParallel())
            {
                var teamTask = string.Format(this.TeamUri, standing.Entry).GetJsonAsync<Api.Team.FplTeam>();
                var picksTask = string.Format(this.PicksUri, standing.Entry, this.currentEventId).GetJsonAsync<Api.Picks.FplPicks>();

                await Task.WhenAll(teamTask, picksTask).ConfigureAwait(false);

                Api.Team.FplTeam team = teamTask.Result;
                Api.Picks.FplPicks picks = picksTask.Result;

                this.fplTeam.Add(standing.Entry, team);
                this.fplPicks.Add(standing.Entry, picks);
            }
        }

        /// <summary>
        /// Calculates the weekly rank for all the teams
        /// </summary>
        private void CalculateWeeklyRank()
        {
            this.weeklyResults = this.fplTeam
                .Select(x =>
                {
                    Api.Team.History history = x.Value.History.Find(y => y.Event == this.currentEventId);
                    string chip = x.Value.Chips.Find(c => c.Event == this.currentEventId)?.Name ?? string.Empty;

                    var result = new TeamWeeklyResult()
                    {
                        Name = x.Value.Entry.Name,
                        HitsTakenCost = history.EventTransfersCost.Value,
                        ScoreBeforeHits = history.Points.Value,
                        ChipUsed = chip
                    };

                    return result;
                })
                .OrderByDescending(x => x.Points);

            this.lastWeekStandings = this.fplTeam.OrderByDescending(x => x.Value.History.Find(y => y.Event == this.currentEventId - 1).TotalPoints);
            this.currentWeekStandings = this.fplTeam.OrderByDescending(x => x.Value.History.Find(y => y.Event == this.currentEventId).TotalPoints);
        }

        /// <summary>
        /// Calculates the top 5(ish) placements for this week
        /// </summary>
        /// <returns></returns>
        private string CalculateTop()
        {
            // Currently implements the Dan Davies rule
            // To find the winner of the gameweek we take every score and subtract the transfer cost for that week
            // This helps counteract managers taking multiple hits every week in an attempt to win the weekly prize
            // Might put this in the app.config as a configurable setting at some point

            StringBuilder result = new StringBuilder();

            long topScore = this.weeklyResults.Max(t => t.Points);                  // this.rankedTeams.First().Value.History.Find(y => y.Event == this.currentEvent).Points;
            long topGrossScore = this.weeklyResults.Max(t => t.ScoreBeforeHits);    // this.rankedTeams.First().Value.History.Find(y => y.Event == this.currentEvent).Points;
            long top3Score = this.weeklyResults.Take(3).Last().Points;              // this.rankedTeams.Take(3).Last().Value.History.Find(y => y.Event == this.currentEvent).Points;

            IEnumerable<string> winnerNames = this.weeklyResults                    // This weeks winner(s)
                .Where(x => x.Points == topScore)                                   // Find all teams with the top score
                .Select(x => x.Name);                                               // Get their name(s)

            IEnumerable<string> topNamesBeforeTransferCost = this.weeklyResults     // Used for Dan Davies rule
                .OrderByDescending(x => x.ScoreBeforeHits)
                .Where(x => x.ScoreBeforeHits == topGrossScore)
                .Select(x => x.Name);

            IEnumerable<string> topNames =  this.weeklyResults                      // Top 3ish teams for this week
                .Where(x => x.Points >= top3Score)                                  // Find anyone that had top 3 score or better
                .Where(x => x.Points < topScore)                                    // Exlude the first one
                .Select(x => $"{x.Name} ({x.Points} pts)");                         // Prepare text

            bool daviesRuleInEffect = !(topNamesBeforeTransferCost.All(x => winnerNames.Contains(x)) && winnerNames.All(x => topNamesBeforeTransferCost.Contains(x)));

            if (daviesRuleInEffect)
            {
                TextUtilities.StringJoinWithCommasAndAnd(result, topNamesBeforeTransferCost);
                if (topNamesBeforeTransferCost.Count() > 2)
                {
                    result.Append(" were all");
                }
                else if (topNamesBeforeTransferCost.Count() > 1)
                {
                    result.Append(" were both");
                }
                else
                {
                    result.Append(" was");
                }

                result.Append(" in line to win the week until the ever so controversial Dan Davies rule was applied. But once the dust settled the "); // come up with something better here
            }
            else
            {
                result.Append("The ");
            }

            result.Append($"winner{TextUtilities.Pluralize(winnerNames.Count())} for gameweek #{this.currentEventId} {TextUtilities.WasWere(winnerNames.Count())} ");
            TextUtilities.StringJoinWithCommasAndAnd(result, winnerNames);
            result.Append($" with {topScore} points");

            long winnerHitCost = this.weeklyResults.First().HitsTakenCost;
            if (winnerHitCost > 0 && winnerNames.Count() == 1)
            {
                result.Append($" despite taking a -{winnerHitCost} point hit! ");
            }
            else
            {
                result.Append("! ");
            }

            result.Append($"Rounding up the top {topNames.Count() + winnerNames.Count()} for the week {TextUtilities.WasWere(winnerNames.Count())} ");
            TextUtilities.StringJoinWithCommasAndAnd(result, topNames);
            result.AppendLine(". ");

            return result.ToString();
        }

        /// <summary>
        /// Calculate the bottom 5 results for this week
        /// </summary>
        /// <returns></returns>
        private string CalculateBottom()
        {
            StringBuilder result = new StringBuilder();

            IEnumerable<string> worstTeams = this.weeklyResults // The worst performing teams of the week
                .Skip(this.weeklyResults.Count() - 4)           // Get the last 4 teams
                .Select(t => $"{t.Name} ({t.Points} pts)");     // Prepare text

            result.Append($"The worst ranking teams this week were ");
            TextUtilities.StringJoinWithCommasAndAnd(result, worstTeams);
            result.AppendLine(". You should probably be embarrassed. ");

            return result.ToString();
        }

        /// <summary>
        /// Calculates the average score, and how many players beat it
        /// </summary>
        /// <returns></returns>
        private string CalculateAverages()
        {
            StringBuilder result = new StringBuilder();

            long overallAverage = this.currentEvent.AverageEntryScore.Value;
            long teamsAtAverageOrBetter = this.weeklyResults.Where(x=> x.Points > overallAverage).Count();
            
            result.Append($"{teamsAtAverageOrBetter} teams managed to reach or beat the overall average of {overallAverage} points for the week. ");

            return result.ToString();
        }
        
        /// <summary>
        /// Calculates the captaincy choices for the week.
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateCaptains()
        {
            StringBuilder result = new StringBuilder();

            var groupedCaptains = this.fplPicks
                .Select(p => p.Value.Picks.Find(y => y.IsCaptain.Value).Element)
                .GroupBy(id => id.Value)
                .OrderByDescending(g => g.Count());

            var groupedViceCaptains = this.fplPicks
                .Select(p => p.Value.Picks.Find(y => y.IsViceCaptain.Value).Element)
                .GroupBy(id => id.Value)
                .OrderByDescending(g => g.Count());

            foreach (var p in groupedCaptains.AsParallel())
            {
                if (!this.fplPlayerSummaries.ContainsKey(p.Key))
                {
                    FplPlayerSummary summary = await string.Format(this.PlayerSummaryUrI, p.Key).GetJsonAsync<FplPlayerSummary>().ConfigureAwait(false);
                    this.fplPlayerSummaries.Add(p.Key, summary);
                }
            }

            foreach (var p in groupedViceCaptains.AsParallel())
            {                
                if (!this.fplPlayerSummaries.ContainsKey(p.Key))
                {
                    FplPlayerSummary summary = await string.Format(this.PlayerSummaryUrI, p.Key).GetJsonAsync<FplPlayerSummary>().ConfigureAwait(false);
                    this.fplPlayerSummaries.Add(p.Key, summary);
                }
            }

            List<CaptainChoice> captains = new List<CaptainChoice>();

            foreach (var pick in this.fplPicks)
            {
                long teamId = pick.Key;
                var cptPick = pick.Value.Picks.Find(p => p.IsCaptain.Value);
                var vicePick = pick.Value.Picks.Find(p => p.IsViceCaptain.Value);

                var cptPlayer = this.fplPlayers[cptPick.Element.Value];
                var vicePlayer = this.fplPlayers[vicePick.Element.Value];

                CaptainChoice cc = new CaptainChoice()
                {
                    CaptainMultiplier = cptPick.Multiplier.Value,
                    CaptainId = cptPlayer.Id.Value,
                    CaptainEventPoints = this.fplPlayerSummaries[cptPlayer.Id.Value].History.FindAll(y => y.Round == this.currentEventId).Sum(x => x.TotalPoints.Value),
                    TeamEntryId = teamId,
                    CaptainPlayed = this.fplPlayerSummaries[cptPlayer.Id.Value].History.FindAll(y => y.Round == this.currentEventId).Sum(x => x.Minutes.Value) > 0,
                    ViceMultiplier = vicePick.Multiplier.Value,
                    ViceEventPoints = this.fplPlayerSummaries[vicePlayer.Id.Value].History.FindAll(y => y.Round == this.currentEventId).Sum(x => x.TotalPoints.Value),
                    ViceId = vicePlayer.Id.Value,
                    VicePlayed = this.fplPlayerSummaries[vicePlayer.Id.Value].History.FindAll(y => y.Round == this.currentEventId).Sum(x => x.Minutes.Value) > 0
                };

                captains.Add(cc);
            }

            var groupedScores = captains
                .Where(c => !c.BothBlanked)
                .GroupBy(c => c.EventScoreMultiplied)
                .OrderByDescending(g => g.Key);

            int noBest = groupedScores.First().Count();
            int noWorst = groupedScores.Last().Count();
            long bestScore = groupedScores.First().Key;
            long worstScore = groupedScores.Last().Key;

            List<long> bestPlayerIds = new List<long>();
            List<long> worstPlayerIds = new List<long>();
            List<long> bestTeamIds = new List<long>();
            List<long> worstTeamIds = new List<long>();

            foreach (var p in groupedScores.First())
            {
                if (!bestPlayerIds.Contains(p.ActivePlayerId))
                {
                    bestPlayerIds.Add(p.ActivePlayerId);
                }

                if (!bestTeamIds.Contains(p.TeamEntryId))
                {
                    bestTeamIds.Add(p.TeamEntryId);
                }
            }

            foreach (var p in groupedScores.Last())
            {
                if (!worstPlayerIds.Contains(p.ActivePlayerId))
                {
                    worstPlayerIds.Add(p.ActivePlayerId);
                }

                if (!worstTeamIds.Contains(p.TeamEntryId))
                {
                    worstTeamIds.Add(p.TeamEntryId);
                }
            }

            result.Append($"When it came to captaincy choice{TextUtilities.Pluralize(noBest)} ");
            TextUtilities.StringJoinWithCommasAndAnd(result, bestTeamIds.Select(i => $"{this.fplTeam[i].Entry.Name}").ToList());
            result.Append($" did the best this week with {bestScore} point{TextUtilities.Pluralize(noBest)} from ");
            TextUtilities.StringJoinWithCommasAndAnd(result, bestPlayerIds.Select(i => $"{this.fplPlayers[i].FirstName} {this.fplPlayers[i].SecondName}").ToList());
            result.Append(". ");

            result.Append("On the other end of the spectrum were ");
            TextUtilities.StringJoinWithCommasAndAnd(result, worstTeamIds.Select(i => $"{this.fplTeam[i].Entry.Name}").ToList());
            result.Append($" who had picked ");
            TextUtilities.StringJoinWithCommasAndAnd(result, worstPlayerIds.Select(i => $"{this.fplPlayers[i].FirstName} {this.fplPlayers[i].SecondName}").ToList());
            result.AppendLine($" for a total of {worstScore} point{TextUtilities.Pluralize((int)worstScore)}. You receive the armband of shame for this week. ");

            return result.ToString();
        }

        /// <summary>
        /// Figures out which teams moved overall places compared to last week
        /// </summary>
        /// <returns></returns>
        private string CalculateTopAndBottomStandings()
        {
            StringBuilder result = new StringBuilder();

            string previousFirstPlace = this.lastWeekStandings.First().Value.Entry.Name;
            string currentFirstPlace = this.currentWeekStandings.First().Value.Entry.Name;
            long firstPlacePoints = this.currentWeekStandings.First().Value.History.Find(x => x.Event == this.currentEventId).TotalPoints.Value;

            if (previousFirstPlace == currentFirstPlace)
            {
                result.Append($"{previousFirstPlace} stay at the top of the table with {firstPlacePoints} points.");
            }
            else
            {
                result.Append($"{currentFirstPlace} with {firstPlacePoints} total points is the new league leader, supplanting last weeks leader {previousFirstPlace}.");
            }

            result.Append(" At the other end ");

            string previousLastPlace = this.lastWeekStandings.Last().Value.Entry.Name;
            string currentLastPlace = this.currentWeekStandings.Last().Value.Entry.Name;
            long lastPlacePoints = this.currentWeekStandings.Last().Value.History.Find(x => x.Event == this.currentEventId).TotalPoints.Value;
            
            if (previousLastPlace == currentLastPlace)
            {
                result.Append($"{previousLastPlace} continues to languish in last place with a {TextUtilities.GetPoorAdjective()} {lastPlacePoints} points.");
            }
            else
            {
                result.Append($"{currentLastPlace} is the new bottom feeder with a {TextUtilities.GetPoorAdjective()} {lastPlacePoints} points total.");
            }

            result.AppendLine();

            return result.ToString();
        }

        /// <summary>
        /// Calculates the highest left on bench score
        /// </summary>
        /// <returns></returns>
        private string CalculatePointsOnBench()
        {
            StringBuilder result = new StringBuilder();
            Dictionary<long, long> teams = new Dictionary<long, long>();
            long highestPoints = 0;

            foreach (var team in this.fplTeam.Values)
            {
                var history = team.History.Find(t => t.Event == this.currentEventId);
                teams.Add(team.Entry.Id.Value, history.PointsOnBench.Value);

                if (history.PointsOnBench.Value > highestPoints)
                {
                    highestPoints = history.PointsOnBench.Value;
                }
            }

            var teansWithHighest = teams
                .GroupBy(y => y.Value)
                .OrderByDescending(y => y.Key)
                .First()
                .Select(a => this.fplTeam[a.Key].Entry.Name)
                .ToList();

            TextUtilities.StringJoinWithCommasAndAnd(result, teansWithHighest);
            result.Append($" left ");

            if (highestPoints > 20)
            {
                result.Append($"an {TextUtilities.GetGoodAdjective()} ");
            }

            result.Append($"{highestPoints} points on the bench which was the highest in the league.");

            return result.ToString();
        }

        /// <summary>
        /// Calculates hits taken
        /// </summary>
        /// <returns></returns>
        private string PrintHitsTaken()
        {
            StringBuilder result = new StringBuilder();

            var teamBlurbs = this.weeklyResults
                .Where(x => x.HitsTakenCost > 0)
                .Select(y => $"{y.Name} (-{y.HitsTakenCost} pts)");

            if (teamBlurbs.Count() > 0)
            {
                if (teamBlurbs.Count() == 1)
                {
                    result.Append("The only team to take a hit this week was ");
                }

                TextUtilities.StringJoinWithCommasAndAnd(result, teamBlurbs);

                if (teamBlurbs.Count() == 2)
                {
                    result.Append(" both took transfer hits");
                }
                else if (teamBlurbs.Count() > 2)
                {
                    result.Append(" all took hits this week");
                }

                result.Append(". ");

                return result.ToString();
            }
            else
            {
                result.Append("No hits were taken this week.");
            }


            return result.ToString();
        }
        
        /// <summary>
        /// Calculate which chips were used during this gameweek
        /// </summary>
        /// <returns></returns>
        private string CalculateChipsUsed()
        {
            StringBuilder result = new StringBuilder();

            var teamBlurbs = this.weeklyResults
                .Where(t => !string.IsNullOrEmpty(t.ChipUsed))
                .Select(t => $"{t.Name} ({t.ChipUsed})");

            if (teamBlurbs.Count() < 1)
            {
                result.Append("No chips were played this week.");
            }
            else if (teamBlurbs.Count() > 5)
            {
                result.Append("A flurry of activity this week as ");
                TextUtilities.StringJoinWithCommasAndAnd(result, teamBlurbs);
                result.Append(" all played chips.");
            }
            else if (teamBlurbs.Count() == 1)
            {
                TextUtilities.StringJoinWithCommasAndAnd(result, teamBlurbs);
                result.Append(" was the only team to use a chip this week.");
            }
            else
            {
                TextUtilities.StringJoinWithCommasAndAnd(result, teamBlurbs);
                result.Append(" decided to spend one of their chips this week.");
            }           

            return result.ToString();
        }
    }
}
