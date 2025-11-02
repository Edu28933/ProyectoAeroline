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
                new SelectListItem { Value = "Primera", Text = "Primera Clase" }
            };

            // Estados posibles
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Utilizado", Text = "Utilizado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
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
                new SelectListItem { Value = "Primera", Text = "Primera Clase" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Utilizado", Text = "Utilizado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
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
                new SelectListItem { Value = "Primera", Text = "Primera Clase" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Utilizado", Text = "Utilizado" },
                new SelectListItem { Value = "Reembolsado", Text = "Reembolsado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
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
        public JsonResult ObtenerPrecioVuelo(int idVuelo, string? clase = null)
        {
            var vuelo = _BoletosData.MtdBuscarVuelo(idVuelo);
            if (vuelo == null || !vuelo.Precio.HasValue)
                return Json(new { precio = 0, precioBase = 0 });

            decimal precioBase = vuelo.Precio.Value;
            decimal precioFinal = precioBase;

            // Aplicar ajustes según la clase
            if (!string.IsNullOrEmpty(clase))
            {
                if (clase == "Ejecutiva")
                {
                    precioFinal = precioBase * 1.30m; // +30%
                }
                else if (clase == "Primera")
                {
                    precioFinal = precioBase * 1.50m; // +50%
                }
                // Económica se queda con el precio base
            }

            return Json(new { precio = precioFinal, precioBase = precioBase });
        }

        [HttpGet]
        public JsonResult ObtenerTipoPasajero(int idPasajero)
        {
            var tipoPasajero = _BoletosData.MtdObtenerTipoPasajero(idPasajero);
            return Json(new { tipoPasajero = tipoPasajero ?? "" });
        }

        [HttpGet]
        public JsonResult ObtenerCapacidadAvion(int idVuelo)
        {
            var capacidad = _BoletosData.MtdObtenerCapacidadAvionPorVuelo(idVuelo);
            return Json(new { capacidad = capacidad });
        }

        [HttpGet]
        public JsonResult ObtenerAsientosOcupados(int idVuelo, int? idBoletoExcluir = null)
        {
            var asientosOcupados = _BoletosData.MtdObtenerAsientosOcupados(idVuelo, idBoletoExcluir);
            return Json(new { asientosOcupados = asientosOcupados });
        }

        [HttpGet]
        public JsonResult ObtenerInfoVuelo(int idVuelo)
        {
            var vuelo = _BoletosData.MtdObtenerInfoVuelo(idVuelo);
            if (vuelo == null)
            {
                return Json(new { 
                    fechaSalida = (DateTime?)null, 
                    fechaLlegada = (DateTime?)null,
                    aeropuertoOrigen = "",
                    aeropuertoDestino = ""
                });
            }

            return Json(new { 
                fechaSalida = vuelo.FechaHoraSalida,
                fechaLlegada = vuelo.FechaHoraLlegada,
                aeropuertoOrigen = vuelo.AeropuertoOrigen ?? "",
                aeropuertoDestino = vuelo.AeropuertoDestino ?? ""
            });
        }

    }
}
