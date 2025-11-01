using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class RolPantallaOpcionController : Controller
    {
        private readonly RolPantallaOpcionData _RolPantallaOpcionData = new RolPantallaOpcionData();

        // --------------------------------------------------------------
        // LISTAR
        // --------------------------------------------------------------
        public IActionResult Listar()
        {
            var oListaRolPantallaOpciones = _RolPantallaOpcionData.MtdConsultarRolPantallaOpciones();
            return View(oListaRolPantallaOpciones);
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
        public IActionResult Guardar(RolPantallaOpcionModel oRolPantallaOpcion)
        {
            var respuesta = _RolPantallaOpcionData.MtdAgregarRolPantallaOpcion(oRolPantallaOpcion);

            if (respuesta)
            {
                TempData["Success"] = "Permiso creado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al crear el permiso.";
                return View(oRolPantallaOpcion);
            }
        }

        // --------------------------------------------------------------
        // MODIFICAR (GET)
        // --------------------------------------------------------------
        public IActionResult Modificar(int? CodigoRolPantallaOpcion = null)
        {
            int idRolPantallaOpcion = CodigoRolPantallaOpcion ?? 0;

            if (idRolPantallaOpcion == 0)
            {
                TempData["Error"] = "No se pudo determinar el permiso a modificar.";
                return RedirectToAction("Listar");
            }

            var oRolPantallaOpcion = _RolPantallaOpcionData.MtdBuscarRolPantallaOpcion(idRolPantallaOpcion);

            if (oRolPantallaOpcion == null || oRolPantallaOpcion.IdRolPantallaOpcion == 0)
            {
                TempData["Error"] = "Permiso no encontrado.";
                return RedirectToAction("Listar");
            }

            return View(oRolPantallaOpcion);
        }

        // --------------------------------------------------------------
        // MODIFICAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        public IActionResult Modificar(RolPantallaOpcionModel oRolPantallaOpcion)
        {
            var respuesta = _RolPantallaOpcionData.MtdEditarRolPantallaOpcion(oRolPantallaOpcion);

            if (respuesta)
            {
                TempData["Success"] = "Permiso actualizado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al actualizar el permiso.";
                return View(oRolPantallaOpcion);
            }
        }

        // --------------------------------------------------------------
        // ELIMINAR (GET)
        // --------------------------------------------------------------
        public IActionResult Eliminar(int CodigoRolPantallaOpcion)
        {
            var oRolPantallaOpcion = _RolPantallaOpcionData.MtdBuscarRolPantallaOpcion(CodigoRolPantallaOpcion);
            return View(oRolPantallaOpcion);
        }

        // --------------------------------------------------------------
        // ELIMINAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        public IActionResult Eliminar(RolPantallaOpcionModel oRolPantallaOpcion)
        {
            var respuesta = _RolPantallaOpcionData.MtdEliminarRolPantallaOpcion(oRolPantallaOpcion.IdRolPantallaOpcion);

            if (respuesta)
            {
                TempData["Success"] = "Permiso eliminado exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el permiso.";
                return View(oRolPantallaOpcion);
            }
        }
    }
}

