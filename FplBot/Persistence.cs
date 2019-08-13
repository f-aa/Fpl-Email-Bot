using FplBot.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Text;

namespace FplBot
{
    /// <summary>
    /// Handles reading and writing the current gameweek to a persistent storage
    /// </summary>
    internal class Persistence
    {
        private const string GAMEWEEK_FILENAME = "gameweek.txt";    // The name of the textfile to persist gameweek
        private const string STANDINGS_FILENAME = "standings.txt";  // The name of the textfile to persist weekly total standings

        private readonly ILogger logger;
        private readonly bool useAzure = false;
        private readonly string basePath;
        private readonly string gameweekPath;
        private readonly string standingsPath;
        private readonly string blobStorageName;

        private CloudBlockBlob gameweekBlob;
        private CloudBlockBlob standingsBlob;
        private int gameweek;

        /// <summary>
        /// Instantiates a new instance of the Persistence class
        /// </summary>
        /// <param name="useAzure">Whether to use Azure Blob Storage to persist gameweek data</param>
        /// <param name="blobStorageName">The name of the blob storage we want to save gameweek data to</param>
        internal Persistence(ILogger logger, bool useAzure = false, string blobStorageName = null)
        {
            this.logger = logger;
            this.useAzure = useAzure;
            this.blobStorageName = blobStorageName;
            this.basePath = AppDomain.CurrentDomain.BaseDirectory;
            this.gameweekPath = $"{basePath}\\{GAMEWEEK_FILENAME}";
            this.standingsPath = $"{basePath}\\{STANDINGS_FILENAME}";
            this.gameweek = 1; // default

            if (useAzure && string.IsNullOrWhiteSpace(blobStorageName))
            {
                throw new ArgumentException("If webjob is configured to use Azure, a blob storage name must be specified!");
            }
        }

        /// <summary>
        /// Initialize the class
        /// </summary>
        internal void Initialize()
        {
            this.logger.Log("Initializing persistent storage");

            if (useAzure)
            {
                string connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.Storage);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(this.blobStorageName);
                container.CreateIfNotExists();

                this.gameweekBlob = container.GetBlockBlobReference(GAMEWEEK_FILENAME);
                this.standingsBlob = container.GetBlockBlobReference(STANDINGS_FILENAME);

                this.logger.Log($"Attempting to store gameweek data to: {container.Uri}");

                if (!gameweekBlob.Exists())
                {
                    this.gameweekBlob.UploadText(this.gameweek.ToString());  // Create a file if it doesn't exist already
                }

                if (!standingsBlob.Exists())
                {
                    this.standingsBlob.UploadText(string.Empty);  // Create a file if it doesn't exist already
                }
            }
            else
            {
                this.logger.Log($"Attempting to store gameweek data to: {this.basePath}");

                if (!File.Exists(this.gameweekPath))
                {
                    File.WriteAllText(this.gameweekPath, this.gameweek.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the current gameweek from persistent storage
        /// </summary>
        /// <returns>The currently active gameweek retriveed from persistent storage</returns>
        internal int GetGameweek()
        {
            this.logger.Log("Loading current gameweek Id from persistent storage");

            this.gameweek = this.useAzure ? int.Parse(gameweekBlob.DownloadText()) : int.Parse(File.ReadAllText(this.gameweekPath));
            return this.gameweek;
        }

        /// <summary>
        /// Increment the gameweek number and store it back to persistent storage
        /// </summary>
        internal void CompleteGameweek()
        {
            this.gameweek++;

            this.logger.Log("Incrementing gameweek in persistent storage");

            if (this.useAzure)
            {
                this.gameweekBlob.UploadText(gameweek.ToString());
            }
            else
            {
                File.WriteAllText(this.gameweekPath, gameweek.ToString());
            }
        }

        /// <summary>
        /// Gets the current standings as a stream
        /// </summary>
        /// <returns>A stream with the current standings</returns>
        internal Stream GetStandingsStream()
        {
            this.logger.Log("Loading weekly standings from persistent storage");

            if (this.useAzure)
            {
                Stream stream = new MemoryStream();
                this.standingsBlob.DownloadToStream(stream);
                return stream;
            }
            else
            {
                return File.OpenRead(this.standingsPath);
            }
        }

        /// <summary>
        /// Saves the current standings to persistent storage
        /// </summary>
        /// <param name="result">A StringBuilder object representing the current standings</param>
        internal void SaveStandings(StringBuilder result)
        {
            if (result == null) return;

            this.logger.Log("Saving weekly standings to persistent storage");

            if (this.useAzure)
            {
                this.standingsBlob.UploadText(result.ToString());
            }
            else
            {
                File.WriteAllText(this.standingsPath, result.ToString());
            }
        }
    }
}
