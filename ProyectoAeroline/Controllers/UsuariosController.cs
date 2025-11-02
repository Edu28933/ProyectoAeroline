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
            var respuesta = _UsuariosData.MtdAgregarUsuario(oUsuario);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
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
            var respuesta = _UsuariosData.MtdEditarUsuario(oUsuario);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View(oUsuario);
        }

        // --------------------------------------------------------------
        // ELIMINAR (GET)
        // --------------------------------------------------------------
        [Authorize]
        [RequirePermission("Usuarios", "Eliminar")]
        public IActionResult Eliminar(int CodigoUsuario)
        {
            var oUsuario = _UsuariosData.MtdBuscarUsuario(CodigoUsuario);
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
            var respuesta = _UsuariosData.MtdEliminarUsuario(oUsuario.IdUsuario);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View(oUsuario);
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
