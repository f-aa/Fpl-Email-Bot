using FplBot.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace FplBot.Services
{
    public class EmailService(
        IPersistenceService persistenceService,
        IAttachmentService attachmentService,
        IOptions<EmailOptions> emailOptions,
        IOptions<FplOptions> fplOptions,
        Logging.ILogger logger) : IEmailService
    {
        private readonly IPersistenceService persistenceService = persistenceService;
        private readonly IAttachmentService attachmentService = attachmentService;
        private readonly EmailOptions emailOptions = emailOptions.Value ?? throw new Exception($"Couldn't load {typeof(EmailOptions)} from app settings");
        private readonly FplOptions fplOptions = fplOptions.Value ?? throw new Exception($"Couldn't load {typeof(FplOptions)} from app settings");
        private readonly Logging.ILogger logger = logger;

        /// <summary>
        /// Sends an email to participants in the league
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>Email addresses must be configured in the app.config file, as the API does not supply the addresses.</remarks>
        public async Task SendEmail(string output, string eventName)
        {
            this.logger.Log("Attempting to send email...");

            Stream? standingsStream = null;
            Stream? weeklyWinsStream = null;

            try
            {
                if (output == null) throw new Exception("Could not find an output to email.");

                var multipart = new Multipart("mixed");
                var body = new TextPart(TextFormat.Plain) { Text = output };

                multipart.Add(body);

                if (this.fplOptions.AttachTable)
                {
                    this.persistenceService.SaveStandings(await this.attachmentService.GenerateStandingsTable());
                    standingsStream = File.OpenRead("standings.txt");

                    MimePart attachment = new MimePart("plain", "txt")
                    {
                        Content = new MimeContent(standingsStream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName($"Standings-{eventName.Replace(" ", "").Replace("+", "plus")}.txt")
                    };

                    multipart.Add(attachment);
                }

                if (this.fplOptions.AttachWeeklyWins)
                {
                    this.persistenceService.SaveWeeklyWins(await this.attachmentService.GenerateTotalWeeklyWins());
                    weeklyWinsStream = File.OpenRead("weeklywins.txt");

                    MimePart attachment = new MimePart("plain", "txt")
                    {
                        Content = new MimeContent(weeklyWinsStream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName($"WeeklyWinSummary-{eventName.Replace(" ", "").Replace("+", "plus")}.txt")
                    };

                    multipart.Add(attachment);
                }

                MimeMessage message = new MimeMessage();
                message.Subject = "Weekly update from your friendly FPL bot!";
                message.Body = multipart;
                message.From.Add(new MailboxAddress("FPLBot", this.emailOptions.EmailFrom));
                message.To.AddRange(
                    this.emailOptions
                        .EmailTo
                        .Split(",")
                        .Where(r => !string.IsNullOrEmpty(r))
                        .Select(r => new MailboxAddress(r, r)));

                using SmtpClient emailClient = new();

                this.logger.Log($"Connecting to {this.emailOptions.SmtpServer}...");
                emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                emailClient.Connect(this.emailOptions.SmtpServer, this.emailOptions.SmtpPort, false);
                emailClient.Authenticate(this.emailOptions.EmailUser, this.emailOptions.EmailPassword);
                emailClient.Send(message);
                emailClient.Disconnect(true);
                this.logger.Log("Email sent successfully.");
            }
            catch (Exception ex)
            {
                this.logger.Log($"Unable to send email: {ex.Message}");
                Environment.Exit(-1);
            }
            finally
            {
                standingsStream?.Dispose();
                weeklyWinsStream?.Dispose();
            }
        }
    }
}
