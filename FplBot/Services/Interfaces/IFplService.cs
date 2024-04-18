
using FplBot.Api.League;
using FplBot.Api.Player;
using FplBot.Api.Root;
using FplBot.Api.Summary;
using FplBot.Models;

namespace FplBot.Services
{
    public interface IFplService
    {
        Task<Event> GetCurrentEventAsync();

        Dictionary<long, ApiPlayerDetail> GetSoccerPlayers();

        Task<ApiLeague> GetLeagueAsync();

        Task<Dictionary<long, Team>> GetTeamsAsync();

        Task<Dictionary<long, ApiPlayerSummary>> GetPlayersAsync();

        Task<IOrderedEnumerable<WeeklyResult>> GetWeeklyResult();

        Task<IOrderedEnumerable<KeyValuePair<long, Team>>> GetLastWeekStandings();

        Task<IOrderedEnumerable<KeyValuePair<long, Team>>> GetCurrentWeekStandings();

    }
}