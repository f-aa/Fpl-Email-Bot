namespace FplBot.Data
{
    internal struct TeamWeeklyResult
    {
        /// <summary>
        /// Name of the team
        /// </summary>
        internal string Name { get; set; }

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
        internal long Points => this.ScoreBeforeHits - this.HitsTakenCost;

        /// <summary>
        /// Total amount of points
        /// </summary>
        internal long TotalPoints { get; set; }

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
        internal long PositionChangedSinceLastWeek => this.PreviousWeekPosition - this.CurrentWeekPosition;
    }
}
