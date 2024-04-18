using System.Text;

namespace FplBot.Services
{
    public interface IPersistenceService
    {
        Task IncrementGameweekAsync();

        Task<int> GetEventIdAsync();

        void SaveStandings(StringBuilder result);

        void SaveWeeklyWins(StringBuilder result);
    }
}
