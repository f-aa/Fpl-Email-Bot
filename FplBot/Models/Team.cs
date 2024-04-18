using FplBot.Api.Picks;
using FplBot.Api.Team;

namespace FplBot.Models
{
    public class Team
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<ApiTeamEvents> Current { get; set; }

        public List<ApiTeamPreviousSeasons> Past { get; set; }

        public List<ApiChip> Chips { get; set; }

        public int TotalWeeklySingleWins { get; set; }

        public int TotalWeeklySharedWins { get; set; }

        public int FoiledByDanDaviesRule { get; set; }

        public float TotalWeeklyWins { get; set; }

        public ApiSquad? Squad { get; internal set; }

        public List<string> WinWeeks = new List<string>();
    }
}
