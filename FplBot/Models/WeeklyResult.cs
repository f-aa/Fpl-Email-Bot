namespace FplBot.Models
{
    public struct WeeklyResult
    {
        internal long Id { get; set; }

        /// <summary>
        /// Name of the team
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The overall rank of all players
        /// </summary>
        internal long OverallRank { get; set; }

        /// <summary>
        /// Which chip was used, if any
        /// </summary>
        internal string ChipUsed { get; set; }

        /// <summary>
        /// Score before points hits
        /// </summary>
        internal long ScoreBeforeHits { get; set; }

        /// <summary>
        /// The total cost of transfer hits taken
        /// </summary>
        internal long HitsTakenCost { get; set; }

        /// <summary>
        /// Score after points hits
        /// </summary>
        internal long Points => ScoreBeforeHits - HitsTakenCost;

        /// <summary>
        /// Total amount of points
        /// </summary>
        internal long TotalPoints { get; set; }

        /// <summary>
        /// Total transfers made during season
        /// </summary>
        internal long TotalTransfers { get; set; }

        /// <summary>
        /// Total team value
        /// </summary>
        internal float TeamValue { get; set; }

        /// <summary>
        /// Points for this game week
        /// </summary>
        internal long GameWeekPoints { get; set; }

        /// <summary>
        /// The position in the mini-league this team reached this week
        /// </summary>
        internal int CurrentWeekPosition { get; set; }

        /// <summary>
        /// The position in the mini-league this team was last week
        /// </summary>
        internal int PreviousWeekPosition { get; set; }

        /// <summary>
        /// The difference between last week and this week
        /// </summary>
        internal long PositionChangedSinceLastWeek => PreviousWeekPosition - CurrentWeekPosition;
    }
}
