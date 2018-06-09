namespace FplBot.Api.Summary
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class FplPlayerSummary
    {
        [JsonProperty("history_past", NullValueHandling = NullValueHandling.Ignore)]
        public List<HistoryPast> HistoryPast { get; set; }

        [JsonProperty("fixtures_summary", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> FixturesSummary { get; set; }

        [JsonProperty("explain", NullValueHandling = NullValueHandling.Ignore)]
        public List<ExplainElement> Explain { get; set; }

        [JsonProperty("history_summary", NullValueHandling = NullValueHandling.Ignore)]
        public List<History> HistorySummary { get; set; }

        [JsonProperty("fixtures", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Fixtures { get; set; }

        [JsonProperty("history", NullValueHandling = NullValueHandling.Ignore)]
        public List<History> History { get; set; }
    }

    public partial class ExplainElement
    {
        [JsonProperty("explain", NullValueHandling = NullValueHandling.Ignore)]
        public ExplainExplain Explain { get; set; }

        [JsonProperty("fixture", NullValueHandling = NullValueHandling.Ignore)]
        public Fixture Fixture { get; set; }
    }

    public partial class ExplainExplain
    {
        [JsonProperty("saves", NullValueHandling = NullValueHandling.Ignore)]
        public GoalsConceded Saves { get; set; }

        [JsonProperty("minutes", NullValueHandling = NullValueHandling.Ignore)]
        public GoalsConceded Minutes { get; set; }

        [JsonProperty("goals_conceded", NullValueHandling = NullValueHandling.Ignore)]
        public GoalsConceded GoalsConceded { get; set; }
    }

    public partial class GoalsConceded
    {
        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }
    }

    public partial class Fixture
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("kickoff_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string KickoffTimeFormatted { get; set; }

        [JsonProperty("started", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Started { get; set; }

        [JsonProperty("event_day", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventDay { get; set; }

        [JsonProperty("deadline_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? DeadlineTime { get; set; }

        [JsonProperty("deadline_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string DeadlineTimeFormatted { get; set; }

        [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
        public List<Dictionary<string, Stat>> Stats { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }

        [JsonProperty("kickoff_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? KickoffTime { get; set; }

        [JsonProperty("team_h_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamHScore { get; set; }

        [JsonProperty("team_a_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamAScore { get; set; }

        [JsonProperty("finished", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Finished { get; set; }

        [JsonProperty("minutes", NullValueHandling = NullValueHandling.Ignore)]
        public long? Minutes { get; set; }

        [JsonProperty("provisional_start_time", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ProvisionalStartTime { get; set; }

        [JsonProperty("finished_provisional", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FinishedProvisional { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }

        [JsonProperty("team_a", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamA { get; set; }

        [JsonProperty("team_h", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamH { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("a", NullValueHandling = NullValueHandling.Ignore)]
        public List<A> A { get; set; }

        [JsonProperty("h", NullValueHandling = NullValueHandling.Ignore)]
        public List<A> H { get; set; }
    }

    public partial class A
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("element", NullValueHandling = NullValueHandling.Ignore)]
        public long? Element { get; set; }
    }

    public partial class History
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("kickoff_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? KickoffTime { get; set; }

        [JsonProperty("kickoff_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string KickoffTimeFormatted { get; set; }

        [JsonProperty("team_h_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamHScore { get; set; }

        [JsonProperty("team_a_score", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamAScore { get; set; }

        [JsonProperty("was_home", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WasHome { get; set; }

        [JsonProperty("round", NullValueHandling = NullValueHandling.Ignore)]
        public long? Round { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("transfers_balance", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersBalance { get; set; }

        [JsonProperty("selected", NullValueHandling = NullValueHandling.Ignore)]
        public long? Selected { get; set; }

        [JsonProperty("transfers_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersIn { get; set; }

        [JsonProperty("transfers_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersOut { get; set; }

        [JsonProperty("loaned_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoanedIn { get; set; }

        [JsonProperty("loaned_out", NullValueHandling = NullValueHandling.Ignore)]
        public long? LoanedOut { get; set; }

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

        [JsonProperty("open_play_crosses", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpenPlayCrosses { get; set; }

        [JsonProperty("big_chances_created", NullValueHandling = NullValueHandling.Ignore)]
        public long? BigChancesCreated { get; set; }

        [JsonProperty("clearances_blocks_interceptions", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClearancesBlocksInterceptions { get; set; }

        [JsonProperty("recoveries", NullValueHandling = NullValueHandling.Ignore)]
        public long? Recoveries { get; set; }

        [JsonProperty("key_passes", NullValueHandling = NullValueHandling.Ignore)]
        public long? KeyPasses { get; set; }

        [JsonProperty("tackles", NullValueHandling = NullValueHandling.Ignore)]
        public long? Tackles { get; set; }

        [JsonProperty("winning_goals", NullValueHandling = NullValueHandling.Ignore)]
        public long? WinningGoals { get; set; }

        [JsonProperty("attempted_passes", NullValueHandling = NullValueHandling.Ignore)]
        public long? AttemptedPasses { get; set; }

        [JsonProperty("completed_passes", NullValueHandling = NullValueHandling.Ignore)]
        public long? CompletedPasses { get; set; }

        [JsonProperty("penalties_conceded", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenaltiesConceded { get; set; }

        [JsonProperty("big_chances_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? BigChancesMissed { get; set; }

        [JsonProperty("errors_leading_to_goal", NullValueHandling = NullValueHandling.Ignore)]
        public long? ErrorsLeadingToGoal { get; set; }

        [JsonProperty("errors_leading_to_goal_attempt", NullValueHandling = NullValueHandling.Ignore)]
        public long? ErrorsLeadingToGoalAttempt { get; set; }

        [JsonProperty("tackled", NullValueHandling = NullValueHandling.Ignore)]
        public long? Tackled { get; set; }

        [JsonProperty("offside", NullValueHandling = NullValueHandling.Ignore)]
        public long? Offside { get; set; }

        [JsonProperty("target_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? TargetMissed { get; set; }

        [JsonProperty("fouls", NullValueHandling = NullValueHandling.Ignore)]
        public long? Fouls { get; set; }

        [JsonProperty("dribbles", NullValueHandling = NullValueHandling.Ignore)]
        public long? Dribbles { get; set; }

        [JsonProperty("element", NullValueHandling = NullValueHandling.Ignore)]
        public long? Element { get; set; }

        [JsonProperty("fixture", NullValueHandling = NullValueHandling.Ignore)]
        public long? Fixture { get; set; }

        [JsonProperty("opponent_team", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpponentTeam { get; set; }
    }

    public partial class HistoryPast
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("season_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SeasonName { get; set; }

        [JsonProperty("element_code", NullValueHandling = NullValueHandling.Ignore)]
        public long? ElementCode { get; set; }

        [JsonProperty("start_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartCost { get; set; }

        [JsonProperty("end_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? EndCost { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

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

        [JsonProperty("season", NullValueHandling = NullValueHandling.Ignore)]
        public long? Season { get; set; }
    }
}
