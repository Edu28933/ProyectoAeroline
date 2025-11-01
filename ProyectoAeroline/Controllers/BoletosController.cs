using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class BoletosController : Controller
    {
        // Instancia de la clase con la conexión y stored procedures
        BoletosData _BoletosData = new BoletosData();

        // --- LISTAR BOLETOS ---
        public IActionResult Listar()
        {
            var oListaBoletos = _BoletosData.MtdConsultarBoletos();
            return View(oListaBoletos);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        public IActionResult Guardar()
        {
            ViewBag.Vuelos = _BoletosData.MtdListarVuelosActivos();
            ViewBag.Pasajeros = _BoletosData.MtdListarPasajerosActivos();

            // Clases de asiento
            ViewBag.Clases = new List<SelectListItem>
            {
                new SelectListItem { Value = "Económica", Text = "Económica" },
                new SelectListItem { Value = "Ejecutiva", Text = "Ejecutiva" },
                new SelectListItem { Value = "Primera Clase", Text = "Primera Clase" }
            };

            // Estados posibles
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" }
            };

            return View();
        }

        // --- GUARDAR BOLETO (POST) ---
        [HttpPost]
        public IActionResult Guardar(BoletosModel oBoleto)
        {
            if (ModelState.IsValid)
            {
                var respuesta = _BoletosData.MtdAgregarBoleto(oBoleto);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Vuelos = _BoletosData.MtdListarVuelosActivos();
            ViewBag.Pasajeros = _BoletosData.MtdListarPasajerosActivos();

            ViewBag.Clases = new List<SelectListItem>
            {
                new SelectListItem { Value = "Económica", Text = "Económica" },
                new SelectListItem { Value = "Ejecutiva", Text = "Ejecutiva" },
                new SelectListItem { Value = "Primera Clase", Text = "Primera Clase" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" }
            };

            return View(oBoleto);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        public IActionResult Modificar(int CodigoBoleto)
        {
            var oBoleto = _BoletosData.MtdBuscarBoleto(CodigoBoleto);

            ViewBag.Vuelos = _BoletosData.MtdListarVuelosActivos();
            ViewBag.Pasajeros = _BoletosData.MtdListarPasajerosActivos();

            ViewBag.Clases = new List<SelectListItem>
            {
                new SelectListItem { Value = "Económica", Text = "Económica" },
                new SelectListItem { Value = "Ejecutiva", Text = "Ejecutiva" },
                new SelectListItem { Value = "Primera Clase", Text = "Primera Clase" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" }
            };

            return View(oBoleto);
        }

        // --- MODIFICAR BOLETO (POST) ---
        [HttpPost]
        public IActionResult Modificar(BoletosModel oBoleto)
        {
            var respuesta = _BoletosData.MtdEditarBoleto(oBoleto);

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
        public IActionResult Eliminar(int CodigoBoleto)
        {
            var boleto = _BoletosData.MtdBuscarBoleto(CodigoBoleto);

            if (boleto == null || boleto.IdBoleto == 0)
            {
                TempData["Error"] = "El boleto no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(boleto);
        }

        // --- ELIMINAR BOLETO (POST) ---
        [HttpPost]
        public IActionResult Eliminar(BoletosModel oBoleto)
        {
            try
            {
                bool respuesta = _BoletosData.MtdEliminarBoleto(oBoleto.IdBoleto);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Boleto eliminado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar el boleto, su estado no lo permite.");
                    return View(oBoleto);
                }
            }
            catch (SqlException ex)
            {
                // Captura el mensaje del trigger (por ejemplo: "Solo se pueden eliminar boletos en estado 'Cancelado'.")
                ModelState.AddModelError("", ex.Message);
                return View(oBoleto);
            }
        }

        [HttpGet]
        public JsonResult ObtenerPrecioVuelo(int idVuelo)
        {
            var vuelo = _BoletosData.MtdBuscarVuelo(idVuelo);
            if (vuelo == null)
                return Json(new { precio = 0 });

            return Json(new { precio = vuelo.Precio });
        }

    }
}
