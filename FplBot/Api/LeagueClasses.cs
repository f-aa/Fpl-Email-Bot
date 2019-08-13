namespace FplBot.Api.League
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class ApiLeague
    {
        [JsonProperty("league", NullValueHandling = NullValueHandling.Ignore)]
        public ApiLeagueInformation League { get; set; }

        [JsonProperty("new_entries", NullValueHandling = NullValueHandling.Ignore)]
        public ApiLeagueNewEntries NewEntries { get; set; }

        [JsonProperty("standings", NullValueHandling = NullValueHandling.Ignore)]
        public ApiLeagueStandings Standings { get; set; }
    }

    public partial class ApiLeagueInformation
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Created { get; set; }

        [JsonProperty("closed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Closed { get; set; }

        [JsonProperty("rank")]
        public object Rank { get; set; }

        [JsonProperty("max_entries")]
        public object MaxEntries { get; set; }

        [JsonProperty("league_type", NullValueHandling = NullValueHandling.Ignore)]
        public string LeagueType { get; set; }

        [JsonProperty("scoring", NullValueHandling = NullValueHandling.Ignore)]
        public string Scoring { get; set; }

        [JsonProperty("admin_entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? AdminEntry { get; set; }

        [JsonProperty("start_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartEvent { get; set; }

        [JsonProperty("code_privacy", NullValueHandling = NullValueHandling.Ignore)]
        public string CodePrivacy { get; set; }
    }

    public partial class ApiLeagueNewEntries
    {
        [JsonProperty("has_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasNext { get; set; }

        [JsonProperty("page", NullValueHandling = NullValueHandling.Ignore)]
        public long? Page { get; set; }

        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Results { get; set; }
    }

    public partial class ApiLeagueStandings
    {
        [JsonProperty("has_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasNext { get; set; }

        [JsonProperty("page", NullValueHandling = NullValueHandling.Ignore)]
        public long? Page { get; set; }

        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiLeagueFplTeams> Results { get; set; }
    }

    public partial class ApiLeagueFplTeams
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("event_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTotal { get; set; }

        [JsonProperty("player_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerName { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rank { get; set; }

        [JsonProperty("last_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastRank { get; set; }

        [JsonProperty("rank_sort", NullValueHandling = NullValueHandling.Ignore)]
        public long? RankSort { get; set; }

        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Total { get; set; }

        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public long Entry { get; set; }

        [JsonProperty("entry_name", NullValueHandling = NullValueHandling.Ignore)]
        public string EntryName { get; set; }
    }
}
