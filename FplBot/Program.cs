using FplBot.Logging;
using Microsoft.Azure.WebJobs;

namespace FplBot
{
    class Program
    {
        static void Main()
        {
            var logger = new Logger();
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            while (true)
            {
                FplBot bot = new FplBot(logger);

                bot.Initialize().ConfigureAwait(false).GetAwaiter().GetResult();    // Get preliminary stuff setup
                bot.Process().ConfigureAwait(false).GetAwaiter().GetResult();       // Do the processing

                logger.Log(bot.Output.ToString());                                  // Print result to console

                bot.Wait();                                                         // Wait
            }
        }
    }
}
