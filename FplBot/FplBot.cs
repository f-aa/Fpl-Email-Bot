using Flurl.Http;
using FplBot.Api;
using FplBot.Data;
using FplBot.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FplBot
{
    /// <summary>
    /// A class that's responsible for connecting to and processing the FPL API
    /// </summary>
    internal class FplBot
    {
        private readonly string FplRootUri = "https://fantasy.premierleague.com/api/bootstrap-static/";
        private readonly string LeagueStandingsUri = "https://fantasy.premierleague.com/api/leagues-classic/{0}/standings/";
        private readonly string TeamUri = "https://fantasy.premierleague.com/api/entry/{0}/history/";
        private readonly string PicksUri = "https://fantasy.premierleague.com/api/entry/{0}/event/{1}/picks/";
        private readonly string PlayerSummaryUrI = "https://fantasy.premierleague.com/api/element-summary/{0}/";
        private readonly Dictionary<long, FplTeamEntry> fplTeams;
        private readonly Dictionary<long, Api.Picks.ApiFplTeamPicks> fplPicks;
        private readonly Dictionary<long, Api.Summary.ApiSoccerPlayerSummary> fplPlayerSummaries;
        private readonly Authentication authentication;
        private readonly ILogger logger;

        private readonly long leagueId;
        private readonly bool attachTable;
        private readonly string emailUser;
        private readonly string emailPassword;
        private readonly string emailFrom;
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly bool useAzure;
        private readonly string azureBlobName;
        private readonly int interval;
        private readonly string[] recipients;
        private readonly Persistence persistence;

        IOrderedEnumerable<TeamWeeklyResult> weeklyResults;
        IOrderedEnumerable<KeyValuePair<long, FplTeamEntry>> lastWeekStandings;
        IOrderedEnumerable<KeyValuePair<long, FplTeamEntry>> currentWeekStandings;
        private Api.Root.FplRoot fplRoot;
        private Dictionary<long, Api.Player.ApiSoccerPlayer> soccerPlayers;
        private Api.League.ApiLeague fplLeague;
        private Api.Root.Event currentEvent;

        private int currentEventId;
        private bool isInitialized = false;

        public StringBuilder Output { get; set; }

        /// <summary>
        /// Initializes a new instance of the Fpl class
        /// </summary>
        /// <param name="gameweekToProcess"></param>
        internal FplBot(ILogger logger)
        {
            this.logger = logger;

            string fplUsername = string.Empty;
            string fplPassword = string.Empty;
            try
            {
                this.logger.Log("Loading configuration file");

                this.attachTable = bool.Parse(ConfigurationManager.AppSettings["attachTable"]);
                this.leagueId = long.Parse(ConfigurationManager.AppSettings["leagueId"]);
                this.emailUser = ConfigurationManager.AppSettings["emailUser"];
                this.emailPassword = ConfigurationManager.AppSettings["emailPassword"];
                this.emailFrom = ConfigurationManager.AppSettings["emailFrom"];
                this.smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                this.smtpPort = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                this.useAzure = bool.Parse(ConfigurationManager.AppSettings["useAzure"]);
                this.azureBlobName = ConfigurationManager.AppSettings["azureBlobName"];
                this.interval = int.Parse(ConfigurationManager.AppSettings["interval"]);

                fplUsername = ConfigurationManager.AppSettings["fplUsername"];
                fplPassword = ConfigurationManager.AppSettings["fplPassword"];

                if (string.IsNullOrWhiteSpace(this.emailUser) ||
                    string.IsNullOrWhiteSpace(this.emailPassword) ||
                    string.IsNullOrWhiteSpace(this.emailFrom) ||
                    string.IsNullOrWhiteSpace(this.smtpServer) ||
                    string.IsNullOrWhiteSpace(fplUsername) ||
                    string.IsNullOrWhiteSpace(fplPassword) ||
                    (useAzure && string.IsNullOrWhiteSpace(this.azureBlobName)))
                {
                    throw new ArgumentException("Missing or incorrect configuration. Please make sure your FplBot.exe.config file is properly configured.");
                }

                this.recipients = ConfigurationManager.AppSettings["emailTo"].Split(';');
                if (recipients == null || recipients.Count() < 1) throw new ArgumentException("Email recipients are not configured properly.");
            }
            catch (Exception ex)
            {
                this.logger.Log($"Error trying to read FPL Bot configuration: {ex.Message}.");
                Environment.Exit(-1);
            }

            this.fplTeams = new Dictionary<long, FplTeamEntry>();
            this.fplPicks = new Dictionary<long, Api.Picks.ApiFplTeamPicks>();
            this.fplPlayerSummaries = new Dictionary<long, Api.Summary.ApiSoccerPlayerSummary>();
            this.persistence = new Persistence(this.logger, this.useAzure, this.azureBlobName);
            this.authentication = new Authentication(fplUsername, fplPassword, this.logger);

            this.Output = new StringBuilder();
        }

        /// <summary>
        /// Initialize the FplBot
        /// </summary>
        /// <returns></returns>
        internal async Task Initialize()
        {
            this.logger.Log("Initializing FPL Bot");

            this.persistence.Initialize();
            this.currentEventId = this.persistence.GetGameweek();

            this.logger.Log("Fetching bootstrap-static root...");
            this.fplRoot = await this.FplRootUri.GetJsonAsync<Api.Root.FplRoot>().ConfigureAwait(false);
            this.soccerPlayers = this.fplRoot.Elements.ToDictionary(player => player.Id.Value);
            this.currentEvent = this.fplRoot.Events.Find(x => x.Id == this.currentEventId);

            this.isInitialized = true;
            this.logger.Log("Initializing completed!");
        }

        /// <summary>
        /// Processes the information from the FPL API
        /// </summary>
        internal async Task Process()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (this.isInitialized &&
                this.currentEvent != null &&
                this.currentEvent.Finished.Value)   // Make sure gameweek is finished
            {
                this.Output.AppendLine($"Beep boop! I am a robot. This is your weekly FPL update.").AppendLine();

                await this.LoadApiDataAsync();

                if (this.fplTeams.Count() < 1)
                {
                    this.logger.Log($"Could not find any teams in league {this.fplLeague.League.Name}. Did you enter the correct ID? Terminating application.");
                    Environment.Exit(-1);
                }

                this.CalculateWeeklyRank();

                string topResults = this.CalculateTop();
                string bottomResults = this.CalculateBottom();
                string averageResults = this.CalculateAverages();
                string captains = await this.CalculateCaptains();
                string movement = this.currentEventId > 1 ? this.CalculateTopAndBottomStandings() : string.Empty;
                string moversShakers = this.currentEventId > 1 ? this.CalculateMoversAndShakers() : string.Empty;
                string pointsBenched = this.CalculatePointsOnBench();
                string autoSubs = this.CalculateAutomaticSubs();
                string hitsTaken = this.PrintHitsTaken();
                string chipsUsed = this.CalculateChipsUsed();

                this.Output.AppendLine(topResults);
                this.Output.AppendLine(bottomResults);
                this.Output.AppendLine(captains);
                this.Output.Append(autoSubs).AppendLine(pointsBenched);

                if (!string.IsNullOrEmpty(movement))
                {
                    this.Output.AppendLine(movement);
                    this.Output.AppendLine(moversShakers);
                }

                if (this.attachTable)
                {
                    StringBuilder standings = this.GenerateStandingsTable();
                    persistence.SaveStandings(standings);
                }

                this.Output.AppendLine();
                this.Output.AppendLine("Notable news:").AppendLine();

                this.Output.Append("- ").AppendLine(averageResults);
                this.Output.Append("- ").AppendLine(hitsTaken);
                this.Output.Append("- ").AppendLine(chipsUsed);

                this.Output.AppendLine().AppendLine("Your friendly FPL bot will return next gameweek with another update.");


                if(!this.SendEmail())
                {
                    this.logger.Log("FPL Bot failed to send email. Terminating application.");
                    Environment.Exit(-1);
                }

                this.persistence.CompleteGameweek();

                stopwatch.Stop();
                this.logger.Log($"Beep boop. This is diagnostics. Completed FPL processing in {((double)stopwatch.ElapsedMilliseconds / 1000).ToString("N1")} seconds.");
            }
            else if (!(this.currentEvent.Finished.Value && this.currentEvent.DataChecked.Value))
            {
                this.logger.Log("Gameweek has not completed yet.");
            }
        }

        /// <summary>
        /// Sleeps the process for the configured amount of time
        /// </summary>
        internal void Wait()
        {
            this.logger.Log($"Waiting {this.interval} seconds to retry...");
            Thread.Sleep(this.interval * 1000);
        }

        /// <summary>
        /// Loads all the data from the FPL API to keep in memory
        /// </summary>
        /// <returns></returns>
        private async Task LoadApiDataAsync()
        {
            this.logger.Log("Fetching league...");

            try
            {
                var cookie = await this.authentication.GetCookie();
                this.fplLeague = await string.Format(this.LeagueStandingsUri, this.leagueId)
                    .WithCookie(cookie)
                    .GetJsonAsync<Api.League.ApiLeague>();

                this.logger.Log($"Fetched {this.fplLeague.League.Name}");
            }
            catch (Exception ex)
            {
                this.logger.Log($"Could not fetch data for league with ID={this.leagueId}.");
                this.logger.Log(ex.Message);
                this.logger.Log("Terminating application.");
                Environment.Exit(-1);
            }

            foreach (Api.League.ApiLeagueFplTeams standing in this.fplLeague.Standings.Results.AsParallel())
            {
                this.logger.Log($"Fetching {standing.EntryName}...");

                var teamTask = string.Format(this.TeamUri, standing.Entry).GetJsonAsync<Api.Team.ApiFplTeam>();
                var picksTask = string.Format(this.PicksUri, standing.Entry, this.currentEventId)
                    .AllowHttpStatus(HttpStatusCode.NotFound)   // Fix for teams that signed up after GW1
                    .GetJsonAsync<Api.Picks.ApiFplTeamPicks>();

                await Task.WhenAll(teamTask, picksTask).ConfigureAwait(false);

                if (picksTask.Result.EntryHistory != null)   // If team doesn't have picks, they weren't part of that gameweek
                {
                    this.fplTeams.Add(standing.Entry, new FplTeamEntry()
                    {
                        Chips = teamTask.Result.Chips,
                        Current = teamTask.Result.Current,
                        Id = standing.Entry,
                        Name = standing.EntryName,
                        Past = teamTask.Result.Past
                    });

                    this.fplPicks.Add(standing.Entry, picksTask.Result);
                }
            }

            var pickedPlayers = this.fplPicks.SelectMany(p => p.Value.Picks.Select(pp => pp.Element.Value)).Distinct();
            this.logger.Log($"Fetching detailed statistics {pickedPlayers.Count()} players...");
            foreach (var p in pickedPlayers.AsParallel())
            {
                if (!this.fplPlayerSummaries.ContainsKey(p))
                {
                    Api.Summary.ApiSoccerPlayerSummary summary = await string.Format(this.PlayerSummaryUrI, p).GetJsonAsync<Api.Summary.ApiSoccerPlayerSummary>().ConfigureAwait(false);
                    this.fplPlayerSummaries.Add(p, summary);
                }
            }
        }

        /// <summary>
        /// Calculates the weekly rank for all the teams
        /// </summary>
        private void CalculateWeeklyRank()
        {
            this.lastWeekStandings = this.fplTeams
                .OrderByDescending(t => t.Value.Current.DefaultIfEmpty(null).SingleOrDefault(e => e.Event == this.currentEventId - 1)?.TotalPoints ?? 0)
                .ThenBy(t => t.Value.Current.Where(e => e.Event <= this.currentEventId).Sum(e => e.EventTransfers).Value);

            this.currentWeekStandings = this.fplTeams
                .OrderByDescending(t => t.Value.Current.Find(e => e.Event == this.currentEventId).TotalPoints)
                .ThenBy(t => t.Value.Current.Where(e => e.Event <= this.currentEventId).Sum(e => e.EventTransfers).Value);

            this.weeklyResults = this.fplTeams
                .Select(team =>
                {
                    Api.Team.ApiFplTeamEvents history = team.Value.Current.Find(e => e.Event == this.currentEventId);
                    string chip = team.Value.Chips.Find(c => c.Event == this.currentEventId)?.Name ?? string.Empty;


                    int calculatedLastRank = this.currentEventId == 1 ? -1 : this.lastWeekStandings
                        .Select((t, i) => new { Index = i + 1, TeamId = t.Key })
                        .First(t => t.TeamId == team.Key)
                        .Index;

                    int calculatedCurrentRank = this.currentWeekStandings
                        .Select((t, i) => new { Index = i + 1, TeamId = t.Key })
                        .First(t => t.TeamId == team.Key)
                        .Index;

                    var result = new TeamWeeklyResult()
                    {
                        Name = this.fplLeague.Standings.Results.FirstOrDefault(t => t.Entry == team.Key).EntryName, // TODO: maube we need an actual dictionary of the teams in the league, seems to not exist anymore
                        OverallRank = history.OverallRank.Value,
                        HitsTakenCost = history.EventTransfersCost.Value,
                        ScoreBeforeHits = history.Points.Value,
                        TotalPoints = history.TotalPoints.Value,
                        TotalTransfers = team.Value.Current.Where(e => e.Event <= this.currentEventId).Sum(e => e.EventTransfers).Value,
                        TeamValue = history.Value.Value / 10f,
                        GameWeekPoints = history.Points.Value,
                        ChipUsed = chip,
                        PreviousWeekPosition = calculatedLastRank,
                        CurrentWeekPosition = calculatedCurrentRank
                    };

                    return result;
                })
                .OrderByDescending(x => x.Points);
        }

        /// <summary>
        /// Calculates the top 5(ish) placements for this week
        /// </summary>
        /// <returns></returns>
        private string CalculateTop()
        {
            this.logger.Log("Calculating top positions...");
            
            StringBuilder result = new StringBuilder();

            long topScore = this.weeklyResults.Max(t => t.Points);
            long topGrossScore = this.weeklyResults.Max(t => t.ScoreBeforeHits);
            long top3Score = this.weeklyResults.Take(3).Last().Points;

            IEnumerable<string> winnerNames = this.weeklyResults                    // This weeks winner(s)
                .Where(x => x.Points == topScore)                                   // Find all teams with the top score
                .Select(x => x.Name);                                               // Get their name(s)

            IEnumerable<string> topNamesBeforeTransferCost = this.weeklyResults     // Used for Dan Davies rule
                .OrderByDescending(x => x.ScoreBeforeHits)
                .Where(x => x.ScoreBeforeHits == topGrossScore)
                .Select(x => x.Name);

            IEnumerable<string> topNames = this.weeklyResults                       // Top 3ish teams for this week
                .Where(x => x.Points >= top3Score)                                  // Find anyone that had top 3 score or better
                .Where(x => x.Points < topScore)                                    // Exlude the first one
                .Select(x => $"{x.Name} ({x.Points} pts)");                         // Prepare text

            bool daviesRuleInEffect = !(topNamesBeforeTransferCost.All(x => winnerNames.Contains(x)) && winnerNames.All(x => topNamesBeforeTransferCost.Contains(x)));
            long winnerHitCost = this.weeklyResults.First().HitsTakenCost;

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
        private string CalculateBottom()
        {
            this.logger.Log("Calculating bottom positions...");

            StringBuilder result = new StringBuilder();

            IEnumerable<string> worstTeams = this.weeklyResults // The worst performing teams of the week
                .Skip(this.weeklyResults.Count() - 4)           // Get the last 4 teams
                .Select(t => $"{t.Name} ({t.Points} pts)");     // Prepare text

            result.AppendLine($"The worst ranking teams this week were {TextUtilities.NaturalParse(worstTeams)}. You should probably be embarrassed. ");

            return result.ToString();
        }

        /// <summary>
        /// Calculates the average score, and how many players beat it
        /// </summary>
        /// <returns></returns>
        private string CalculateAverages()
        {
            this.logger.Log("Calculating averages...");

            StringBuilder result = new StringBuilder();

            long overallAverage = this.currentEvent.AverageEntryScore.Value;
            long teamsAtAverageOrBetter = this.weeklyResults.Where(x => x.Points > overallAverage).Count();

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
                    Api.Summary.ApiSoccerPlayerSummary summary = await string.Format(this.PlayerSummaryUrI, p.Key).GetJsonAsync<Api.Summary.ApiSoccerPlayerSummary>().ConfigureAwait(false);
                    this.fplPlayerSummaries.Add(p.Key, summary);
                }
            }

            foreach (var p in groupedViceCaptains.AsParallel())
            {
                if (!this.fplPlayerSummaries.ContainsKey(p.Key))
                {
                    Api.Summary.ApiSoccerPlayerSummary summary = await string.Format(this.PlayerSummaryUrI, p.Key).GetJsonAsync<Api.Summary.ApiSoccerPlayerSummary>().ConfigureAwait(false);
                    this.fplPlayerSummaries.Add(p.Key, summary);
                }
            }

            List<CaptainChoice> captains = new List<CaptainChoice>();

            foreach (var pick in this.fplPicks)
            {
                long teamId = pick.Key;
                var cptPick = pick.Value.Picks.Find(p => p.IsCaptain.Value);
                var vicePick = pick.Value.Picks.Find(p => p.IsViceCaptain.Value);

                var cptPlayer = this.soccerPlayers[cptPick.Element.Value];
                var vicePlayer = this.soccerPlayers[vicePick.Element.Value];

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

            int noBest = groupedScores.FirstOrDefault()?.Count() ?? 0;
            int noWorst = groupedScores.LastOrDefault()?.Count() ?? 0;
            long bestScore = groupedScores.FirstOrDefault()?.Key ?? 0;
            long worstScore = groupedScores.LastOrDefault()?.Key ?? 0;

            List<long> bestPlayerIds = new List<long>();
            List<long> worstPlayerIds = new List<long>();
            List<long> bestTeamIds = new List<long>();
            List<long> worstTeamIds = new List<long>();

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

                result.Append($"When it came to captaincy choice{TextUtilities.Pluralize(noBest)} {TextUtilities.NaturalParse(bestTeamIds.Select(i => $"{this.fplTeams[i].Name}").ToList())} did the best this week with {bestScore} point{TextUtilities.Pluralize(noBest)} from {TextUtilities.NaturalParse(bestPlayerIds.Select(i => $"{this.soccerPlayers[i].FirstName} {this.soccerPlayers[i].SecondName}").ToList())}. ");
                result.AppendLine($"On the other end of the spectrum were {TextUtilities.NaturalParse(worstTeamIds.Select(i => $"{this.fplTeams[i].Name}").ToList())} who had picked {TextUtilities.NaturalParse(worstPlayerIds.Select(i => $"{this.soccerPlayers[i].FirstName} {this.soccerPlayers[i].SecondName}").ToList())} for a total of {worstScore} point{TextUtilities.Pluralize((int)worstScore)}. You receive the armband of shame for this week. ");
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Figures out which teams moved overall places compared to last week
        /// </summary>
        /// <returns></returns>
        private string CalculateTopAndBottomStandings()
        {
            this.logger.Log("Calculating top and bottom movement...");

            StringBuilder result = new StringBuilder();

            string previousFirstPlace = this.lastWeekStandings.First().Value.Name ?? string.Empty;
            string currentFirstPlace = this.currentWeekStandings.First().Value.Name;
            string currentSecondPlace = this.currentWeekStandings.Take(2).Last().Value.Name;

            long firstPlacePoints = this.currentWeekStandings.First().Value.Current.Find(x => x.Event == this.currentEventId).TotalPoints.Value;
            long secondPlacePoint = this.currentWeekStandings.Take(2).Last().Value.Current.Find(x => x.Event == this.currentEventId).TotalPoints.Value;

            long lastWeekFirstPlacePoints = this.currentWeekStandings.First().Value.Current.Find(x => x.Event == this.currentEventId - 1)?.TotalPoints.Value ?? 0;
            long lastWeeksecondPlacePoint = this.currentWeekStandings.Take(2).Last().Value.Current.Find(x => x.Event == this.currentEventId - 1)?.TotalPoints.Value ?? 0;

            string previousLastPlace = this.lastWeekStandings.Last().Value.Name ?? string.Empty;
            string currentLastPlace = this.currentWeekStandings.Last().Value.Name;
            long lastPlacePoints = this.currentWeekStandings.Last().Value.Current.Find(x => x.Event == this.currentEventId).TotalPoints.Value;

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
        private string CalculateMoversAndShakers()
        {
            StringBuilder result = new StringBuilder();

            long highestClimbed = this.weeklyResults.Max(team => team.PositionChangedSinceLastWeek);
            long highestDrop = this.weeklyResults.Min(team => team.PositionChangedSinceLastWeek);

            var bestClimbers = this.weeklyResults
                .Where(team => team.PositionChangedSinceLastWeek == highestClimbed);

            var worstDroppers = this.weeklyResults
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

                result.Append(" ");

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
        private string CalculatePointsOnBench()
        {
            this.logger.Log("Calculating points benched...");

            StringBuilder result = new StringBuilder();
            Dictionary<long, long> teams = new Dictionary<long, long>();
            long highestPoints = 0;

            foreach (var team in this.fplTeams.Values)
            {
                var history = team.Current.Find(t => t.Event == this.currentEventId);
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
                .Select(a => this.fplTeams[a.Key].Name)
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

        private string CalculateAutomaticSubs()
        {
            this.logger.Log("Calculating automatic subs");

            StringBuilder result = new StringBuilder();

            var allPicks = this.fplPicks
                .Where(team => team.Value.AutomaticSubs.Count() > 0)
                .Select(team =>
                    new AutomaticSub()
                    {
                        ElementInScore = team.Value.AutomaticSubs.Sum(sub => this.fplPlayerSummaries[sub.ElementIn.Value].History.Find(x => x.Round == this.currentEventId)?.TotalPoints.Value ?? 0),
                        ElementInName = TextUtilities.NaturalParse(team.Value.AutomaticSubs.Select(sub => this.soccerPlayers[sub.ElementIn.Value].WebName)),
                        ElementOutName = TextUtilities.NaturalParse(team.Value.AutomaticSubs.Select(sub => this.soccerPlayers[sub.ElementOut.Value].WebName)),
                        TeamEntryId = team.Key,
                    })
                .OrderByDescending(x => x.ElementInScore);
            
            var groupedPicks = allPicks
                .GroupBy(x => x.ElementInScore)
                .OrderByDescending(x => x.Key);

            if (allPicks.Count() < 1)
            {
                result.Append("No teams had automatic substitutions made this week. ");
            }
            else
            {
                var best = groupedPicks.First();
                var worst = groupedPicks.Last();

                if (best.Count() > 1)
                {
                    result.Append($"Managers from {TextUtilities.NaturalParse(best.Select(i => $"{this.fplTeams[i.TeamEntryId].Name}").ToList())} did the best with automatic substitutions this week receiving");

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
                    result.Append($"This week {TextUtilities.NaturalParse(best.Select(i => $"{this.fplTeams[i.TeamEntryId].Name}").ToList())} did the best with automatic substitutions receiving");


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
                        result.Append($"{TextUtilities.NaturalParse(worst.Select(i => $"{this.fplTeams[i.TeamEntryId].Name}").ToList())} were not as fortunate getting");
                        
                        if (worst.First().ElementInScore < 1)
                        {
                            result.Append($" {(TextUtilities.GetPoorAdjective())}");
                        }

                        result.Append($" {worst.First().ElementInScore} point{TextUtilities.Pluralize((int)worst.First().ElementInScore)} from their automatic subs. ");
                    }
                    else
                    {
                        result.Append($"{TextUtilities.NaturalParse(worst.Select(i => $"{this.fplTeams[i.TeamEntryId].Name}").ToList())} was not as fortunate getting"); 

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
        private string PrintHitsTaken()
        {
            StringBuilder result = new StringBuilder();

            var teamBlurbs = this.weeklyResults
                .Where(x => x.HitsTakenCost > 0)
                .Select(y => $"{y.Name} (-{y.HitsTakenCost} pts)");

            if (teamBlurbs.Count() > 0)
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
        private string CalculateChipsUsed()
        {
            this.logger.Log("Calculating chips used...");

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

        /// <summary>
        /// Generates the current overall standings for this league
        /// </summary>
        /// <returns>A StringBuilder object with the current standings in plaintext format</returns>
        private StringBuilder GenerateStandingsTable()
        {
            if (this.currentWeekStandings == null) return null;

            StringBuilder standings = new StringBuilder();

            const int dashPadding = 49;
            int longestTeamName = this.currentWeekStandings.Max(team => team.Value.Name.Length);

            standings.AppendLine($"Standings for {this.fplLeague.League.Name} after {this.currentEvent.Name}:");
            standings.AppendLine("".PadLeft(longestTeamName + dashPadding, '-'));
            standings.AppendLine($"Rank Chg. PW   Overall  Team{string.Empty.PadLeft(longestTeamName - 3)}  GW  Total   TT   TmVal");
            standings.AppendLine("".PadLeft(longestTeamName + dashPadding, '-')).AppendLine();

            foreach (var team in this.weeklyResults.OrderBy(x => x.CurrentWeekPosition))
            {
                // if this is the first gameweek, there was no rank last week so we'll return -1 and print out -- for movement
                string currentRank = team.CurrentWeekPosition.ToString().PadRight(2);
                string previousRank = this.currentEventId == 1 ? "--" : team.PreviousWeekPosition.ToString().PadRight(2);
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
            standings.AppendLine($"Rank:    Current rank in league {this.fplLeague.League.Name}");
            standings.AppendLine("Chg.:    Movement in league compared to previous week");
            standings.AppendLine("PW:      Previous week rank in league");
            standings.AppendLine("Overall: Rank amongst all players in FPL");
            standings.AppendLine("GW:      Game week points");
            standings.AppendLine("Total:   Point sum of all game weeks");
            standings.AppendLine("TT:      Total transfers");
            standings.AppendLine("TmVal:   Team value (including bank)");

            return standings;
        }

        /// <summary>
        /// Sends an email to participants in the league
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>Email addresses must be configured in the app.config file, as the API does not supply the addresses.</remarks>
        private bool SendEmail()
        {
            this.logger.Log("Attempting to send email...");

            Stream stream = null;

            try
            {
                if (this.Output == null) throw new Exception("Could not find an output to email.");

                var multipart = new Multipart("mixed");
                var body = new TextPart(TextFormat.Plain) { Text = this.Output.ToString() };

                multipart.Add(body);

                if (this.attachTable)
                {
                    stream = persistence.GetStandingsStream();

                    MimePart attachment = new MimePart("plain", "txt")
                    {
                        Content = new MimeContent(stream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName($"Standings-{this.currentEvent.Name.Replace(" ", "")}.txt")
                    };

                    multipart.Add(attachment);
                }

                MimeMessage message = new MimeMessage();
                message.Subject = "Weekly update from your friendly FPL bot!";
                message.Body = multipart;
                message.From.Add(new MailboxAddress(this.emailFrom));

                foreach (var r in this.recipients)
                {
                    if (!string.IsNullOrEmpty(r))
                    {
                        message.To.Add(new MailboxAddress(r));
                    }
                }

                using (SmtpClient emailClient = new SmtpClient())
                {
                    this.logger.Log($"Connecting to {this.smtpServer}...");
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Connect(this.smtpServer, this.smtpPort, false);
                    emailClient.Authenticate(this.emailUser, this.emailPassword);
                    emailClient.Send(message);
                    emailClient.Disconnect(true);
                    this.logger.Log("Email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                this.logger.Log($"Unable to send email: {ex.Message}");
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return true;
        }
    }
}
