using GrpcService1.Config;
using MimeKit;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace GrpcService1.Services
{
    public interface IEmailService
    {
        #region Public Methods

        Task SendCollecteMailAsync(MimeMessage message);

        #endregion Public Methods
    }

    public class EmailService : IEmailService
    {
        #region Private Fields

        private readonly IMailServerConfiguration _mailServerConfiguration;

        #endregion Private Fields

        #region Public Constructors

        public EmailService(IMailServerConfiguration mailServerConfiguration)
        {
            _mailServerConfiguration = mailServerConfiguration;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task SendCollecteMailAsync(MimeMessage message)
        {
            using var emailClient = new MailKit.Net.Smtp.SmtpClient();
            emailClient.ServerCertificateValidationCallback = (mySender, certificate, chain, sslpolicyErrors) => { return true; };
            emailClient.CheckCertificateRevocation = false;
            await emailClient.ConnectAsync(_mailServerConfiguration.SmtpHost, _mailServerConfiguration.SmtpPort, _mailServerConfiguration.SmtpSsl);
            if (!string.IsNullOrEmpty(_mailServerConfiguration.SmtpUser) && !string.IsNullOrEmpty(_mailServerConfiguration.SmtpPassword))
                await emailClient.AuthenticateAsync(_mailServerConfiguration.SmtpUser, _mailServerConfiguration.SmtpPassword);
            await emailClient.SendAsync(message);
            await emailClient.DisconnectAsync(true);
        }
        #endregion Public Methods
    }
}
