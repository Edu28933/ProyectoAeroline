using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class EquipajeController : Controller
    {
        EquipajeData _EquipajeData = new EquipajeData();

        // --- LISTAR EQUIPAJE ---
        [RequirePermission("Equipaje", "Ver")]
        public IActionResult Listar()
        {
            var oListaEquipaje = _EquipajeData.MtdConsultarEquipajes();
            return View(oListaEquipaje);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        [RequirePermission("Equipaje", "Crear")]
        public IActionResult Guardar()
        {
            ViewBag.Boletos = _EquipajeData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View();
        }

        // --- GUARDAR EQUIPAJE (POST) ---
        [HttpPost]
        [RequirePermission("Equipaje", "Crear")]
        public IActionResult Guardar(EquipajeModel oEquipaje)
        {
            if (ModelState.IsValid)
            {
                var respuesta = _EquipajeData.MtdAgregarEquipaje(oEquipaje);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Boletos = _EquipajeData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View(oEquipaje);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        [RequirePermission("Equipaje", "Editar")]
        public IActionResult Modificar(int CodigoEquipaje)
        {
            var oEquipaje = _EquipajeData.MtdBuscarEquipaje(CodigoEquipaje);

            ViewBag.Boletos = _EquipajeData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View(oEquipaje);
        }

        // --- MODIFICAR EQUIPAJE (POST) ---
        [HttpPost]
        [RequirePermission("Equipaje", "Editar")]
        public IActionResult Modificar(EquipajeModel oEquipaje)
        {
            var respuesta = _EquipajeData.MtdEditarEquipaje(oEquipaje);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
            }
        }

        // --- MOSTRAR FORMULARIO ELIMINAR ---
        [RequirePermission("Equipaje", "Eliminar")]
        public IActionResult Eliminar(int CodigoEquipaje)
        {
            var equipaje = _EquipajeData.MtdBuscarEquipaje(CodigoEquipaje);

            if (equipaje == null || equipaje.IdEquipaje == 0)
            {
                TempData["Error"] = "El equipaje no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(equipaje);
        }

        // --- ELIMINAR EQUIPAJE (POST) ---
        [HttpPost]
        [RequirePermission("Equipaje", "Eliminar")]
        public IActionResult Eliminar(EquipajeModel oEquipaje)
        {
            try
            {
                bool respuesta = _EquipajeData.MtdEliminarEquipaje(oEquipaje.IdEquipaje);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Equipaje eliminado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar el equipaje.");
                    return View(oEquipaje);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oEquipaje);
            }
        }
    }
}

