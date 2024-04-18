namespace FplBot.Api.Player
{
    using Newtonsoft.Json;
    using System;

    public partial class ApiPlayerDetail
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("photo", NullValueHandling = NullValueHandling.Ignore)]
        public string Photo { get; set; }

        [JsonProperty("web_name", NullValueHandling = NullValueHandling.Ignore)]
        public string WebName { get; set; }

        [JsonProperty("team_code", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamCode { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }

        [JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("second_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SecondName { get; set; }

        [JsonProperty("squad_number")]
        public long? SquadNumber { get; set; }

        [JsonProperty("news", NullValueHandling = NullValueHandling.Ignore)]
        public string News { get; set; }

        [JsonProperty("now_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? NowCost { get; set; }

        [JsonProperty("news_added")]
        public DateTimeOffset? NewsAdded { get; set; }

        [JsonProperty("chance_of_playing_this_round")]
        public long? ChanceOfPlayingThisRound { get; set; }

        [JsonProperty("chance_of_playing_next_round")]
        public long? ChanceOfPlayingNextRound { get; set; }

        [JsonProperty("value_form", NullValueHandling = NullValueHandling.Ignore)]
        public string ValueForm { get; set; }

        [JsonProperty("value_season", NullValueHandling = NullValueHandling.Ignore)]
        public string ValueSeason { get; set; }

        [JsonProperty("cost_change_start", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostChangeStart { get; set; }

        [JsonProperty("cost_change_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostChangeEvent { get; set; }

        [JsonProperty("cost_change_start_fall", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostChangeStartFall { get; set; }

        [JsonProperty("cost_change_event_fall", NullValueHandling = NullValueHandling.Ignore)]
        public long? CostChangeEventFall { get; set; }

        [JsonProperty("in_dreamteam", NullValueHandling = NullValueHandling.Ignore)]
        public bool? InDreamteam { get; set; }

        [JsonProperty("dreamteam_count", NullValueHandling = NullValueHandling.Ignore)]
        public long? DreamteamCount { get; set; }

        [JsonProperty("selected_by_percent", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedByPercent { get; set; }

        [JsonProperty("form", NullValueHandling = NullValueHandling.Ignore)]
        public string Form { get; set; }

        [JsonProperty("transfers_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersOut { get; set; }

        [JsonProperty("transfers_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersIn { get; set; }

        [JsonProperty("transfers_out_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersOutEvent { get; set; }

        [JsonProperty("transfers_in_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersInEvent { get; set; }

        [JsonProperty("loans_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoansIn { get; set; }

        [JsonProperty("loans_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoansOut { get; set; }

        [JsonProperty("loaned_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoanedIn { get; set; }

        [JsonProperty("loaned_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoanedOut { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("event_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventPoints { get; set; }

        [JsonProperty("points_per_game", NullValueHandling = NullValueHandling.Ignore)]
        public string PointsPerGame { get; set; }

        [JsonProperty("ep_this", NullValueHandling = NullValueHandling.Ignore)]
        public string EpThis { get; set; }

        [JsonProperty("ep_next", NullValueHandling = NullValueHandling.Ignore)]
        public string EpNext { get; set; }

        [JsonProperty("special", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Special { get; set; }

        [JsonProperty("minutes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Minutes { get; set; }

        [JsonProperty("goals_scored", NullValueHandling = NullValueHandling.Ignore)]
        public long? GoalsScored { get; set; }

        [JsonProperty("assists", NullValueHandling = NullValueHandling.Ignore)]
        public long? Assists { get; set; }

        [JsonProperty("clean_sheets", NullValueHandling = NullValueHandling.Ignore)]
        public long? CleanSheets { get; set; }

        [JsonProperty("goals_conceded", NullValueHandling = NullValueHandling.Ignore)]
        public long? GoalsConceded { get; set; }

        [JsonProperty("own_goals", NullValueHandling = NullValueHandling.Ignore)]
        public long? OwnGoals { get; set; }

        [JsonProperty("penalties_saved", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenaltiesSaved { get; set; }

        [JsonProperty("penalties_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenaltiesMissed { get; set; }

        [JsonProperty("yellow_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? YellowCards { get; set; }

        [JsonProperty("red_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? RedCards { get; set; }

        [JsonProperty("saves", NullValueHandling = NullValueHandling.Ignore)]
        public long? Saves { get; set; }

        [JsonProperty("bonus", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bonus { get; set; }

        [JsonProperty("bps", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bps { get; set; }

        [JsonProperty("influence", NullValueHandling = NullValueHandling.Ignore)]
        public string Influence { get; set; }

        [JsonProperty("creativity", NullValueHandling = NullValueHandling.Ignore)]
        public string Creativity { get; set; }

        [JsonProperty("threat", NullValueHandling = NullValueHandling.Ignore)]
        public string Threat { get; set; }

        [JsonProperty("ict_index", NullValueHandling = NullValueHandling.Ignore)]
        public string IctIndex { get; set; }

        [JsonProperty("ea_index", NullValueHandling = NullValueHandling.Ignore)]
        public long? EaIndex { get; set; }

        [JsonProperty("element_type", NullValueHandling = NullValueHandling.Ignore)]
        public long? ElementType { get; set; }

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public long? Team { get; set; }
    }
}
