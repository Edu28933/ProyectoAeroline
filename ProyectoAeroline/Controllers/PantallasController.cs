using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class PantallasController : Controller
    {
        private readonly PantallasData _PantallasData = new PantallasData();

        // --------------------------------------------------------------
        // LISTAR
        // --------------------------------------------------------------
        [RequirePermission("Pantallas", "Ver")]
        public IActionResult Listar()
        {
            var oListaPantallas = _PantallasData.MtdConsultarPantallas();
            return View(oListaPantallas);
        }

        // --------------------------------------------------------------
        // GUARDAR (GET)
        // --------------------------------------------------------------
        [RequirePermission("Pantallas", "Crear")]
        public IActionResult Guardar()
        {
            return View();
        }

        // --------------------------------------------------------------
        // GUARDAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [RequirePermission("Pantallas", "Crear")]
        public IActionResult Guardar(PantallasModel oPantalla)
        {
            var respuesta = _PantallasData.MtdAgregarPantalla(oPantalla);

            if (respuesta)
            {
                TempData["Success"] = "Pantalla creada exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al crear la pantalla.";
                return View(oPantalla);
            }
        }

        // --------------------------------------------------------------
        // MODIFICAR (GET)
        // --------------------------------------------------------------
        [RequirePermission("Pantallas", "Editar")]
        public IActionResult Modificar(int? CodigoPantalla = null)
        {
            int idPantalla = CodigoPantalla ?? 0;

            if (idPantalla == 0)
            {
                TempData["Error"] = "No se pudo determinar la pantalla a modificar.";
                return RedirectToAction("Listar");
            }

            var oPantalla = _PantallasData.MtdBuscarPantalla(idPantalla);

            if (oPantalla == null || oPantalla.IdPantalla == 0)
            {
                TempData["Error"] = "Pantalla no encontrada.";
                return RedirectToAction("Listar");
            }

            return View(oPantalla);
        }

        // --------------------------------------------------------------
        // MODIFICAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [RequirePermission("Pantallas", "Editar")]
        public IActionResult Modificar(PantallasModel oPantalla)
        {
            var respuesta = _PantallasData.MtdEditarPantalla(oPantalla);

            if (respuesta)
            {
                TempData["Success"] = "Pantalla actualizada exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al actualizar la pantalla.";
                return View(oPantalla);
            }
        }

        // --------------------------------------------------------------
        // ELIMINAR (GET)
        // --------------------------------------------------------------
        [RequirePermission("Pantallas", "Eliminar")]
        public IActionResult Eliminar(int CodigoPantalla)
        {
            var oPantalla = _PantallasData.MtdBuscarPantalla(CodigoPantalla);
            return View(oPantalla);
        }

        // --------------------------------------------------------------
        // ELIMINAR (POST)
        // --------------------------------------------------------------
        [HttpPost]
        [RequirePermission("Pantallas", "Eliminar")]
        public IActionResult Eliminar(PantallasModel oPantalla)
        {
            var respuesta = _PantallasData.MtdEliminarPantalla(oPantalla.IdPantalla);

            if (respuesta)
            {
                TempData["Success"] = "Pantalla eliminada exitosamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "Error al eliminar la pantalla.";
                return View(oPantalla);
            }
        }
    }
}

