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
        private const string BLOB_STORAGE_NAME = "oskana";          // The name of the azure blob container
        private const string GAMEWEEK_FILENAME = "gameweek.txt";    // The name of the textfile to persist gameweek
        private const string STANDINGS_FILENAME = "standings.txt";  // The name of the textfile to persist weekly total standings
        private readonly bool useAzure = false;
        private readonly string basePath;
        private readonly string gameweekPath;
        private readonly string standingsPath;

        private CloudBlockBlob gameweekBlob;
        private CloudBlockBlob standingsBlob;
        private int gameweek;

        /// <summary>
        /// Instantiates a new instance of the Persistence class
        /// </summary>
        internal Persistence(bool useAzure)
        {
            this.useAzure = useAzure;
            this.basePath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            this.gameweekPath = $"{basePath}\\{GAMEWEEK_FILENAME}";
            this.standingsPath = $"{basePath}\\{STANDINGS_FILENAME}";
            this.gameweek = 1; // default
        }

        /// <summary>
        /// Initialize the class
        /// </summary>
        internal void Initialize()
        {
            if (useAzure)
            {
                string connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(ConnectionStringNames.Storage);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(BLOB_STORAGE_NAME);
                container.CreateIfNotExists();

                this.gameweekBlob = container.GetBlockBlobReference(GAMEWEEK_FILENAME);
                this.standingsBlob = container.GetBlockBlobReference(STANDINGS_FILENAME);

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
                if(!File.Exists(this.gameweekPath))
                {
                    File.WriteAllText(this.gameweekPath, this.gameweek.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the current gameweek from persistent storage
        /// </summary>
        /// <returns></returns>
        internal int GetGameweek()
        {
            this.gameweek = this.useAzure ? int.Parse(gameweekBlob.DownloadText()) : int.Parse(File.ReadAllText(this.gameweekPath));
            return this.gameweek;
        }

        /// <summary>
        /// Increment the gameweek number and store it back to persistent storage
        /// </summary>
        internal void CompleteGameweek()
        {
            this.gameweek++;

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
        /// <param name="result">A Strinbuilder object representing the current standings</param>
        internal void SaveStandings(StringBuilder result)
        {
            if (result == null) return;

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
