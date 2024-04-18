namespace FplBot.Options
{
    public class EmailOptions
    {

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string EmailTo { get; set; }

        public string EmailFrom { get; set; }

        public string EmailUser { get; set; }

        public string EmailPassword { get; set; }
    }
}
