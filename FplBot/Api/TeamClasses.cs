namespace FplBot.Api.Team
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class ApiFplTeam
    {
        [JsonProperty("current", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiFplTeamEvents> Current { get; set; }

        [JsonProperty("past", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiFplTeamPreviousSeasons> Past { get; set; }

        [JsonProperty("chips", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiFplTeamChip> Chips { get; set; }
    }

    public partial class ApiFplTeamEvents
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

    public partial class ApiFplTeamPreviousSeasons
    {
        [JsonProperty("season_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SeasonName { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rank { get; set; }
    }

    public partial class ApiFplTeamChip
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Time { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }
}
