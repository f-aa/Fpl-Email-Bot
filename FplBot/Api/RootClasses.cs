namespace FplBot.Api.Root
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class FplRoot
    {
        [JsonProperty("elements", NullValueHandling = NullValueHandling.Ignore)]
        public List<Player.ApiPlayerDetail> Elements { get; set; }

        [JsonProperty("total-players", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalPlayers { get; set; }

        [JsonProperty("player")]
        public object Player { get; set; }

        [JsonProperty("element_types", NullValueHandling = NullValueHandling.Ignore)]
        public List<ElementTypeElement> ElementTypes { get; set; }

        [JsonProperty("watched", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Watched { get; set; }

        [JsonProperty("next-event", NullValueHandling = NullValueHandling.Ignore)]
        public long? NextEvent { get; set; }

        [JsonProperty("phases", NullValueHandling = NullValueHandling.Ignore)]
        public List<Phase> Phases { get; set; }

        [JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
        public Stats Stats { get; set; }

        [JsonProperty("game-settings", NullValueHandling = NullValueHandling.Ignore)]
        public GameSettings GameSettings { get; set; }

        [JsonProperty("current-event", NullValueHandling = NullValueHandling.Ignore)]
        public long? CurrentEvent { get; set; }

        [JsonProperty("teams", NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiTeam> Teams { get; set; }

        [JsonProperty("stats_options", NullValueHandling = NullValueHandling.Ignore)]
        public List<StatsOption> StatsOptions { get; set; }

        [JsonProperty("last-entry-event", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastEntryEvent { get; set; }

        [JsonProperty("entry")]
        public object Entry { get; set; }

        [JsonProperty("next_event_fixtures", NullValueHandling = NullValueHandling.Ignore)]
        public List<NextEventFixture> NextEventFixtures { get; set; }

        [JsonProperty("events", NullValueHandling = NullValueHandling.Ignore)]
        public List<Event> Events { get; set; }
    }

    public partial class ElementTypeElement
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("singular_name", NullValueHandling = NullValueHandling.Ignore)]
        public string SingularName { get; set; }

        [JsonProperty("singular_name_short", NullValueHandling = NullValueHandling.Ignore)]
        public string SingularNameShort { get; set; }

        [JsonProperty("plural_name", NullValueHandling = NullValueHandling.Ignore)]
        public string PluralName { get; set; }

        [JsonProperty("plural_name_short", NullValueHandling = NullValueHandling.Ignore)]
        public string PluralNameShort { get; set; }
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

        [JsonProperty("highest_scoring_entry")]
        public long? HighestScoringEntry { get; set; }

        [JsonProperty("deadline_time_epoch", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeadlineTimeEpoch { get; set; }

        [JsonProperty("deadline_time_game_offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? DeadlineTimeGameOffset { get; set; }

        [JsonProperty("deadline_time_formatted", NullValueHandling = NullValueHandling.Ignore)]
        public string DeadlineTimeFormatted { get; set; }

        [JsonProperty("highest_score")]
        public long? HighestScore { get; set; }

        [JsonProperty("is_previous", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPrevious { get; set; }

        [JsonProperty("is_current", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCurrent { get; set; }

        [JsonProperty("is_next", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNext { get; set; }
    }

    public partial class GameSettings
    {
        [JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public Game Game { get; set; }

        [JsonProperty("element_type", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ElementTypeValue> ElementType { get; set; }
    }

    public partial class ElementTypeValue
    {
        [JsonProperty("scoring_clean_sheets", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringCleanSheets { get; set; }

        [JsonProperty("squad_min_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadMinPlay { get; set; }

        [JsonProperty("bps_clean_sheets", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsCleanSheets { get; set; }

        [JsonProperty("scoring_goals_conceded", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringGoalsConceded { get; set; }

        [JsonProperty("scoring_goals_scored", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringGoalsScored { get; set; }

        [JsonProperty("squad_max_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadMaxPlay { get; set; }

        [JsonProperty("bps_goals_scored", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsGoalsScored { get; set; }

        [JsonProperty("ui_shirt_specific", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UiShirtSpecific { get; set; }

        [JsonProperty("squad_select", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadSelect { get; set; }

        [JsonProperty("sub_positions_locked", NullValueHandling = NullValueHandling.Ignore)]
        public List<long> SubPositionsLocked { get; set; }
    }

    public partial class Game
    {
        [JsonProperty("scoring_ea_index", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringEaIndex { get; set; }

        [JsonProperty("league_prefix_public", NullValueHandling = NullValueHandling.Ignore)]
        public string LeaguePrefixPublic { get; set; }

        [JsonProperty("bps_tackles", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsTackles { get; set; }

        [JsonProperty("league_h2h_tiebreak", NullValueHandling = NullValueHandling.Ignore)]
        public string LeagueH2HTiebreak { get; set; }

        [JsonProperty("scoring_long_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringLongPlay { get; set; }

        [JsonProperty("bps_recoveries_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsRecoveriesLimit { get; set; }

        [JsonProperty("facebook_app_id", NullValueHandling = NullValueHandling.Ignore)]
        public string FacebookAppId { get; set; }

        [JsonProperty("bps_tackled", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsTackled { get; set; }

        [JsonProperty("bps_errors_leading_to_goal", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsErrorsLeadingToGoal { get; set; }

        [JsonProperty("bps_yellow_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsYellowCards { get; set; }

        [JsonProperty("ui_el_hide_currency_qi", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UiElHideCurrencyQi { get; set; }

        [JsonProperty("scoring_bonus", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringBonus { get; set; }

        [JsonProperty("transfers_cost", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersCost { get; set; }

        [JsonProperty("default_formation", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<long>> DefaultFormation { get; set; }

        [JsonProperty("bps_long_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsLongPlay { get; set; }

        [JsonProperty("bps_long_play_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsLongPlayLimit { get; set; }

        [JsonProperty("scoring_assists", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringAssists { get; set; }

        [JsonProperty("scoring_long_play_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringLongPlayLimit { get; set; }

        [JsonProperty("fifa_league_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? FifaLeagueId { get; set; }

        [JsonProperty("league_size_classic_max", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeagueSizeClassicMax { get; set; }

        [JsonProperty("scoring_red_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringRedCards { get; set; }

        [JsonProperty("scoring_creativity", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringCreativity { get; set; }

        [JsonProperty("game_timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string GameTimezone { get; set; }

        [JsonProperty("static_game_url", NullValueHandling = NullValueHandling.Ignore)]
        public string StaticGameUrl { get; set; }

        [JsonProperty("currency_symbol", NullValueHandling = NullValueHandling.Ignore)]
        public string CurrencySymbol { get; set; }

        [JsonProperty("bps_target_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsTargetMissed { get; set; }

        [JsonProperty("bps_penalties_saved", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPenaltiesSaved { get; set; }

        [JsonProperty("support_email_address", NullValueHandling = NullValueHandling.Ignore)]
        public string SupportEmailAddress { get; set; }

        [JsonProperty("cup_start_event_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? CupStartEventId { get; set; }

        [JsonProperty("scoring_penalties_saved", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringPenaltiesSaved { get; set; }

        [JsonProperty("scoring_threat", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringThreat { get; set; }

        [JsonProperty("scoring_saves", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringSaves { get; set; }

        [JsonProperty("league_join_private_max", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeagueJoinPrivateMax { get; set; }

        [JsonProperty("scoring_short_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringShortPlay { get; set; }

        [JsonProperty("sys_use_event_live_api", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SysUseEventLiveApi { get; set; }

        [JsonProperty("scoring_concede_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringConcedeLimit { get; set; }

        [JsonProperty("bps_key_passes", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsKeyPasses { get; set; }

        [JsonProperty("bps_clearances_blocks_interceptions", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsClearancesBlocksInterceptions { get; set; }

        [JsonProperty("bps_pass_percentage_90", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPassPercentage90 { get; set; }

        [JsonProperty("bps_big_chances_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsBigChancesMissed { get; set; }

        [JsonProperty("league_max_ko_rounds_h2h", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeagueMaxKoRoundsH2H { get; set; }

        [JsonProperty("bps_open_play_crosses", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsOpenPlayCrosses { get; set; }

        [JsonProperty("league_points_h2h_win", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeaguePointsH2HWin { get; set; }

        [JsonProperty("bps_saves", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsSaves { get; set; }

        [JsonProperty("bps_cbi_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsCbiLimit { get; set; }

        [JsonProperty("league_size_h2h_max", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeagueSizeH2HMax { get; set; }

        [JsonProperty("sys_vice_captain_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SysViceCaptainEnabled { get; set; }

        [JsonProperty("squad_squadplay", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadSquadplay { get; set; }

        [JsonProperty("bps_fouls", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsFouls { get; set; }

        [JsonProperty("squad_squadsize", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadSquadsize { get; set; }

        [JsonProperty("ui_selection_short_team_names", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UiSelectionShortTeamNames { get; set; }

        [JsonProperty("transfers_sell_on_fee", NullValueHandling = NullValueHandling.Ignore)]
        public double? TransfersSellOnFee { get; set; }

        [JsonProperty("transfers_type", NullValueHandling = NullValueHandling.Ignore)]
        public string TransfersType { get; set; }

        [JsonProperty("scoring_ict_index", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringIctIndex { get; set; }

        [JsonProperty("bps_pass_percentage_80", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPassPercentage80 { get; set; }

        [JsonProperty("bps_own_goals", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsOwnGoals { get; set; }

        [JsonProperty("scoring_yellow_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringYellowCards { get; set; }

        [JsonProperty("bps_pass_percentage_70", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPassPercentage70 { get; set; }

        [JsonProperty("ui_show_home_away", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UiShowHomeAway { get; set; }

        [JsonProperty("ui_el_hide_currency_sy", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UiElHideCurrencySy { get; set; }

        [JsonProperty("bps_assists", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsAssists { get; set; }

        [JsonProperty("squad_team_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadTeamLimit { get; set; }

        [JsonProperty("league_points_h2h_draw", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeaguePointsH2HDraw { get; set; }

        [JsonProperty("transfers_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? TransfersLimit { get; set; }

        [JsonProperty("bps_dribbles", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsDribbles { get; set; }

        [JsonProperty("bps_offside", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsOffside { get; set; }

        [JsonProperty("sys_cdn_cache_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SysCdnCacheEnabled { get; set; }

        [JsonProperty("currency_multiplier", NullValueHandling = NullValueHandling.Ignore)]
        public long? CurrencyMultiplier { get; set; }

        [JsonProperty("bps_red_cards", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsRedCards { get; set; }

        [JsonProperty("bps_winning_goals", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsWinningGoals { get; set; }

        [JsonProperty("league_join_public_max", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeagueJoinPublicMax { get; set; }

        [JsonProperty("formations", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<List<long>>> Formations { get; set; }

        [JsonProperty("league_points_h2h_lose", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeaguePointsH2HLose { get; set; }

        [JsonProperty("currency_decimal_places", NullValueHandling = NullValueHandling.Ignore)]
        public long? CurrencyDecimalPlaces { get; set; }

        [JsonProperty("bps_errors_leading_to_goal_attempt", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsErrorsLeadingToGoalAttempt { get; set; }

        [JsonProperty("ui_selection_price_gap", NullValueHandling = NullValueHandling.Ignore)]
        public long? UiSelectionPriceGap { get; set; }

        [JsonProperty("bps_big_chances_created", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsBigChancesCreated { get; set; }

        [JsonProperty("ui_selection_player_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? UiSelectionPlayerLimit { get; set; }

        [JsonProperty("bps_attempted_passes_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsAttemptedPassesLimit { get; set; }

        [JsonProperty("scoring_penalties_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringPenaltiesMissed { get; set; }

        [JsonProperty("photo_base_url", NullValueHandling = NullValueHandling.Ignore)]
        public string PhotoBaseUrl { get; set; }

        [JsonProperty("scoring_bps", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringBps { get; set; }

        [JsonProperty("scoring_influence", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringInfluence { get; set; }

        [JsonProperty("bps_penalties_conceded", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPenaltiesConceded { get; set; }

        [JsonProperty("scoring_own_goals", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringOwnGoals { get; set; }

        [JsonProperty("squad_total_spend", NullValueHandling = NullValueHandling.Ignore)]
        public long? SquadTotalSpend { get; set; }

        [JsonProperty("bps_short_play", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsShortPlay { get; set; }

        [JsonProperty("ui_element_wrap", NullValueHandling = NullValueHandling.Ignore)]
        public long? UiElementWrap { get; set; }

        [JsonProperty("bps_recoveries", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsRecoveries { get; set; }

        [JsonProperty("bps_penalties_missed", NullValueHandling = NullValueHandling.Ignore)]
        public long? BpsPenaltiesMissed { get; set; }

        [JsonProperty("scoring_saves_limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoringSavesLimit { get; set; }
    }

    public partial class NextEventFixture
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
        public List<object> Stats { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }

        [JsonProperty("kickoff_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? KickoffTime { get; set; }

        [JsonProperty("team_h_score")]
        public object TeamHScore { get; set; }

        [JsonProperty("team_a_score")]
        public object TeamAScore { get; set; }

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

    public partial class Phase
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("num_winners", NullValueHandling = NullValueHandling.Ignore)]
        public long? NumWinners { get; set; }

        [JsonProperty("start_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartEvent { get; set; }

        [JsonProperty("stop_event", NullValueHandling = NullValueHandling.Ignore)]
        public long? StopEvent { get; set; }
    }

    public partial class Stats
    {
        [JsonProperty("headings", NullValueHandling = NullValueHandling.Ignore)]
        public List<Heading> Headings { get; set; }

        [JsonProperty("categories")]
        public object Categories { get; set; }
    }

    public partial class Heading
    {
        [JsonProperty("category")]
        public object Category { get; set; }

        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        [JsonProperty("abbr")]
        public object Abbr { get; set; }

        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
    }

    public partial class StatsOption
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
    }

    public partial class ApiTeam
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("current_event_fixture", NullValueHandling = NullValueHandling.Ignore)]
        public List<TEventFixture> CurrentEventFixture { get; set; }

        [JsonProperty("next_event_fixture", NullValueHandling = NullValueHandling.Ignore)]
        public List<TEventFixture> NextEventFixture { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public long? Code { get; set; }

        [JsonProperty("short_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortName { get; set; }

        [JsonProperty("unavailable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Unavailable { get; set; }

        [JsonProperty("strength", NullValueHandling = NullValueHandling.Ignore)]
        public long? Strength { get; set; }

        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public long? Position { get; set; }

        [JsonProperty("played", NullValueHandling = NullValueHandling.Ignore)]
        public long? Played { get; set; }

        [JsonProperty("win", NullValueHandling = NullValueHandling.Ignore)]
        public long? Win { get; set; }

        [JsonProperty("loss", NullValueHandling = NullValueHandling.Ignore)]
        public long? Loss { get; set; }

        [JsonProperty("draw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Draw { get; set; }

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public long? Points { get; set; }

        [JsonProperty("form")]
        public object Form { get; set; }

        [JsonProperty("link_url", NullValueHandling = NullValueHandling.Ignore)]
        public string LinkUrl { get; set; }

        [JsonProperty("strength_overall_home", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthOverallHome { get; set; }

        [JsonProperty("strength_overall_away", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthOverallAway { get; set; }

        [JsonProperty("strength_attack_home", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthAttackHome { get; set; }

        [JsonProperty("strength_attack_away", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthAttackAway { get; set; }

        [JsonProperty("strength_defence_home", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthDefenceHome { get; set; }

        [JsonProperty("strength_defence_away", NullValueHandling = NullValueHandling.Ignore)]
        public long? StrengthDefenceAway { get; set; }

        [JsonProperty("team_division", NullValueHandling = NullValueHandling.Ignore)]
        public long? TeamDivision { get; set; }
    }

    public partial class TEventFixture
    {
        [JsonProperty("is_home", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHome { get; set; }

        [JsonProperty("day", NullValueHandling = NullValueHandling.Ignore)]
        public long? Day { get; set; }

        [JsonProperty("event_day", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventDay { get; set; }

        [JsonProperty("month", NullValueHandling = NullValueHandling.Ignore)]
        public long? Month { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("opponent", NullValueHandling = NullValueHandling.Ignore)]
        public long? Opponent { get; set; }
    }
}