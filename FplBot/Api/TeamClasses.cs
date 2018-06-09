namespace FplBot.Api.Team
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FplTeam
    {
        [JsonProperty("chips", NullValueHandling = NullValueHandling.Ignore)]
        public List<Chip> Chips { get; set; }

        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public Entry Entry { get; set; }

        [JsonProperty("leagues", NullValueHandling = NullValueHandling.Ignore)]
        public Leagues Leagues { get; set; }

        [JsonProperty("season", NullValueHandling = NullValueHandling.Ignore)]
        public List<Season> Season { get; set; }

        [JsonProperty("history", NullValueHandling = NullValueHandling.Ignore)]
        public List<History> History { get; set; }
    }

    public partial class Chip
    {
        [JsonProperty("played_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayedTimeFormatted { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Time { get; set; }

        [JsonProperty("chip", NullValueHandling = NullValueHandling.Ignore)]
        public long? ChipChip { get; set; }

        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }

    public partial class Entry
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("player_first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerFirstName { get; set; }

        [JsonProperty("player_last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerLastName { get; set; }

        [JsonProperty("player_region_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? PlayerRegionId { get; set; }

        [JsonProperty("player_region_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerRegionName { get; set; }

        [JsonProperty("player_region_short_iso", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerRegionShortIso { get; set; }

        [JsonProperty("summary_overall_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? SummaryOverallPoints { get; set; }

        [JsonProperty("summary_overall_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? SummaryOverallRank { get; set; }

        [JsonProperty("summary_event_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? SummaryEventPoints { get; set; }

        [JsonProperty("summary_event_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? SummaryEventRank { get; set; }

        [JsonProperty("joined_seconds", NullValueHandling = NullValueHandling.Ignore)]
        public long? JoinedSeconds { get; set; }

        [JsonProperty("current_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? CurrentEvent { get; set; }

        [JsonProperty("total_transfers", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalTransfers { get; set; }

        [JsonProperty("total_loans", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalLoans { get; set; }

        [JsonProperty("total_loans_active", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalLoansActive { get; set; }

        [JsonProperty("transfers_or_loans", NullValueHandling = NullValueHandling.Ignore)]
        public string TransfersOrLoans { get; set; }

        [JsonProperty("deleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Deleted { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Email { get; set; }

        [JsonProperty("joined_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? JoinedTime { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("bank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bank { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value { get; set; }

        [JsonProperty("kit", NullValueHandling = NullValueHandling.Ignore)]
        public string Kit { get; set; }

        [JsonProperty("event_transfers", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfers { get; set; }

        [JsonProperty("event_transfers_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTransfersCost { get; set; }

        [JsonProperty("extra_free_transfers", NullValueHandling = NullValueHandling.Ignore)]
        public long? ExtraFreeTransfers { get; set; }

        [JsonProperty("strategy")]
        public object Strategy { get; set; }

        [JsonProperty("favourite_team", NullValueHandling = NullValueHandling.Ignore)]
        public long? FavouriteTeam { get; set; }

        [JsonProperty("started_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartedEvent { get; set; }

        [JsonProperty("player", NullValueHandling = NullValueHandling.Ignore)]
        public long? Player { get; set; }
    }

    public partial class History
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

    public partial class Leagues
    {
        [JsonProperty("cup", NullValueHandling = NullValueHandling.Ignore)]
        public List<Cup> Cup { get; set; }

        [JsonProperty("h2h", NullValueHandling = NullValueHandling.Ignore)]
        public List<Classic> H2H { get; set; }

        [JsonProperty("classic", NullValueHandling = NullValueHandling.Ignore)]
        public List<Classic> Classic { get; set; }
    }

    public partial class Classic
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("entry_rank")]
        public long? EntryRank { get; set; }

        [JsonProperty("entry_last_rank")]
        public long? EntryLastRank { get; set; }

        [JsonProperty("entry_movement")]
        public string EntryMovement { get; set; }

        [JsonProperty("entry_change")]
        public long? EntryChange { get; set; }

        [JsonProperty("entry_can_leave", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EntryCanLeave { get; set; }

        [JsonProperty("entry_can_admin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EntryCanAdmin { get; set; }

        [JsonProperty("entry_can_invite", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EntryCanInvite { get; set; }

        [JsonProperty("entry_can_forum", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EntryCanForum { get; set; }

        [JsonProperty("entry_code")]
        public object EntryCode { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

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

        [JsonProperty("admin_entry")]
        public long? AdminEntry { get; set; }

        [JsonProperty("start_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartEvent { get; set; }

        [JsonProperty("is_cup", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCup { get; set; }

        [JsonProperty("ko_rounds", NullValueHandling = NullValueHandling.Ignore)]
        public long? KoRounds { get; set; }
    }

    public partial class Cup
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("entry_1_entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Entry { get; set; }

        [JsonProperty("entry_1_name", NullValueHandling = NullValueHandling.Ignore)]
        public string Entry1_Name { get; set; }

        [JsonProperty("entry_1_player_name", NullValueHandling = NullValueHandling.Ignore)]
        public string Entry1_PlayerName { get; set; }

        [JsonProperty("entry_2_entry", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Entry { get; set; }

        [JsonProperty("entry_2_name", NullValueHandling = NullValueHandling.Ignore)]
        public string Entry2_Name { get; set; }

        [JsonProperty("entry_2_player_name", NullValueHandling = NullValueHandling.Ignore)]
        public string Entry2_PlayerName { get; set; }

        [JsonProperty("is_knockout", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsKnockout { get; set; }

        [JsonProperty("winner", NullValueHandling = NullValueHandling.Ignore)]
        public long? Winner { get; set; }

        [JsonProperty("tiebreak")]
        public object Tiebreak { get; set; }

        [JsonProperty("own_entry", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OwnEntry { get; set; }

        [JsonProperty("entry_1_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Points { get; set; }

        [JsonProperty("entry_1_win", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Win { get; set; }

        [JsonProperty("entry_1_draw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Draw { get; set; }

        [JsonProperty("entry_1_loss", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Loss { get; set; }

        [JsonProperty("entry_2_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Points { get; set; }

        [JsonProperty("entry_2_win", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Win { get; set; }

        [JsonProperty("entry_2_draw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Draw { get; set; }

        [JsonProperty("entry_2_loss", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Loss { get; set; }

        [JsonProperty("entry_1_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry1_Total { get; set; }

        [JsonProperty("entry_2_total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Entry2_Total { get; set; }

        [JsonProperty("seed_value")]
        public object SeedValue { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public long? Event { get; set; }
    }

    public partial class Season
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("season_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SeasonName { get; set; }

        [JsonProperty("total_points", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPoints { get; set; }

        [JsonProperty("rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? Rank { get; set; }

        [JsonProperty("season", NullValueHandling = NullValueHandling.Ignore)]
        public long? SeasonSeason { get; set; }

        [JsonProperty("player", NullValueHandling = NullValueHandling.Ignore)]
        public long? Player { get; set; }
    }
}
