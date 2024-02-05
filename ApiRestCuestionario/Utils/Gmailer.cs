using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace ApiRestCuestionario.Utils
{
    public class EmailSettings
    {
   public string From;
    public string Host;
    public int Port;
    public string Password;
    public string Username;  
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
