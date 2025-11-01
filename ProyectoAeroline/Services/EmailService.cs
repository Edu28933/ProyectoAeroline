using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ProyectoAeroline.Services
{
    // Lee configuración de appsettings:
    // "Smtp": {
    //   "Host": "smtp.gmail.com",
    //   "Port": 587,
    //   "User": "tucorreo@gmail.com",
    //   "Pass": "tu-app-password",
    //   "FromEmail": "tucorreo@gmail.com",
    //   "FromName": "Proyecto Aerolínea"
    // }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<EmailService> _log;

        public EmailService(IConfiguration cfg, ILogger<EmailService> log)
        {
            _cfg = cfg;
            _log = log;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _cfg["Smtp:Host"];
            var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
            var user = _cfg["Smtp:User"];
            var pass = _cfg["Smtp:Pass"];
            var fromEmail = _cfg["Smtp:FromEmail"] ?? user;
            var fromName = _cfg["Smtp:FromName"] ?? "Notificaciones";

            using var msg = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(host)
            {
                Port = port,
                EnableSsl = true,
                Credentials = new NetworkCredential(user, pass)
            };

            try
            {
                await client.SendMailAsync(msg);
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "Error enviando correo a {to}", toEmail);
                throw;
            }
        }
    }
}
