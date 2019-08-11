using FplBot.Api.Team;
using System.Collections.Generic;

namespace FplBot.Data
{
    internal struct FplTeamEntry
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<ApiFplTeamEvents> Current { get; set; }

        public List<ApiFplTeamPreviousSeasons> Past { get; set; }

        public List<ApiFplTeamChip> Chips { get; set; }
    }
}
