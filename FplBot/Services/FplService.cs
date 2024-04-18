using Flurl.Http;
using FplBot.Api.League;
using FplBot.Api.Picks;
using FplBot.Api.Player;
using FplBot.Api.Root;
using FplBot.Api.Summary;
using FplBot.Api.Team;
using FplBot.Models;
using FplBot.Options;
using Microsoft.Extensions.Options;
using System.Net;

namespace FplBot.Services
{
    public class FplService(
        IPersistenceService persistenceService,
        IOptions<FplOptions> fplOptions,
        Logging.ILogger logger) : IFplService
    {
        private readonly string FplRootUri = "https://fantasy.premierleague.com/api/bootstrap-static/";
        private readonly string LeagueStandingsUri = "https://fantasy.premierleague.com/api/leagues-classic/{0}/standings/";
        private readonly string TeamUri = "https://fantasy.premierleague.com/api/entry/{0}/history/";
        private readonly string PicksUri = "https://fantasy.premierleague.com/api/entry/{0}/event/{1}/picks/";
        private readonly string PlayerSummaryUrI = "https://fantasy.premierleague.com/api/element-summary/{0}/";

        private FplRoot fplRoot;
        private ApiLeague fplLeague;
        private Dictionary<long, Team> fplTeams;
        private Dictionary<long, ApiPlayerSummary> fplPlayers;
        private IOrderedEnumerable<WeeklyResult>? weeklyResults;
        private IOrderedEnumerable<KeyValuePair<long, Team>>? lastWeekStandings;
        private IOrderedEnumerable<KeyValuePair<long, Team>>? currentWeekStandings;

        private readonly IPersistenceService persistenceService = persistenceService;
        private readonly FplOptions fplOptions = fplOptions.Value ?? throw new Exception($"Couldn't load {typeof(FplOptions)} from app settings");
        private readonly Logging.ILogger logger = logger;

        public async Task<Event> GetCurrentEventAsync()
        {
            if (this.fplRoot == null)
            {
                this.logger.Log("Fetching bootstrap-static root...");
                this.fplRoot = await this.FplRootUri.GetJsonAsync<FplRoot>().ConfigureAwait(false);
                this.logger.Log("Gameweek loaded!");
            }

            var eventId = await this.persistenceService.GetEventIdAsync();

            return this.fplRoot.Events.Find(x => x.Id == eventId);
        }

        public Dictionary<long, ApiPlayerDetail> GetSoccerPlayers() =>
            this.fplRoot.Elements.ToDictionary(player => player.Id.Value);

        /// <summary>
        /// Loads all the data from the FPL API to keep in memory
        /// </summary>
        /// <returns></returns>
        public async Task<ApiLeague> GetLeagueAsync()
        {
            if (this.fplLeague == null)
            {
                this.logger.Log("Fetching league...");

                try
                {
                    // var cookie = await this.authentication.GetCookie();
                    this.fplLeague = await string.Format(this.LeagueStandingsUri, this.fplOptions.LeagueId)
                        // .WithCookie(cookie)
                        .GetJsonAsync<ApiLeague>();

                    this.logger.Log($"Fetched {fplLeague.League.Name}");
                }
                catch (Exception ex)
                {
                    this.logger.Log($"Could not fetch data for league with ID={this.fplOptions.LeagueId}.");
                    this.logger.Log(ex.Message);
                    this.logger.Log("Terminating application.");
                    Environment.Exit(-1);

                    return null;
                }
            }

            return this.fplLeague;
        }

        public async Task<Dictionary<long, Team>> GetTeamsAsync()
        {
            if (this.fplTeams == null)
            {
                var fplLeague = await this.GetLeagueAsync(); // may not be loaded yet, so explicitly load and use
                int eventId = await this.persistenceService.GetEventIdAsync();

                this.fplTeams = [];

                foreach (ApiLeagueFplTeams standing in fplLeague.Standings.Results)
                {
                    this.logger.Log($"Fetching {standing.EntryName}...");

                    var teamTask = string.Format(this.TeamUri, standing.Entry).GetJsonAsync<ApiTeamHistory>();
                    var squadTask = string.Format(this.PicksUri, standing.Entry, eventId)
                        .AllowHttpStatus(HttpStatusCode.NotFound)   // Fix for teams that signed up after GW1
                        .GetJsonAsync<ApiSquad>();

                    await Task.WhenAll(teamTask, squadTask).ConfigureAwait(false);

                    if (squadTask.Result.EntryHistory != null)   // If team doesn't have picks, they weren't part of that gameweek
                    {
                        this.fplTeams.Add(standing.Entry, new Team()
                        {
                            Chips = teamTask.Result.Chips,
                            Current = teamTask.Result.Current,
                            Id = standing.Entry,
                            Name = standing.EntryName,
                            Past = teamTask.Result.Past,
                            Squad = squadTask.Result
                        });
                    }
                }
            }

            return this.fplTeams;
        }

        public async Task<Dictionary<long, ApiPlayerSummary>> GetPlayersAsync()
        {
            if (this.fplPlayers == null)
            {
                var fplTeams = await this.GetTeamsAsync();
                this.fplPlayers = [];

                var pickedPlayers = fplTeams
                    .SelectMany(p => p.Value.Squad.Picks.Select(pp => pp.Element.Value))
                    .Distinct();

                this.logger.Log($"Fetching detailed statistics {pickedPlayers.Count()} players...");

                // Get 
                this.fplPlayers = pickedPlayers
                    .Select(p =>
                        new KeyValuePair<long, Task<ApiPlayerSummary>>(
                            p,
                            string.Format(this.PlayerSummaryUrI, p).GetJsonAsync<ApiPlayerSummary>()))
                    .AsParallel()   // honestly don't know if this works the way I expect it to
                    .ToDictionary(x => x.Key, y => y.Value.Result);
            }

            return this.fplPlayers;
        }

        public async Task<IOrderedEnumerable<WeeklyResult>> GetWeeklyResult()
        {
            if (this.weeklyResults == null)
            {
                var fplTeams = await this.GetTeamsAsync();
                var eventId = await this.persistenceService.GetEventIdAsync();

                if (this.lastWeekStandings == null)
                    await this.GetLastWeekStandings();

                if (this.currentWeekStandings == null)
                    await this.GetCurrentWeekStandings();

                this.weeklyResults = fplTeams
                    .Select(team =>
                    {
                        ApiTeamEvents history = team.Value.Current.Find(e => e.Event == eventId);
                        string chip = team.Value.Chips.Find(c => c.Event == eventId)?.Name ?? string.Empty;

                        int calculatedLastRank = eventId == 1 ? -1 : this.lastWeekStandings
                            .Select((t, i) => new { Index = i + 1, TeamId = t.Key })
                            .First(t => t.TeamId == team.Key)
                            .Index;

                        int calculatedCurrentRank = this.currentWeekStandings
                            .Select((t, i) => new { Index = i + 1, TeamId = t.Key })
                            .First(t => t.TeamId == team.Key)
                            .Index;

                        var result = new WeeklyResult()
                        {
                            Name = fplTeams[team.Key].Name,
                            OverallRank = history.OverallRank.Value,
                            HitsTakenCost = history.EventTransfersCost.Value,
                            ScoreBeforeHits = history.Points.Value,
                            TotalPoints = history.TotalPoints.Value,
                            TotalTransfers = team.Value.Current.Where(e => e.Event <= eventId).Sum(e => e.EventTransfers).Value,
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

            return this.weeklyResults;
        }

        public async Task<IOrderedEnumerable<KeyValuePair<long, Team>>> GetLastWeekStandings()
        {
            var eventId = await this.persistenceService.GetEventIdAsync();

            this.lastWeekStandings ??= (await this.GetTeamsAsync())
                .OrderByDescending(t => t.Value.Current.DefaultIfEmpty(null).SingleOrDefault(e => e.Event == eventId - 1)?.TotalPoints ?? 0)
                .ThenBy(t => t.Value.Current.Where(e => e.Event <= eventId).Sum(e => e.EventTransfers).Value);

            return this.lastWeekStandings;
        }

        public async Task<IOrderedEnumerable<KeyValuePair<long, Team>>> GetCurrentWeekStandings()
        {
            var eventId = await this.persistenceService.GetEventIdAsync();

            this.currentWeekStandings ??= (await this.GetTeamsAsync())
                .OrderByDescending(t => t.Value.Current.Find(e => e.Event == eventId).TotalPoints)
                .ThenBy(t => t.Value.Current.Where(e => e.Event <= eventId).Sum(e => e.EventTransfers).Value);

            return this.currentWeekStandings;
        }
    }
}
