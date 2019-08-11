using System;

namespace FplBot.Logging
{
    /// <summary>
    /// A basic logging class
    /// </summary>
    /// <remarks>This is a thing wrapper we can swap in for a full featured logger sometime in the future</remarks>
    public class Logger : ILogger
    {
        /// <summary>
        /// Thin wrapper around Console.WriteLine()
        /// </summary>
        /// <param name="message">The message we want to log</param>
        public void Log(string message, bool breakLine = true)
        {
            if (breakLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            
        }
    }
}
