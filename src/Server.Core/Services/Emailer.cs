using System;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SimplySocial.Server.Core.Settings;

using MimeKit;
using MailKit.Security;
using MKSmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace SimplySocial.Server.Core.Services
{
    public class Emailer : IEmailer
    {
        #region Dependency Injected Members
        private readonly ILogger<Emailer>   _logger;
        private readonly SmtpSettings       _smtpSettings;
        #endregion

        public Emailer(ILogger<Emailer> logger, IOptions<SmtpSettings> smtpSettings)
        {
            _logger         = logger;
            _smtpSettings   = smtpSettings.Value;
        }

        public async Task SendEmailConfirmationAsync(String recipient, String username, String confirmationLink, CancellationToken cancellationToken = default)
        {
            var body = Properties.Resources.ConfirmEmailTemplate.Replace("[[UserName]]", username);
            var email = new MimeMessage
            {
                Subject = "SimplySocial: Confirm Email Address",
                Body    = new TextPart("html") { Text = body }
            };

            email.To.Add(MailboxAddress.Parse(recipient));
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));

            using (var client = new MKSmtpClient())
            {
                client.ServerCertificateValidationCallback = (sender, cert, chain, errors) =>
                {
                    return errors == SslPolicyErrors.None;
                };

                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password, cancellationToken);
                await client.SendAsync(email, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}
