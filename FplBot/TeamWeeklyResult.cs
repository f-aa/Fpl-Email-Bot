namespace FplBot
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
    }
}
