namespace FplBot.Models
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

        internal bool BothBlanked => !(CaptainPlayed || VicePlayed);

        internal long ActivePlayerId
        {
            get
            {
                if (CaptainPlayed)
                {
                    return CaptainId;
                }
                else if (VicePlayed)
                {
                    return ViceId;
                }
                else
                {
                    return -1;
                }
            }
        }

        internal long EventScore => CaptainPlayed ? CaptainEventPoints : ViceEventPoints;

        internal long EventScoreMultiplied => CaptainPlayed ? CaptainEventPoints * CaptainMultiplier : ViceEventPoints * ViceMultiplier;

    }
}
