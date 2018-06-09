namespace FplBot.Api.Picks
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class FplPicks
    {
        [JsonProperty("active_chip", NullValueHandling = NullValueHandling.Ignore)]
        public string ActiveChip { get; set; }

        [JsonProperty("automatic_subs", NullValueHandling = NullValueHandling.Ignore)]
        public List<AutomaticSub> AutomaticSubs { get; set; }

        [JsonProperty("entry_history", NullValueHandling = NullValueHandling.Ignore)]
        public EntryHistory EntryHistory { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public Event Event { get; set; }

        [JsonProperty("picks", NullValueHandling = NullValueHandling.Ignore)]
        public List<Pick> Picks { get; set; }
    }

    public partial class AutomaticSub
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("element_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? ElementIn { get; set; }

        [JsonProperty("element_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? ElementOut { get; set; }

        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }

    public partial class EntryHistory
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("movement", NullValueHandling = NullValueHandling.Ignore)]
        public string Movement { get; set; }

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rank { get; set; }

        [JsonProperty("rank_sort", NullValueHandling = NullValueHandling.Ignore)]
        public long? RankSort { get; set; }

        [JsonProperty("overall_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? OverallRank { get; set; }

        [JsonProperty("targets")]
        public object Targets { get; set; }

        [JsonProperty("event_transfers", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfers { get; set; }

        [JsonProperty("event_transfers_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfersCost { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("points_on_bench", NullValueHandling = NullValueHandling.Ignore)]
        public long? PointsOnBench { get; set; }

        [JsonProperty("bank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bank { get; set; }

        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }

    public partial class Event
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("deadline_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? DeadlineTime { get; set; }

        [JsonProperty("average_entry_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? AverageEntryScore { get; set; }

        [JsonProperty("finished", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Finished { get; set; }

        [JsonProperty("data_checked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DataChecked { get; set; }

        [JsonProperty("highest_scoring_entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? HighestScoringEntry { get; set; }

        [JsonProperty("deadline_time_epoch", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeadlineTimeEpoch { get; set; }

        [JsonProperty("deadline_time_game_offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeadlineTimeGameOffset { get; set; }

        [JsonProperty("deadline_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string DeadlineTimeFormatted { get; set; }

        [JsonProperty("highest_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? HighestScore { get; set; }

        [JsonProperty("is_previous", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPrevious { get; set; }

        [JsonProperty("is_current", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCurrent { get; set; }

        [JsonProperty("is_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNext { get; set; }
    }

    public partial class Pick
    {
        [JsonProperty("element", NullValueHandling = NullValueHandling.Ignore)]
        public long? Element { get; set; }

        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public long? Position { get; set; }

        [JsonProperty("is_captain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCaptain { get; set; }

        [JsonProperty("is_vice_captain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsViceCaptain { get; set; }

        [JsonProperty("multiplier", NullValueHandling = NullValueHandling.Ignore)]
        public long? Multiplier { get; set; }
    }
}
