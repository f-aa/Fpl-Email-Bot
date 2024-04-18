using FplBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FplBot
{
    public class FplJob
    {
        private readonly IFplService fplService;
        private readonly IOutputService outputService;
        private readonly IPersistenceService persistenceService;
        private readonly IEmailService emailService;
        private readonly Logging.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the Fpl class
        /// </summary>
        /// <param name="gameweekToProcess"></param>
        public FplJob(
            IFplService fplService,
            IOutputService outputService,
            IPersistenceService persistenceService,
            IEmailService emailService,
            Logging.ILogger logger)
        {
            this.fplService = fplService;
            this.outputService = outputService;
            this.persistenceService = persistenceService;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task Run([TimerTrigger("%CronSettings%", RunOnStartup = true)] TimerInfo timerInfo, ILogger logger)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            var currentEvent = await this.fplService.GetCurrentEventAsync();

            if (currentEvent?.Finished ?? false && currentEvent.DataChecked.Value)   // Make sure gameweek is finished
            {
                var fplLeague = await this.fplService.GetLeagueAsync();
                var fplTeams = await this.fplService.GetTeamsAsync();

                if (fplTeams.Count == 0)
                {
                    this.logger.Log($"Could not find any teams in league {fplLeague.League.Name}. Did you enter the correct ID? Terminating application.");
                    Environment.Exit(-1);
                }

                var output = await this.outputService.BuildOutput();
                await this.emailService.SendEmail(output, currentEvent.Name);
                await this.persistenceService.IncrementGameweekAsync();
            }
            else
            {
                this.logger.Log($"Gameweek has not completed yet.");
            }

            stopwatch.Stop();
            this.logger.Log($"Beep boop. This is diagnostics. Completed FPL processing in {(double)stopwatch.ElapsedMilliseconds / 1000:N1} seconds.");
        }
    }
}
