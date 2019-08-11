namespace FplBot.Data
{
    internal struct CaptainChoice
    {
        internal long TeamEntryId { get; set; }

        internal long CaptainId { get; set; }

        internal long CaptainMultiplier { get; set; }

        internal long CaptainEventPoints { get; set; }

        internal bool CaptainPlayed { get; set; }

        internal long ViceId { get; set; }

        internal long ViceMultiplier { get; set; }

        internal long ViceEventPoints { get; set; }

        internal bool VicePlayed { get; set; }

        internal bool BothBlanked => !(this.CaptainPlayed || this.VicePlayed);

        internal long ActivePlayerId
        {
            get
            {
                if (this.CaptainPlayed)
                {
                    return this.CaptainId;
                }
                else if (this.VicePlayed)
                {
                    return this.ViceId;
                }
                else
                {
                    return -1;
                }
            }
        }

        internal long EventScore => this.CaptainPlayed ? this.CaptainEventPoints : this.ViceEventPoints;

        internal long EventScoreMultiplied => this.CaptainPlayed ? this.CaptainEventPoints * this.CaptainMultiplier : this.ViceEventPoints * this.ViceMultiplier;

    }
}
