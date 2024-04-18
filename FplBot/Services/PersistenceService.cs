using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using FplBot.Logging;
using System.Text;

namespace FplBot.Services
{
    /// <summary>
    /// Handles reading and writing the current gameweek to a persistent storage
    /// </summary>
    /// <remarks>
    /// Instantiates a new instance of the Persistence class
    /// </remarks>
    public class PersistenceService(BlobContainerClient client, ILogger logger) : IPersistenceService
    {
        // TODO: should probably load these from appsettings
        private readonly string gameweekPath = "gameweek.txt";
        private readonly string standingsPath = "standings.txt";
        private readonly string weeklyWinsPath = "weeklyWins.txt";

        private readonly BlobContainerClient client = client;
        private readonly ILogger logger = logger;

        private int gameweek = -1;

        /// <summary>
        /// Gets the current gameweek from persistent storage
        /// </summary>
        /// <returns>The currently active gameweek retriveed from persistent storage</returns>
        public async Task<int> GetEventIdAsync()
        {
            if (gameweek < 0)
            {
                logger.Log("Loading current gameweek Id from persistent storage");

                await client
                    .GetBlockBlobClient(gameweekPath)
                    .DownloadToAsync(gameweekPath);

                gameweek = int.Parse(File.ReadAllText(gameweekPath));
            }

            return gameweek;
        }

        /// <summary>
        /// Increment the gameweek number and store it back to persistent storage
        /// </summary>
        public async Task IncrementGameweekAsync()
        {
            logger.Log("Incrementing gameweek in persistent storage");

            File.WriteAllText(gameweekPath, (++gameweek).ToString());

            await client
                .GetBlockBlobClient(gameweekPath)
                .UploadAsync(File.OpenRead(gameweekPath));
        }

        /// <summary>
        /// Saves the current standings to persistent storage
        /// </summary>
        /// <param name="result">A StringBuilder object representing the current standings</param>
        public void SaveStandings(StringBuilder result)
        {
            logger.Log("Saving standings to disk");

            if (result != null)
            {
                File.WriteAllText(standingsPath, result.ToString());
            }
        }

        /// <summary>
        /// Saves the current weekly wins to persistent storage
        /// </summary>
        /// <param name="result"></param>
        public void SaveWeeklyWins(StringBuilder result)
        {
            logger.Log("Saving weekly wins to disk");

            if (result != null)
            {
                File.WriteAllText(weeklyWinsPath, result.ToString());
            }
        }
    }
}
