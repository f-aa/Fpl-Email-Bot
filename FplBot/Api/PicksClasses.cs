namespace FplBot.Api.Picks
{
    using global::FplBot.Api.Team;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class ApiFplTeamPicks
    {
        [JsonProperty("active_chip")]
        public object ActiveChip { get; set; }

        [JsonProperty("automatic_subs", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiFplAutomaticSub> AutomaticSubs { get; set; }

        [JsonProperty("entry_history", NullValueHandling = NullValueHandling.Ignore)]
        public ApiFplTeamEvents EntryHistory { get; set; }

        [JsonProperty("picks", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiFplPick> Picks { get; set; }
    }

    public partial class ApiFplPick
    {
        [JsonProperty("element", NullValueHandling = NullValueHandling.Ignore)]
        public long? Element { get; set; }

        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public long? Position { get; set; }

        [JsonProperty("multiplier", NullValueHandling = NullValueHandling.Ignore)]
        public long? Multiplier { get; set; }

        [JsonProperty("is_captain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCaptain { get; set; }

        [JsonProperty("is_vice_captain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsViceCaptain { get; set; }
    }

    public partial class ApiFplAutomaticSub
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
}
