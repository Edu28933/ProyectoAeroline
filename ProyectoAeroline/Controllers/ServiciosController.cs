using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class ServiciosController : Controller
    {
        ServiciosData _ServiciosData = new ServiciosData();

        // --- LISTAR SERVICIOS ---
        public IActionResult Listar()
        {
            var oListaServicios = _ServiciosData.MtdConsultarServicios();
            return View(oListaServicios);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        public IActionResult Guardar()
        {
            ViewBag.Boletos = _ServiciosData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposServicio = new List<SelectListItem>
            {
                new SelectListItem { Value = "WiFi", Text = "WiFi" },
                new SelectListItem { Value = "Comida", Text = "Comida" },
                new SelectListItem { Value = "Bebida", Text = "Bebida" },
                new SelectListItem { Value = "Entretenimiento", Text = "Entretenimiento" },
                new SelectListItem { Value = "Asiento Extra", Text = "Asiento Extra" },
                new SelectListItem { Value = "Equipaje Extra", Text = "Equipaje Extra" },
                new SelectListItem { Value = "Prioridad en Embarque", Text = "Prioridad en Embarque" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View();
        }

        // --- GUARDAR SERVICIO (POST) ---
        [HttpPost]
        public IActionResult Guardar(ServiciosModel oServicio)
        {
            if (ModelState.IsValid)
            {
                // Calcular CostoTotal si no se proporciona
                if (oServicio.CostoTotal == null && oServicio.Cantidad.HasValue)
                {
                    oServicio.CostoTotal = oServicio.Costo * oServicio.Cantidad.Value;
                }

                var respuesta = _ServiciosData.MtdAgregarServicio(oServicio);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Boletos = _ServiciosData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposServicio = new List<SelectListItem>
            {
                new SelectListItem { Value = "WiFi", Text = "WiFi" },
                new SelectListItem { Value = "Comida", Text = "Comida" },
                new SelectListItem { Value = "Bebida", Text = "Bebida" },
                new SelectListItem { Value = "Entretenimiento", Text = "Entretenimiento" },
                new SelectListItem { Value = "Asiento Extra", Text = "Asiento Extra" },
                new SelectListItem { Value = "Equipaje Extra", Text = "Equipaje Extra" },
                new SelectListItem { Value = "Prioridad en Embarque", Text = "Prioridad en Embarque" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View(oServicio);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        public IActionResult Modificar(int CodigoServicio)
        {
            var oServicio = _ServiciosData.MtdBuscarServicio(CodigoServicio);

            ViewBag.Boletos = _ServiciosData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposServicio = new List<SelectListItem>
            {
                new SelectListItem { Value = "WiFi", Text = "WiFi" },
                new SelectListItem { Value = "Comida", Text = "Comida" },
                new SelectListItem { Value = "Bebida", Text = "Bebida" },
                new SelectListItem { Value = "Entretenimiento", Text = "Entretenimiento" },
                new SelectListItem { Value = "Asiento Extra", Text = "Asiento Extra" },
                new SelectListItem { Value = "Equipaje Extra", Text = "Equipaje Extra" },
                new SelectListItem { Value = "Prioridad en Embarque", Text = "Prioridad en Embarque" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };

            return View(oServicio);
        }

        // --- MODIFICAR SERVICIO (POST) ---
        [HttpPost]
        public IActionResult Modificar(ServiciosModel oServicio)
        {
            // Calcular CostoTotal si no se proporciona
            if (oServicio.CostoTotal == null && oServicio.Cantidad.HasValue)
            {
                oServicio.CostoTotal = oServicio.Costo * oServicio.Cantidad.Value;
            }

            var respuesta = _ServiciosData.MtdEditarServicio(oServicio);

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
        public IActionResult Eliminar(int CodigoServicio)
        {
            var servicio = _ServiciosData.MtdBuscarServicio(CodigoServicio);

            if (servicio == null || servicio.IdServicio == 0)
            {
                TempData["Error"] = "El servicio no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(servicio);
        }

        // --- ELIMINAR SERVICIO (POST) ---
        [HttpPost]
        public IActionResult Eliminar(ServiciosModel oServicio)
        {
            try
            {
                bool respuesta = _ServiciosData.MtdEliminarServicio(oServicio.IdServicio);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Servicio eliminado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar el servicio.");
                    return View(oServicio);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oServicio);
            }
        }
    }
}

