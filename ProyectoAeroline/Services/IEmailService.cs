using System.Threading.Tasks;

namespace ProyectoAeroline.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
