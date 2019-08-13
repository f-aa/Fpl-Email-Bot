using Flurl.Http;
using FplBot.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FplBot.Api
{
    internal class Authentication
    {
        private readonly Uri FplAuthUri = new Uri("https://users.premierleague.com");
        private const string USERNAME = ""; // TODO: temp, move to app.config
        private const string PASSWORD = ""; // TODO: temp, move to app.config

        private readonly ILogger logger;

        private Cookie authCookie;

        internal Authentication(ILogger logger)
        {
            this.logger = logger;
        }

        internal async Task<Cookie> GetCookie()
        {
            if (this.authCookie != null && !this.authCookie.Expired)
            {
                return this.authCookie;
            }

            using (var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }) { BaseAddress = this.FplAuthUri })
            using (var flurlClient = new FlurlClient(httpClient).EnableCookies())
            {
                this.logger.Log("Aquiring authentication cookie...");

                var response = await flurlClient
                    .Request("/accounts/login/")
                    .AllowHttpStatus(HttpStatusCode.Redirect)
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .PostStringAsync($"login={USERNAME}&password={PASSWORD}&app=plfpl-web&redirect_uri=https://fantasy.premierleague.com/")
                    .ConfigureAwait(false);

                this.authCookie = flurlClient.Cookies.Last().Value;
            }

            return this.authCookie;
        }
    }
}
