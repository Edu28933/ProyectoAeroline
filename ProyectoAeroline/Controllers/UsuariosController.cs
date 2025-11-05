using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using ProyectoAeroline.Attributes;
using System.Security.Claims;

namespace ProyectoAeroline.Controllers
{
    public class UsuariosController : Controller
    {
        // Instancia de la clase con la conexión y stored procedures
        private readonly UsuariosData _UsuariosData = new UsuariosData();

        // --------------------------------------------------------------
        // LISTAR
        // --------------------------------------------------------------
        [Authorize]
        [RequirePermission("Usuarios", "Ver")]
        public IActionResult Listar()
        {
            var oListaUsuarios = _UsuariosData.MtdConsultarUsuarios();
            return View(oListaUsuarios);
        }

        // --------------------------------------------------------------
        // GUARDAR (GET)
        // --------------------------------------------------------------
        [Authorize]
        [RequirePermission("Usuarios", "Crear")]
        public IActionResult Guardar()
        {
            ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
            return View();
        }

        // --------------------------------------------------------------
        // GUARDAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [Authorize]
        [RequirePermission("Usuarios", "Crear")]
        public IActionResult Guardar(UsuariosModel oUsuario)
        {
            // Validar que el nombre de usuario no se repita
            if (_UsuariosData.MtdVerificarNombreExiste(oUsuario.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre de usuario ya existe. Por favor, elija otro nombre.");
                ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
                return View(oUsuario);
            }

            var respuesta = _UsuariosData.MtdAgregarUsuario(oUsuario);

            if (respuesta)
            {
                TempData["Success"] = "Usuario guardado correctamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                // Recargar combos si hay error
                ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
                ModelState.AddModelError("", "Error al guardar el usuario. Por favor, intente nuevamente.");
                return View(oUsuario);
            }
        }

        // --------------------------------------------------------------
        // MODIFICAR (GET)
        // --------------------------------------------------------------
        [Authorize]
        [RequirePermission("Usuarios", "Editar")]
        public IActionResult Modificar(int? CodigoUsuario = null)
        {
            int idUsuario = CodigoUsuario ?? 0;

            // 🔹 Si no viene el parámetro, obtenemos el ID del usuario logueado desde los claims
            if (idUsuario == 0)
            {
                var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(claimId, out int parsedId))
                    idUsuario = parsedId;
            }

            if (idUsuario == 0)
            {
                TempData["Error"] = "No se pudo determinar el usuario a modificar.";
                return RedirectToAction("Listar");
            }

            var oUsuario = _UsuariosData.MtdBuscarUsuario(idUsuario);

            if (oUsuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Listar");
            }

            ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
            return View(oUsuario);
        }

        // --------------------------------------------------------------
        // MODIFICAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [Authorize]
        [RequirePermission("Usuarios", "Editar")]
        public IActionResult Modificar(UsuariosModel oUsuario)
        {
            // Validar que el nombre de usuario no se repita (excluyendo el usuario actual)
            if (_UsuariosData.MtdVerificarNombreExiste(oUsuario.Nombre, oUsuario.IdUsuario))
            {
                ModelState.AddModelError("Nombre", "El nombre de usuario ya existe. Por favor, elija otro nombre.");
                ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
                return View(oUsuario);
            }

            // Si la contraseña está vacía, obtener la contraseña actual del usuario
            if (string.IsNullOrWhiteSpace(oUsuario.Contraseña))
            {
                var usuarioActual = _UsuariosData.MtdBuscarUsuario(oUsuario.IdUsuario);
                if (usuarioActual != null)
                {
                    oUsuario.Contraseña = usuarioActual.Contraseña;
                }
            }

            var respuesta = _UsuariosData.MtdEditarUsuario(oUsuario);

            if (respuesta)
            {
                TempData["Success"] = "Usuario modificado correctamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                // Recargar combos si hay error
                ViewBag.Roles = _UsuariosData.MtdListarRolesActivos();
                ModelState.AddModelError("", "Error al modificar el usuario. Por favor, intente nuevamente.");
                return View(oUsuario);
            }
        }

        // --------------------------------------------------------------
        // ELIMINAR (GET)
        // --------------------------------------------------------------
        [Authorize]
        [RequirePermission("Usuarios", "Eliminar")]
        public IActionResult Eliminar(int CodigoUsuario)
        {
            var oUsuario = _UsuariosData.MtdBuscarUsuario(CodigoUsuario);
            
            if (oUsuario == null || oUsuario.IdUsuario == 0)
            {
                TempData["Error"] = "El usuario no existe.";
                return RedirectToAction("Listar");
            }
            
            return View(oUsuario);
        }

        // --------------------------------------------------------------
        // ELIMINAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [Authorize]
        [RequirePermission("Usuarios", "Eliminar")]
        public IActionResult Eliminar(UsuariosModel oUsuario)
        {
            var (success, errorMessage) = _UsuariosData.MtdEliminarUsuario(oUsuario.IdUsuario);

            if (success)
            {
                TempData["Success"] = "Usuario eliminado correctamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                // Mostrar el mensaje de error específico
                TempData["Error"] = !string.IsNullOrEmpty(errorMessage) 
                    ? errorMessage 
                    : "No se pudo eliminar el usuario. Por favor, verifique que no tenga empleados asociados y que su estado sea 'Inactivo'.";
                
                // Recargar el usuario para mostrarlo en la vista
                var usuarioActualizado = _UsuariosData.MtdBuscarUsuario(oUsuario.IdUsuario);
                return View(usuarioActualizado ?? oUsuario);
            }
        }

        // --------------------------------------------------------------
        // PERFIL DIRECTO (opcional)
        // --------------------------------------------------------------
        [Authorize]
        public IActionResult MiPerfil()
        {
            // 🔹 Acción directa para acceder al perfil del usuario logueado
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claimId, out int idUsuario))
                return RedirectToAction("Listar");

            var oUsuario = _UsuariosData.MtdBuscarUsuario(idUsuario);

            if (oUsuario == null)
                return RedirectToAction("Listar");

            // Reutiliza la vista Modificar
            return View("Modificar", oUsuario);
        }
    }
}
