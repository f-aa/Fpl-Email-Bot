using FplBot.Api.Player;
using FplBot.Api.Root;
using FplBot.Models;
using System.Text;

namespace FplBot.Services
{
    public class OutputService(
        IFplService fplService,
        IPersistenceService persistenceService,
        Logging.ILogger logger) : IOutputService
    {
        private readonly IFplService fplService = fplService;
        private readonly IPersistenceService persistenceService = persistenceService;
        private readonly Logging.ILogger logger = logger;
        private readonly StringBuilder output = new();

        private int eventId = -1;
        public Event? currentEvent;
        private Dictionary<long, ApiPlayerDetail>? playerDetails;

        /// <summary>
        /// Processes the information from the FPL API
        /// </summary>
        public async Task<string> BuildOutput()
        {
            this.currentEvent = await this.fplService.GetCurrentEventAsync();
            this.eventId = await this.persistenceService.GetEventIdAsync();
            this.playerDetails = this.fplService.GetSoccerPlayers();

            this.output.AppendLine($"Beep boop! I am a robot. This is your weekly FPL update.").AppendLine();

            this.output.AppendLine(await this.CalculateTop());
            this.output.AppendLine(await this.CalculateBottom());
            this.output.AppendLine(await this.CalculateCaptains());
            this.output
                .Append(await this.CalculateAutomaticSubs())
                .AppendLine(await this.CalculatePointsOnBench());

            if (this.eventId > 1)
            {
                this.output.AppendLine(await this.CalculateTopAndBottomStandings());
                this.output.AppendLine(await this.CalculateMoversAndShakers());
            }

            this.output.AppendLine();
            this.output.AppendLine("Notable news:").AppendLine();

            this.output.Append("- ").AppendLine(await this.CalculateAverages());
            this.output.Append("- ").AppendLine(await this.PrintHitsTaken());
            this.output.Append("- ").AppendLine(await this.CalculateChipsUsed());

            this.output.AppendLine().AppendLine("Your friendly FPL bot will return next gameweek with another update.");

            this.logger.Log(this.output.ToString());

            return this.output.ToString();
        }

        /// <summary>
        /// Calculates the top 5(ish) placements for this week
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateTop()
        {
            this.logger.Log("Calculating top positions...");

            StringBuilder result = new();

            var weeklyResults = await this.fplService.GetWeeklyResult();

            long topScore = weeklyResults.Max(t => t.Points);
            long topGrossScore = weeklyResults.Max(t => t.ScoreBeforeHits);
            long top3Score = weeklyResults.Take(3).Last().Points;

            IEnumerable<string> winnerNames = weeklyResults                    // This weeks winner(s)
                .Where(x => x.Points == topScore)                                   // Find all teams with the top score
                .Select(x => x.Name);                                               // Get their name(s)

            IEnumerable<string> topNamesBeforeTransferCost = weeklyResults     // Used for Dan Davies rule
                .OrderByDescending(x => x.ScoreBeforeHits)
                .Where(x => x.ScoreBeforeHits == topGrossScore)
                .Select(x => x.Name);

            IEnumerable<string> topNames = weeklyResults                       // Top 3ish teams for this week
                .Where(x => x.Points >= top3Score)                                  // Find anyone that had top 3 score or better
                .Where(x => x.Points < topScore)                                    // Exlude the first one
                .Select(x => $"{x.Name} ({x.Points} pts)");                         // Prepare text

            bool daviesRuleInEffect = !(topNamesBeforeTransferCost.All(x => winnerNames.Contains(x)) && winnerNames.All(x => topNamesBeforeTransferCost.Contains(x)));
            long winnerHitCost = weeklyResults.First().HitsTakenCost;

            if (daviesRuleInEffect)
            {
                result.Append($"{TextUtilities.NaturalParse(topNamesBeforeTransferCost)} {TextUtilities.WasWere(topNamesBeforeTransferCost.Count(), true)} in line to win the week until the ever so controversial Dan Davies rule was applied. But once the dust settled the ");
            }
            else
            {
                result.Append("The ");
            }

            result.Append($"winner{TextUtilities.Pluralize(winnerNames.Count())} for {this.currentEvent.Name.ToLowerInvariant()} {TextUtilities.WasWere(winnerNames.Count())} {TextUtilities.NaturalParse(winnerNames)} with {topScore} points");

            if (winnerHitCost > 0 && winnerNames.Count() == 1)
            {
                result.Append($" despite taking a -{winnerHitCost} point hit! ");
            }
            else
            {
                result.Append("! ");
            }

            result.AppendLine($"Rounding up the top {topNames.Count() + winnerNames.Count()} for the week {TextUtilities.WasWere(winnerNames.Count())} {TextUtilities.NaturalParse(topNames)}.");

            return result.ToString();
        }

        /// <summary>
        /// Calculate the bottom 5 results for this week
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateBottom()
        {
            this.logger.Log("Calculating bottom positions...");

            StringBuilder result = new StringBuilder();

            var weeklyResults = await this.fplService.GetWeeklyResult();

            IEnumerable<string> worstTeams = weeklyResults // The worst performing teams of the week
                .Skip(weeklyResults.Count() - 4)           // Get the last 4 teams
                .Select(t => $"{t.Name} ({t.Points} pts)");     // Prepare text

            result.AppendLine($"The worst ranking teams this week were {TextUtilities.NaturalParse(worstTeams)}. You should probably be embarrassed. ");

            return result.ToString();
        }

        /// <summary>
        /// Calculates the average score, and how many players beat it
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateAverages()
        {
            this.logger.Log("Calculating averages...");

            StringBuilder result = new StringBuilder();

            var weeklyResults = await this.fplService.GetWeeklyResult();

            long overallAverage = this.currentEvent.AverageEntryScore.Value;
            long teamsAtAverageOrBetter = weeklyResults.Where(x => x.Points > overallAverage).Count();

            result.Append($"{teamsAtAverageOrBetter} teams managed to reach or beat the overall average of {overallAverage} points for the week. ");

            return result.ToString();
        }

        /// <summary>
        /// Calculates the captaincy choices for the week.
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateCaptains()
        {
            this.logger.Log("Calculating captains...");

            StringBuilder result = new();

            var fplTeams = await this.fplService.GetTeamsAsync();
            var fplPlayers = await this.fplService.GetPlayersAsync();

            List<CaptainChoice> captains = [];

            foreach (var pick in fplTeams)
            {
                long teamId = pick.Key;
                var cptPick = pick.Value.Squad.Picks.Find(p => p.IsCaptain.Value);
                var vicePick = pick.Value.Squad.Picks.Find(p => p.IsViceCaptain.Value);

                var cptPlayer = this.playerDetails[cptPick.Element.Value];
                var vicePlayer = this.playerDetails[vicePick.Element.Value];

                CaptainChoice cc = new CaptainChoice()
                {
                    CaptainMultiplier = cptPick.Multiplier.Value,
                    CaptainId = cptPlayer.Id.Value,
                    CaptainEventPoints = fplPlayers[cptPlayer.Id.Value].History.FindAll(y => y.Round == this.eventId).Sum(x => x.TotalPoints.Value),
                    TeamEntryId = teamId,
                    CaptainPlayed = fplPlayers[cptPlayer.Id.Value].History.FindAll(y => y.Round == this.eventId).Sum(x => x.Minutes.Value) > 0,
                    ViceMultiplier = vicePick.Multiplier.Value,
                    ViceEventPoints = fplPlayers[vicePlayer.Id.Value].History.FindAll(y => y.Round == this.eventId).Sum(x => x.TotalPoints.Value),
                    ViceId = vicePlayer.Id.Value,
                    VicePlayed = fplPlayers[vicePlayer.Id.Value].History.FindAll(y => y.Round == this.eventId).Sum(x => x.Minutes.Value) > 0
                };

                captains.Add(cc);
            }

            var groupedScores = captains
                .Where(c => !c.BothBlanked)
                .GroupBy(c => c.EventScoreMultiplied)
                .OrderByDescending(g => g.Key);

            int noBest = groupedScores.FirstOrDefault()?.Count() ?? 0;
            int noWorst = groupedScores.LastOrDefault()?.Count() ?? 0;
            long bestScore = groupedScores.FirstOrDefault()?.Key ?? 0;
            long worstScore = groupedScores.LastOrDefault()?.Key ?? 0;

            List<long> bestPlayerIds = [];
            List<long> worstPlayerIds = [];
            List<long> bestTeamIds = [];
            List<long> worstTeamIds = [];

            if (groupedScores.Count() > 0)
            {
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

                result.Append($"When it came to captaincy choice{TextUtilities.Pluralize(noBest)} {TextUtilities.NaturalParse(bestTeamIds.Select(i => $"{fplTeams[i].Name}").ToList())} did the best this week with {bestScore} point{TextUtilities.Pluralize((int)bestScore)} from {TextUtilities.NaturalParse(bestPlayerIds.Select(i => $"{this.playerDetails[i].FirstName} {this.playerDetails[i].SecondName}").ToList())}. ");
                result.AppendLine($"On the other end of the spectrum were {TextUtilities.NaturalParse(worstTeamIds.Select(i => $"{fplTeams[i].Name}").ToList())} who had picked {TextUtilities.NaturalParse(worstPlayerIds.Select(i => $"{this.playerDetails[i].FirstName} {this.playerDetails[i].SecondName}").ToList())} for a total of {worstScore} point{TextUtilities.Pluralize((int)worstScore)}. You receive the armband of shame for this week. ");
            }

            return result.ToString();
        }

        /// <summary>
        /// Figures out which teams moved overall places compared to last week
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateTopAndBottomStandings()
        {
            this.logger.Log("Calculating top and bottom movement...");

            StringBuilder result = new StringBuilder();

            var lastWeekStandings = await this.fplService.GetLastWeekStandings();
            var currentWeekStandings = await this.fplService.GetCurrentWeekStandings();

            string previousFirstPlace = lastWeekStandings.First().Value.Name ?? string.Empty;
            string currentFirstPlace = currentWeekStandings.First().Value.Name;
            string currentSecondPlace = currentWeekStandings.Take(2).Last().Value.Name;

            long firstPlacePoints = currentWeekStandings.First().Value.Current.Find(x => x.Event == this.eventId).TotalPoints.Value;
            long secondPlacePoint = currentWeekStandings.Take(2).Last().Value.Current.Find(x => x.Event == this.eventId).TotalPoints.Value;

            long lastWeekFirstPlacePoints = currentWeekStandings.First().Value.Current.Find(x => x.Event == this.eventId - 1)?.TotalPoints.Value ?? 0;
            long lastWeeksecondPlacePoint = currentWeekStandings.Take(2).Last().Value.Current.Find(x => x.Event == this.eventId - 1)?.TotalPoints.Value ?? 0;

            string previousLastPlace = lastWeekStandings.Last().Value.Name ?? string.Empty;
            string currentLastPlace = currentWeekStandings.Last().Value.Name;
            long lastPlacePoints = currentWeekStandings.Last().Value.Current.Find(x => x.Event == this.eventId).TotalPoints.Value;

            if (previousFirstPlace == currentFirstPlace)
            {
                result.Append($"{previousFirstPlace} stay at the top of the table with {firstPlacePoints} points");

                long thisWeekDifference = firstPlacePoints - secondPlacePoint;
                long lastWeekDifference = lastWeekFirstPlacePoints - lastWeeksecondPlacePoint;

                if (thisWeekDifference > lastWeekDifference)
                {
                    result.Append($" increasing their lead to {thisWeekDifference} points ahead of {currentSecondPlace} in second place.");
                }
                else if (thisWeekDifference < lastWeekDifference)
                {
                    result.Append($", although {currentSecondPlace} is creeping closer {thisWeekDifference} points behind.");
                }
                else
                {
                    result.Append($" maintaining a lead of {thisWeekDifference} points ahead of {currentSecondPlace}.");
                }
            }
            else
            {
                result.Append($"{currentFirstPlace} with {firstPlacePoints} total points is the new league leader, supplanting last weeks leader {previousFirstPlace}.");
            }

            result.Append(" At the other end ");

            if (previousLastPlace == currentLastPlace)
            {
                result.Append($"{previousLastPlace} continues to languish in last place with {TextUtilities.GetPoorAdjective()} {lastPlacePoints} points.");
            }
            else
            {
                result.Append($"{currentLastPlace} is the new {TextUtilities.GetPoorNoun()} in last place with {TextUtilities.GetPoorAdjective()} {lastPlacePoints} points total.");
            }

            result.AppendLine();

            return result.ToString();
        }

        /// <summary>
        /// Figures out any major movements in rank for the game week
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculateMoversAndShakers()
        {
            StringBuilder result = new StringBuilder();

            var weeklyResults = await this.fplService.GetWeeklyResult();

            long highestClimbed = weeklyResults.Max(team => team.PositionChangedSinceLastWeek);
            long highestDrop = weeklyResults.Min(team => team.PositionChangedSinceLastWeek);

            var bestClimbers = weeklyResults
                .Where(team => team.PositionChangedSinceLastWeek == highestClimbed);

            var worstDroppers = weeklyResults
                .Where(team => team.PositionChangedSinceLastWeek == highestDrop);


            if (highestClimbed < 2 && highestDrop < -2)
            {
                result.Append($"There were no significants movement on the overall standings this week.");
            }
            else
            {
                if (highestClimbed > 1)
                {
                    result.Append($"As for shakers and movers {TextUtilities.NaturalParse(bestClimbers.Select(i => $"{i.Name}").ToList())} climbed {highestClimbed} spots this week.");
                }
                else
                {
                    result.Append($"Nobody made any significant climbs upwards on the table this week.");
                }

                result.Append(' ');

                if (highestDrop < -1)
                {
                    result.Append($"In the not so great department we have {TextUtilities.NaturalParse(worstDroppers.Select(i => $"{i.Name}").ToList())} dropping {Math.Abs(highestDrop)} spots.");
                }
                else
                {
                    result.Append($"On the other hand nobody made any significant drops on the table.");
                }
            }

            result.AppendLine();

            return result.ToString();
        }

        /// <summary>
        /// Calculates the highest left on bench score
        /// </summary>
        /// <returns></returns>
        private async Task<string> CalculatePointsOnBench()
        {
            this.logger.Log("Calculating points benched...");

            StringBuilder result = new StringBuilder();
            Dictionary<long, long> teams = new Dictionary<long, long>();
            long highestPoints = 0;

            var fplTeams = await this.fplService.GetTeamsAsync();


            foreach (var team in fplTeams.Values)
            {
                var history = team.Current.Find(t => t.Event == this.eventId);
                teams.Add(team.Id, history.PointsOnBench.Value);

                if (history.PointsOnBench.Value > highestPoints)
                {
                    highestPoints = history.PointsOnBench.Value;
                }
            }

            var teamsWithHighest = teams
                .GroupBy(y => y.Value)
                .OrderByDescending(y => y.Key)
                .First()
                .Select(a => fplTeams[a.Key].Name)
                .ToList();

            result.Append($"{TextUtilities.NaturalParse(teamsWithHighest)}");

            if (highestPoints > 9)
            {
                result.Append(" will be kicking themselves after having");
            }

            result.Append(" left ");

            if (highestPoints > 20)
            {
                result.Append($"{TextUtilities.GetGoodAdjective()} ");
            }

            result.AppendLine($"{highestPoints} points on the bench which was the highest in the league.");

            return result.ToString();
        }

        private async Task<string> CalculateAutomaticSubs()
        {
            this.logger.Log("Calculating automatic subs");

            StringBuilder result = new StringBuilder();

            var fplTeams = await this.fplService.GetTeamsAsync();
            var fplPlayers = await this.fplService.GetPlayersAsync();

            var allPicks = fplTeams
                .Where(team => team.Value.Squad.AutomaticSubs.Count > 0)
                .Select(team =>
                    new AutomaticSub()
                    {
                        ElementInScore = team.Value.Squad.AutomaticSubs.Sum(sub => fplPlayers[sub.ElementIn.Value].History.Find(x => x.Round == this.eventId)?.TotalPoints.Value ?? 0),
                        ElementInName = TextUtilities.NaturalParse(team.Value.Squad.AutomaticSubs.Select(sub => this.playerDetails[sub.ElementIn.Value].WebName)),
                        ElementOutName = TextUtilities.NaturalParse(team.Value.Squad.AutomaticSubs.Select(sub => this.playerDetails[sub.ElementOut.Value].WebName)),
                        TeamEntryId = team.Key,
                    })
                .OrderByDescending(x => x.ElementInScore);

            var groupedPicks = allPicks
                .GroupBy(x => x.ElementInScore)
                .OrderByDescending(x => x.Key);

            if (!allPicks.Any())
            {
                result.Append("No teams had automatic substitutions made this week. ");
            }
            else
            {
                var best = groupedPicks.First();
                var worst = groupedPicks.Last();

                if (best.Count() > 1)
                {
                    result.Append($"Managers from {TextUtilities.NaturalParse(best.Select(i => $"{fplTeams[i.TeamEntryId].Name}").ToList())} did the best with automatic substitutions this week receiving");

                    if (best.First().ElementInScore > 20)
                    {
                        result.Append($" {(TextUtilities.GetGoodAdjective())}");
                    }
                    else if (best.First().ElementInScore < 5)
                    {
                        result.Append($" {(TextUtilities.GetPoorAdjective())}");
                    }

                    result.Append($" {best.First().ElementInScore} point{TextUtilities.Pluralize((int)best.First().ElementInScore)} off the bench in liue of players not playing. ");
                }
                else
                {
                    result.Append($"This week {TextUtilities.NaturalParse(best.Select(i => $"{fplTeams[i.TeamEntryId].Name}").ToList())} did the best with automatic substitutions receiving");


                    if (best.First().ElementInScore > 20)
                    {
                        result.Append($" {(TextUtilities.GetGoodAdjective())}");
                    }
                    else if (best.First().ElementInScore < 5)
                    {
                        result.Append($" {(TextUtilities.GetPoorAdjective())}");
                    }

                    result.Append($" {best.First().ElementInScore} point{TextUtilities.Pluralize((int)best.First().ElementInScore)} from {best.First().ElementInName} coming off the bench in liue of {best.First().ElementOutName}. ");

                }

                if (best.First().ElementInScore != worst.First().ElementInScore)
                {
                    if (worst.Count() > 2)
                    {
                        result.Append($"{TextUtilities.NaturalParse(worst.Select(i => $"{fplTeams[i.TeamEntryId].Name}").ToList())} were not as fortunate getting");

                        if (worst.First().ElementInScore < 1)
                        {
                            result.Append($" {(TextUtilities.GetPoorAdjective())}");
                        }

                        result.Append($" {worst.First().ElementInScore} point{TextUtilities.Pluralize((int)worst.First().ElementInScore)} from their automatic subs. ");
                    }
                    else
                    {
                        result.Append($"{TextUtilities.NaturalParse(worst.Select(i => $"{fplTeams[i.TeamEntryId].Name}").ToList())} was not as fortunate getting");

                        if (worst.First().ElementInScore < 1)
                        {
                            result.Append($" {(TextUtilities.GetPoorAdjective())}");
                        }

                        result.Append($" {worst.First().ElementInScore} point{TextUtilities.Pluralize((int)worst.First().ElementInScore)} from {worst.First().ElementInName} who covered for {worst.First().ElementOutName}. ");
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Calculates hits taken
        /// </summary>
        /// <returns></returns>
        private async Task<string> PrintHitsTaken()
        {
            StringBuilder result = new();

            var teamBlurbs = (await this.fplService.GetWeeklyResult())
                .Where(x => x.HitsTakenCost > 0)
                .Select(y => $"{y.Name} (-{y.HitsTakenCost} pts)");

            if (teamBlurbs.Any())
            {
                string blurb = TextUtilities.NaturalParse(teamBlurbs);

                if (teamBlurbs.Count() == 1)
                {
                    result.Append($"The only team to take a hit this week was {blurb}.");
                }
                else if (teamBlurbs.Count() == 2)
                {
                    result.Append($"{blurb} both took transfer hits.");
                }
                else if (teamBlurbs.Count() > 2)
                {
                    result.Append($"{blurb} all took hits this week.");
                }

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
        private async Task<string> CalculateChipsUsed()
        {
            this.logger.Log("Calculating chips used...");

            StringBuilder result = new();

            var teamBlurbs = (await this.fplService.GetWeeklyResult())
                .Where(t => !string.IsNullOrEmpty(t.ChipUsed))
                .Select(t => $"{t.Name} ({t.ChipUsed})");

            if (!teamBlurbs.Any())
            {
                result.Append("No chips were played this week.");
            }
            else if (teamBlurbs.Count() > 5)
            {
                result.Append($"A flurry of activity this week as {TextUtilities.NaturalParse(teamBlurbs)} all played chips.");
            }
            else if (teamBlurbs.Count() == 1)
            {
                result.Append($"{TextUtilities.NaturalParse(teamBlurbs)} was the only team to use a chip this week.");
            }
            else
            {
                result.Append($"{TextUtilities.NaturalParse(teamBlurbs)} decided to spend one of their chips this week.");
            }

            return result.ToString();
        }
    }
}
