using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Services;
using System.Security.Claims;

namespace ProyectoAeroline.ViewComponents
{
    /// <summary>
    /// ViewComponent para verificar permisos en vistas
    /// Uso: @await Component.InvokeAsync("Permisos", new { nombrePantalla = "Usuarios", operacion = "Crear" })
    /// </summary>
    public class PermisosViewComponent : ViewComponent
    {
        private readonly IPermisosService _permisosService;

        public PermisosViewComponent(IPermisosService permisosService)
        {
            _permisosService = permisosService;
        }

        public IViewComponentResult Invoke(string nombrePantalla, string operacion)
        {
            var user = ViewContext.HttpContext.User;
            bool tienePermiso = _permisosService.TienePermiso(user, nombrePantalla, operacion);
            
            return View(tienePermiso);
        }
    }
}

