using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Microsoft.Extensions.Configuration;
using System;
namespace ApiRestCuestionario.Utils
{
    public class Mailer
    {
        readonly IConfiguration _configuration;
        Mailer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public enum MailCompose
        {
            NewSolicitud = 0,
            ChangeStatusSoli = 1,
            recoverPassword = 2,
            RiesgosExternos = 3
        }

        public void SendMailAll(string address, string subject, string body, string AddressCopy, string[] fileEntries = null, bool isAdjuntImage = true)
        {
            var message = new MailMessage();

            message.To.Add(new MailAddress(address));
            if (!string.IsNullOrEmpty(AddressCopy)) message.To.Add(new MailAddress(AddressCopy));
            message.Subject = subject;
            message.IsBodyHtml = true;

            string fileName = string.Format("{0}{1}", _configuration.GetValue<string>("Templates"), "logo.png");

            AlternateView av = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);

            if (isAdjuntImage)
            {

                LinkedResource lr = new LinkedResource(fileName, MediaTypeNames.Image.Jpeg);
                lr.ContentId = "Logo";
                av.LinkedResources.Add(lr);
            }

            message.AlternateViews.Add(av);
            message.Body = body;

            if (fileEntries != null)
            {
                foreach (string file in fileEntries)
                {
                    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                    message.Attachments.Add(data);
                }
            }

            try
            {
                string host = _configuration.GetValue<string>("Smtp:host");
                int port = _configuration.GetValue<int>("Smtp:port", 25);
                string fromAddress = _configuration.GetValue<string>("Smtp:from");
                string userName = _configuration.GetValue<string>("Smtp:userName");
                string password = _configuration.GetValue<string>("Smtp:password");


                message.From = new MailAddress(fromAddress);
                using (var smtp = new SmtpClient(host, port))
                {
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(userName, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
