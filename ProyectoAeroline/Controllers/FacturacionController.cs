using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class FacturacionController : Controller
    {
        FacturacionData _FacturacionData = new FacturacionData();

        // --- LISTAR FACTURACIÓN ---
        public IActionResult Listar()
        {
            var oListaFacturacion = _FacturacionData.MtdConsultarFacturacion();
            return View(oListaFacturacion);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        public IActionResult Guardar()
        {
            ViewBag.Boletos = _FacturacionData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
                new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                new SelectListItem { Value = "Cheque", Text = "Cheque" }
            };

            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Value = "USD", Text = "USD - Dólares" },
                new SelectListItem { Value = "GTQ", Text = "GTQ - Quetzales" },
                new SelectListItem { Value = "EUR", Text = "EUR - Euros" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
            };

            return View();
        }

        // --- GUARDAR FACTURACIÓN (POST) ---
        [HttpPost]
        public IActionResult Guardar(FacturacionModel oFacturacion)
        {
            if (ModelState.IsValid)
            {
                // Si no se proporciona hora, usar la hora actual
                if (oFacturacion.HoraEmision == null)
                {
                    oFacturacion.HoraEmision = DateTime.Now.TimeOfDay;
                }

                var respuesta = _FacturacionData.MtdAgregarFacturacion(oFacturacion);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Boletos = _FacturacionData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
                new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                new SelectListItem { Value = "Cheque", Text = "Cheque" }
            };

            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Value = "USD", Text = "USD - Dólares" },
                new SelectListItem { Value = "GTQ", Text = "GTQ - Quetzales" },
                new SelectListItem { Value = "EUR", Text = "EUR - Euros" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
            };

            return View(oFacturacion);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        public IActionResult Modificar(int CodigoFactura)
        {
            var oFacturacion = _FacturacionData.MtdBuscarFacturacion(CodigoFactura);

            ViewBag.Boletos = _FacturacionData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.TiposPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
                new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                new SelectListItem { Value = "Cheque", Text = "Cheque" }
            };

            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Value = "USD", Text = "USD - Dólares" },
                new SelectListItem { Value = "GTQ", Text = "GTQ - Quetzales" },
                new SelectListItem { Value = "EUR", Text = "EUR - Euros" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Anulado", Text = "Anulado" }
            };

            return View(oFacturacion);
        }

        // --- MODIFICAR FACTURACIÓN (POST) ---
        [HttpPost]
        public IActionResult Modificar(FacturacionModel oFacturacion)
        {
            var respuesta = _FacturacionData.MtdEditarFacturacion(oFacturacion);

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
        public IActionResult Eliminar(int CodigoFactura)
        {
            var facturacion = _FacturacionData.MtdBuscarFacturacion(CodigoFactura);

            if (facturacion == null || facturacion.IdFactura == 0)
            {
                TempData["Error"] = "La factura no existe o ya fue eliminada.";
                return RedirectToAction("Listar");
            }

            return View(facturacion);
        }

        // --- ELIMINAR FACTURACIÓN (POST) ---
        [HttpPost]
        public IActionResult Eliminar(FacturacionModel oFacturacion)
        {
            try
            {
                bool respuesta = _FacturacionData.MtdEliminarFacturacion(oFacturacion.IdFactura);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Factura eliminada correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar la factura.");
                    return View(oFacturacion);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oFacturacion);
            }
        }
    }
}

