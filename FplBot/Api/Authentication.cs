using Flurl.Http;
using FplBot.Logging;
using System.Net;

namespace FplBot.Api
{
    [Obsolete("FPL no longer requires authentication. Leaving this behind in case they change their mind.")]
    internal class Authentication
    {
        private readonly Uri FplAuthUri = new Uri("https://users.premierleague.com");

        private readonly ILogger logger;
        private readonly string username;
        private readonly string password;

        private Cookie authCookie;

        internal Authentication(string fplUsername, string fplPassword, ILogger logger)
        {
            if (string.IsNullOrEmpty(fplUsername)) throw new ArgumentNullException(nameof(fplUsername));
            if (string.IsNullOrEmpty(fplPassword)) throw new ArgumentNullException(nameof(fplPassword));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.username = fplUsername;
            this.password = fplPassword;

        }

        // This doesn't work with the new Flurl client, but keeping it around in case FPL turns authentication requirements back on
        internal async Task<Cookie> GetCookie()
        {
            if (this.authCookie != null && !this.authCookie.Expired)
            {
                return this.authCookie;
            }

            using (var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }) { BaseAddress = this.FplAuthUri })
            using (var flurlClient = new FlurlClient(httpClient).EnableCookies())
            {
                this.logger.Log("No auth cookie found! Fetching a new one...");

                var response = await flurlClient
                    .Request("/accounts/login/")
                    .AllowHttpStatus("302")
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .PostStringAsync($"login={this.username}&password={this.password}&app=plfpl-web&redirect_uri=https://fantasy.premierleague.com/")
                    .ConfigureAwait(false);

                if (flurlClient.Cookies.Count == 0)
                {
                    this.logger.Log($"Couldn't fetch auth cookie for {this.username}. Please make sure your username and password is correct.");
                    throw new Exception("Failed to get auth cookie.");
                }

                this.authCookie = flurlClient.Cookies.Last().Value;
            }

            return this.authCookie;
        }
    }
}
