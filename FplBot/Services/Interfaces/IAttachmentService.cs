using System.Text;

namespace FplBot.Services
{
    public interface IAttachmentService
    {
        Task<StringBuilder> GenerateStandingsTable();
        Task<StringBuilder> GenerateTotalWeeklyWins();
    }
}