using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProyectoAeroline.Services;
using System.Security.Claims;

namespace ProyectoAeroline.Helpers
{
    /// <summary>
    /// Helper para verificar permisos en vistas Razor
    /// </summary>
    public static class PermisosHelper
    {
        /// <summary>
        /// Verifica si el usuario tiene un permiso específico
        /// </summary>
        public static bool TienePermiso(ViewContext viewContext, string nombrePantalla, string operacion)
        {
            var httpContext = viewContext.HttpContext;
            var user = httpContext.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            var permisosService = httpContext.RequestServices.GetService<IPermisosService>();
            if (permisosService == null)
            {
                return false;
            }

            return permisosService.TienePermiso(user, nombrePantalla, operacion);
        }

        /// <summary>
        /// Verifica si el usuario tiene permiso para Ver
        /// </summary>
        public static bool PuedeVer(ViewContext viewContext, string nombrePantalla)
        {
            return TienePermiso(viewContext, nombrePantalla, "Ver");
        }

        /// <summary>
        /// Verifica si el usuario tiene permiso para Crear
        /// </summary>
        public static bool PuedeCrear(ViewContext viewContext, string nombrePantalla)
        {
            return TienePermiso(viewContext, nombrePantalla, "Crear");
        }

        /// <summary>
        /// Verifica si el usuario tiene permiso para Editar
        /// </summary>
        public static bool PuedeEditar(ViewContext viewContext, string nombrePantalla)
        {
            return TienePermiso(viewContext, nombrePantalla, "Editar");
        }

        /// <summary>
        /// Verifica si el usuario tiene permiso para Eliminar
        /// </summary>
        public static bool PuedeEliminar(ViewContext viewContext, string nombrePantalla)
        {
            return TienePermiso(viewContext, nombrePantalla, "Eliminar");
        }
    }
}

