using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly RolesData _RolesData = new RolesData();

        // --------------------------------------------------------------
        // LISTAR
        // --------------------------------------------------------------
        public IActionResult Listar()
        {
            var oListaRoles = _RolesData.MtdConsultarRoles();
            return View(oListaRoles);
        }

        // --------------------------------------------------------------
        // GUARDAR (GET)
        // --------------------------------------------------------------
        public IActionResult Guardar()
        {
            return View();
        }

        // --------------------------------------------------------------
        // GUARDAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        public IActionResult Guardar(RolesModel oRol)
        {
            var respuesta = _RolesData.MtdAgregarRol(oRol);

            if (respuesta)
            {
                TempData["Success"] = "Rol creado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al crear el rol.";
                return View(oRol);
            }
        }

        // --------------------------------------------------------------
        // MODIFICAR (GET)
        // --------------------------------------------------------------
        public IActionResult Modificar(int? CodigoRol = null)
        {
            int idRol = CodigoRol ?? 0;

            if (idRol == 0)
            {
                TempData["Error"] = "No se pudo determinar el rol a modificar.";
                return RedirectToAction("Listar");
            }

            var oRol = _RolesData.MtdBuscarRol(idRol);

            if (oRol == null || oRol.IdRol == 0)
            {
                TempData["Error"] = "Rol no encontrado.";
                return RedirectToAction("Listar");
            }

            return View(oRol);
        }

        // --------------------------------------------------------------
        // MODIFICAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        public IActionResult Modificar(RolesModel oRol)
        {
            var respuesta = _RolesData.MtdEditarRol(oRol);

            if (respuesta)
            {
                TempData["Success"] = "Rol actualizado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al actualizar el rol.";
                return View(oRol);
            }
        }

        // --------------------------------------------------------------
        // ELIMINAR (GET)
        // --------------------------------------------------------------
        public IActionResult Eliminar(int CodigoRol)
        {
            var oRol = _RolesData.MtdBuscarRol(CodigoRol);
            return View(oRol);
        }

        // --------------------------------------------------------------
        // ELIMINAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        public IActionResult Eliminar(RolesModel oRol)
        {
            var respuesta = _RolesData.MtdEliminarRol(oRol.IdRol);

            if (respuesta)
            {
                TempData["Success"] = "Rol eliminado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el rol. Puede que esté siendo usado por algún usuario.";
                return View(oRol);
            }
        }
    }
}

