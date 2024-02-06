using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ApiRestCuestionario.Utils
{
    public class EmailSettings
    {
        public string From { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty; 
        public int Port { get; set; } = 0;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
    public interface IGmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class GmailSender : IGmailSender
    {
        private readonly EmailSettings _emailSettings;
        public GmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.From, _emailSettings.Username),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email));

            using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            client.EnableSsl = true;

            await client.SendMailAsync(mailMessage);
        }
    }
}
