namespace FplBot.Services
{
    public interface IEmailService
    {
        Task SendEmail(string output, string eventName);
    }
}