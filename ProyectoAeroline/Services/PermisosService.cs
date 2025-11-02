using ProyectoAeroline.Data;
using System.Security.Claims;

namespace ProyectoAeroline.Services
{
    public interface IPermisosService
    {
        bool TienePermiso(int? idUsuario, int? idRol, string nombrePantalla, string operacion);
        bool TienePermiso(ClaimsPrincipal user, string nombrePantalla, string operacion);
        Task<bool> TienePermisoAsync(ClaimsPrincipal user, string nombrePantalla, string operacion);
    }

    public class PermisosService : IPermisosService
    {
        private readonly RolPantallaOpcionData _rolPantallaOpcionData;

        public PermisosService(RolPantallaOpcionData rolPantallaOpcionData)
        {
            _rolPantallaOpcionData = rolPantallaOpcionData;
        }

        /// <summary>
        /// Verifica si un usuario tiene un permiso específico
        /// </summary>
        /// <param name="idUsuario">ID del usuario</param>
        /// <param name="idRol">ID del rol (opcional, se puede obtener del usuario)</param>
        /// <param name="nombrePantalla">Nombre de la pantalla (ej: "Usuarios", "Empleados")</param>
        /// <param name="operacion">Operación: "Ver", "Crear", "Editar", "Eliminar"</param>
        /// <returns>True si tiene permiso, False si no</returns>
        public bool TienePermiso(int? idUsuario, int? idRol, string nombrePantalla, string operacion)
        {
            try
            {
                // Si el rol es SuperAdmin (IdRol = 1), tiene acceso a todo
                if (idRol.HasValue && idRol.Value == 1)
                {
                    return true;
                }

                if (!idRol.HasValue)
                {
                    return false;
                }

                // Obtener todos los permisos del rol
                var permisos = _rolPantallaOpcionData.MtdConsultarRolPantallaOpciones();
                
                // Buscar el permiso específico para este rol y pantalla
                var permiso = permisos.FirstOrDefault(p => 
                    p.IdRol == idRol.Value && 
                    string.Equals(p.NombrePantalla, nombrePantalla, StringComparison.OrdinalIgnoreCase) &&
                    p.Estado == "Activo");

                if (permiso == null)
                {
                    return false;
                }

                // Verificar la operación específica
                return operacion.ToUpper() switch
                {
                    "VER" => permiso.Ver,
                    "CREAR" => permiso.Crear,
                    "EDITAR" => permiso.Editar,
                    "ELIMINAR" => permiso.Eliminar,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica permisos desde un ClaimsPrincipal (síncrono)
        /// </summary>
        public bool TienePermiso(ClaimsPrincipal user, string nombrePantalla, string operacion)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            // Obtener IdRol desde claims
            int? idRol = null;
            
            // Intentar obtener IdRol del claim "IdRol"
            var idRolClaim = user.FindFirst("IdRol")?.Value;
            if (!string.IsNullOrEmpty(idRolClaim) && int.TryParse(idRolClaim, out int idRolValue))
            {
                idRol = idRolValue;
            }
            
            // Si no se encontró IdRol en claims, intentar desde el nombre del rol
            if (!idRol.HasValue)
            {
                var rolNombre = user.FindFirst(ClaimTypes.Role)?.Value;
                if (!string.IsNullOrEmpty(rolNombre))
                {
                    // Mapeo directo: SuperAdmin = IdRol 1
                    if (rolNombre.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                    {
                        idRol = 1;
                    }
                    // Puedes agregar más mapeos aquí si es necesario
                }
            }

            if (!idRol.HasValue)
            {
                return false;
            }

            return TienePermiso(null, idRol, nombrePantalla, operacion);
        }

        /// <summary>
        /// Verifica permisos desde un ClaimsPrincipal (asíncrono)
        /// </summary>
        public Task<bool> TienePermisoAsync(ClaimsPrincipal user, string nombrePantalla, string operacion)
        {
            return Task.FromResult(TienePermiso(user, nombrePantalla, operacion));
        }
    }
}

