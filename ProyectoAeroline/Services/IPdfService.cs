using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Services
{
    public interface IPdfService
    {
        byte[] GenerarPdfBoleto(BoletosData.BoletoCompletoInfo boletoInfo);
        byte[] GenerarPdfEmpleado(EmpleadosModel empleado);
        byte[] GenerarPdfDashboard(DashboardStats estadisticas, List<VueloResumen> vuelos, string nombreUsuario, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    }
}

