namespace GrpcService1.Config
{
    public interface IMailServerConfiguration
    {
        #region Public Properties

        string SmtpHost { get; }
        string SmtpPassword { get; }
        int SmtpPort { get; }
        bool SmtpSsl { get; }
        string SmtpUser { get; }

        #endregion Public Properties
    }

    public class MailServerConfiguration : IMailServerConfiguration
    {
        #region Public Properties

        public string SmtpHost { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSsl { get; set; }
        public string SmtpUser { get; set; }

        #endregion Public Properties
    }
}
