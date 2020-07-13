using FplBot.Api.Team;
using System.Collections.Generic;

namespace FplBot.Data
{
    internal class FplTeamEntry
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<ApiFplTeamEvents> Current { get; set; }

        public List<ApiFplTeamPreviousSeasons> Past { get; set; }

        public List<ApiFplTeamChip> Chips { get; set; }

        public int TotalWeeklySingleWins { get; set; }

        public int TotalWeeklySharedWins { get; set; }

        public int FoiledByDanDaviesRule { get; set; }

        public float TotalWeeklyWins { get; set; }

        public List<string> WinWeeks = new List<string>();
    }
}
