using System.Threading.Tasks;

namespace ProyectoAeroline.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
        Task SendWithAttachmentAsync(string toEmail, string subject, string htmlBody, byte[] attachmentBytes, string attachmentFileName);
    }
}
