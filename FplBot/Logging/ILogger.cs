namespace FplBot.Logging
{
    public interface ILogger
    {
        void Log(string message, bool breakLine = true);
    }
}