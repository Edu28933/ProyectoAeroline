using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Services;
using System.Security.Claims;

namespace ProyectoAeroline.Pages
{

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DashboardData _dashboardData;
        private readonly IPdfService _pdfService;

        public DashboardStats Estadisticas { get; set; } = new();
        public List<SelectListItem> Aeropuertos { get; set; } = new();
        public List<VueloResumen> VuelosEncontrados { get; set; } = new();
        public string? MensajeBusqueda { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IPdfService pdfService)
        {
            _logger = logger;
            _dashboardData = new DashboardData();
            _pdfService = pdfService;
        }

        public void OnGet(string? origen = null, string? destino = null, DateTime? fecha = null, string? numeroVuelo = null, 
            DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            Estadisticas = _dashboardData.MtdObtenerEstadisticas(fechaInicio, fechaFin);
            Aeropuertos = _dashboardData.MtdObtenerAeropuertosActivos();

            // Si hay criterios de búsqueda, buscar vuelos
            if (!string.IsNullOrWhiteSpace(origen) || !string.IsNullOrWhiteSpace(destino) || fecha.HasValue || !string.IsNullOrWhiteSpace(numeroVuelo))
            {
                if (!string.IsNullOrWhiteSpace(numeroVuelo))
                {
                    var vuelo = _dashboardData.MtdBuscarVueloPorNumero(numeroVuelo);
                    if (vuelo != null)
                    {
                        VuelosEncontrados = new List<VueloResumen> { vuelo };
                        MensajeBusqueda = $"Se encontró el vuelo {numeroVuelo}";
                    }
                    else
                    {
                        MensajeBusqueda = $"No se encontró ningún vuelo con el número {numeroVuelo}";
                    }
                }
                else
                {
                    VuelosEncontrados = _dashboardData.MtdBuscarVuelos(origen, destino, fecha);
                    if (VuelosEncontrados.Any())
                    {
                        MensajeBusqueda = $"Se encontraron {VuelosEncontrados.Count} vuelo(s)";
                    }
                    else
                    {
                        MensajeBusqueda = "No se encontraron vuelos con los criterios especificados";
                    }
                }
            }
        }

        // Método para descargar el dashboard en PDF
        public IActionResult OnGetDescargarPdf(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                // Obtener estadísticas con el mismo rango de fechas que el usuario seleccionó
                var estadisticas = _dashboardData.MtdObtenerEstadisticas(fechaInicio, fechaFin);
                
                // Obtener vuelos recientes (los disponibles por defecto)
                var vuelos = estadisticas.VuelosRecientes ?? new List<VueloResumen>();

                // Obtener nombre del usuario
                var nombreUsuario = User.FindFirst(ClaimTypes.Name)?.Value
                    ?? User.FindFirst("Nombre")?.Value
                    ?? HttpContext.Session?.GetString("Nombre")
                    ?? User.Identity?.Name
                    ?? "Usuario";

                // Limpiar nombre para el nombre de archivo (quitar caracteres especiales)
                var nombreArchivo = System.Text.RegularExpressions.Regex.Replace(nombreUsuario, @"[^\w\s-]", "");
                nombreArchivo = nombreArchivo.Replace(" ", "_");

                // Generar PDF
                var pdfBytes = _pdfService.GenerarPdfDashboard(estadisticas, vuelos, nombreUsuario, fechaInicio, fechaFin);
                
                var fileName = $"Dashboard_{nombreArchivo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                
                // Forzar descarga del PDF
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF del dashboard");
                TempData["Error"] = "Error al generar el PDF del dashboard.";
                return RedirectToPage("/Index");
            }
        }
    }
}
