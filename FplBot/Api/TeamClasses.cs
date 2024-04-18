namespace FplBot.Api.Team
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class ApiTeamHistory
    {
        [JsonProperty("current", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiTeamEvents> Current { get; set; }

        [JsonProperty("past", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiTeamPreviousSeasons> Past { get; set; }

        [JsonProperty("chips", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiChip> Chips { get; set; }
    }

    public partial class ApiTeamEvents
    {
        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("overall_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? OverallRank { get; set; }

        [JsonProperty("bank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bank { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("event_transfers", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfers { get; set; }

        [JsonProperty("event_transfers_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfersCost { get; set; }

        [JsonProperty("points_on_bench", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsOnBench { get; set; }
    }

    public partial class ApiTeamPreviousSeasons
    {
        [JsonProperty("season_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SeasonName { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rank { get; set; }
    }

    public partial class ApiChip
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Time { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }
}
