using Microsoft.Azure.WebJobs;
using System;

namespace FplBot
{
    class Program
    {
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            while (true)
            {
                FplBot bot = new FplBot();

                bot.Initialize().ConfigureAwait(false).GetAwaiter().GetResult();    // Get preliminary stuff setup
                bot.Process().ConfigureAwait(false).GetAwaiter().GetResult();       // Do the processing

                Console.WriteLine(bot.Output.ToString());       // Print result to console

                bot.Wait();                                     // Wait for next time
            }
        }
    }
}
