using MailKit.Net.Smtp;
using Microsoft.Azure.WebJobs;
using MimeKit;
using MimeKit.Text;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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

            Stopwatch stopwatch = new Stopwatch();
            Persistence persistence = new Persistence();
            persistence.Initialize();

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();

                int gameweek = persistence.GetGameweek();

                Fpl fpl = new Fpl(gameweek);
                StringBuilder result = fpl.Process().ConfigureAwait(false).GetAwaiter().GetResult();

                if (bool.Parse(ConfigurationManager.AppSettings["attachTable"]))
                {
                    StringBuilder standings = fpl.GenerateStandingsTable();
                    persistence.SaveStandings(standings);
                }

                stopwatch.Stop();

                if (result != null)
                {
                    result
                        .AppendLine()
                        .AppendLine($"Beep boop. This is diagnostics. Completed FPL processing in {((double)stopwatch.ElapsedMilliseconds / 1000).ToString("N1")} seconds.");

                    Console.WriteLine(result.ToString());

                    SendEmail(persistence, gameweek, result);

                    persistence.CompleteGameweek();
                }

                Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["interval"]) * 1000); // expects milliseconds, so multiply by 1000 to get seconds which is what we described in the app.config
            }
        }

        /// <summary>
        /// Sends an email to participants in the league
        /// </summary>
        /// <param name="persistence"></param>
        /// <param name="result"></param>
        /// <remarks>Email addresses must be configured in the app.config file, as the API does not supply the addresses.</remarks>
        private static void SendEmail(Persistence persistence, int gameweek, StringBuilder result)
        {
            string[] recipients = ConfigurationManager.AppSettings["emailTo"]?.Split(';');

            if (recipients == null || recipients.Count() < 1) throw new Exception("App.Config not properly configured.");

            var multipart = new Multipart("mixed");
            var body = new TextPart(TextFormat.Plain) { Text = result.ToString() };
            
            multipart.Add(body);

            bool attachTable = bool.Parse(ConfigurationManager.AppSettings["attachTable"]);

            if (attachTable)
            {
                Stream stream = persistence.GetStandingsStream();

                MimePart attachment = new MimePart("plain", "txt")
                {                    
                    Content = new MimeContent(stream, ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName($"Standings-GW{gameweek.ToString()}.txt")
                };

                multipart.Add(attachment);
            }

            MimeMessage message = new MimeMessage();
            message.Subject = "Weekly update from your friendly FPL bot!";
            message.Body = multipart;
            message.From.Add(new MailboxAddress(ConfigurationManager.AppSettings["emailUser"]));

            foreach (var r in recipients)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    message.To.Add(new MailboxAddress(r));
                }
            }

            using (SmtpClient emailClient = new SmtpClient())
            {
                emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                emailClient.Connect(ConfigurationManager.AppSettings["smtpServer"], int.Parse(ConfigurationManager.AppSettings["smtpPort"]), false);
                emailClient.Authenticate(ConfigurationManager.AppSettings["emailUser"], ConfigurationManager.AppSettings["emailPassword"]);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
