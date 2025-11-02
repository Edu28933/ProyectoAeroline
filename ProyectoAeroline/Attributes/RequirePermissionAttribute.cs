using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProyectoAeroline.Services;
using System.Security.Claims;

namespace ProyectoAeroline.Attributes
{
    /// <summary>
    /// Atributo que requiere un permiso específico para acceder a una acción
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _nombrePantalla;
        private readonly string _operacion;

        /// <summary>
        /// Requiere un permiso específico
        /// </summary>
        /// <param name="nombrePantalla">Nombre de la pantalla (ej: "Usuarios", "Empleados")</param>
        /// <param name="operacion">Operación: "Ver", "Crear", "Editar", "Eliminar"</param>
        public RequirePermissionAttribute(string nombrePantalla, string operacion)
        {
            _nombrePantalla = nombrePantalla;
            _operacion = operacion;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Si no está autenticado, ya será manejado por [Authorize]
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return; // Dejar que [Authorize] maneje esto
            }

            // Obtener el servicio de permisos
            var permisosService = context.HttpContext.RequestServices.GetService<IPermisosService>();
            if (permisosService == null)
            {
                // Si no está registrado el servicio, denegar acceso por seguridad
                context.Result = new ForbidResult();
                return;
            }

            // Verificar el permiso
            bool tienePermiso = permisosService.TienePermiso(user, _nombrePantalla, _operacion);

            if (!tienePermiso)
            {
                // No tiene permiso, redirigir o mostrar error
                var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Home";
                
                // Obtener TempData correctamente
                var tempDataFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(context.HttpContext);
                tempData["Error"] = $"No tienes permiso para {_operacion} en {_nombrePantalla}";
                
                // Redirigir a Listar del mismo controlador, o a Home si no existe
                context.Result = new RedirectToActionResult("Listar", controllerName, null);
            }
        }
    }
}

