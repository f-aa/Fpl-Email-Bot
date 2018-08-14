namespace FplBot.Api.Season
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class FplSeason
    {
        [JsonProperty("new_entries", NullValueHandling = NullValueHandling.Ignore)]
        public NewEntries NewEntries { get; set; }

        [JsonProperty("league", NullValueHandling = NullValueHandling.Ignore)]
        public League League { get; set; }

        [JsonProperty("standings", NullValueHandling = NullValueHandling.Ignore)]
        public Standings Standings { get; set; }

        [JsonProperty("update_status", NullValueHandling = NullValueHandling.Ignore)]
        public long? UpdateStatus { get; set; }
    }

    public partial class League
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("leagueban_set", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> LeaguebanSet { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public object ShortName { get; set; }

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Created { get; set; }

        [JsonProperty("closed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Closed { get; set; }

        [JsonProperty("forum_disabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ForumDisabled { get; set; }

        [JsonProperty("make_code_public", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MakeCodePublic { get; set; }

        [JsonProperty("rank")]
        public object Rank { get; set; }

        [JsonProperty("size")]
        public object Size { get; set; }

        [JsonProperty("league_type", NullValueHandling = NullValueHandling.Ignore)]
        public string LeagueType { get; set; }

        [JsonProperty("_scoring", NullValueHandling = NullValueHandling.Ignore)]
        public string Scoring { get; set; }

        [JsonProperty("reprocess_standings", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReprocessStandings { get; set; }

        [JsonProperty("admin_entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? AdminEntry { get; set; }

        [JsonProperty("start_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartEvent { get; set; }
    }

    public partial class NewEntries
    {
        [JsonProperty("has_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasNext { get; set; }

        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
        public long? Number { get; set; }

        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Results { get; set; }
    }

    public partial class Standings
    {
        [JsonProperty("has_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasNext { get; set; }

        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
        public long? Number { get; set; }

        [JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
        public List<Result> Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("entry_name", NullValueHandling = NullValueHandling.Ignore)]
        public string EntryName { get; set; }

        [JsonProperty("event_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTotal { get; set; }

        [JsonProperty("player_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerName { get; set; }

        [JsonProperty("movement", NullValueHandling = NullValueHandling.Ignore)]
        public string Movement { get; set; }

        [JsonProperty("own_entry", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OwnEntry { get; set; }

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

        [JsonProperty("league", NullValueHandling = NullValueHandling.Ignore)]
        public long? League { get; set; }

        [JsonProperty("start_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartEvent { get; set; }

        [JsonProperty("stop_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StopEvent { get; set; }
    }
}
