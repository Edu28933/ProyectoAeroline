using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Attributes;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class FacturacionController : Controller
    {
        FacturacionData _FacturacionData = new FacturacionData();

        // --- LISTAR FACTURACIÓN ---
        [RequirePermission("Facturacion", "Ver")]
        public IActionResult Listar()
        {
            var oListaFacturacion = _FacturacionData.MtdConsultarFacturacion();
            return View(oListaFacturacion);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        [RequirePermission("Facturacion", "Crear")]
        public IActionResult Guardar()
        {
            ViewBag.Boletos = _FacturacionData.MtdListarBoletosConPasajero();

            ViewBag.TiposPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
                new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" }
            };

            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Value = "GTQ", Text = "GTQ" },
                new SelectListItem { Value = "EURO", Text = "EURO" },
                new SelectListItem { Value = "USD", Text = "USD" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Cancelada", Text = "Cancelada" },
                new SelectListItem { Value = "Pagada", Text = "Pagada" }
            };

            return View();
        }

        // --- GUARDAR FACTURACIÓN (POST) ---
        [HttpPost]
        [RequirePermission("Facturacion", "Crear")]
        public IActionResult Guardar(FacturacionModel oFacturacion)
        {
            if (ModelState.IsValid && oFacturacion.IdBoleto > 0)
            {
                var respuesta = _FacturacionData.MtdAgregarFacturacion(oFacturacion);
                if (respuesta)
                {
                    TempData["Success"] = "Factura guardada correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "Error al guardar la factura. Por favor, intente nuevamente.");
                }
            }

            // Recargar combos si hay error
            CargarCombos();
            return View(oFacturacion);
        }

        // API para obtener datos del boleto (fecha, hora, monto, impuesto)
        [HttpGet]
        [RequirePermission("Facturacion", "Ver")]
        public JsonResult ObtenerDatosBoleto(int idBoleto)
        {
            if (idBoleto <= 0)
                return Json(new { error = "ID de boleto inválido" });

            var datos = _FacturacionData.MtdObtenerDatosBoleto(idBoleto);
            return Json(new
            {
                fechaEmision = datos.FechaCompra.ToString("yyyy-MM-dd"),
                horaEmision = datos.HoraCompra.ToString(@"hh\:mm\:ss"),
                monto = datos.Precio,
                impuesto = datos.Impuesto ?? 0,
                moneda = datos.Moneda,
                montoFactura = datos.Precio + (datos.Impuesto ?? 0),
                montoTotal = datos.Precio + (datos.Impuesto ?? 0)
            });
        }

        private void CargarCombos()
        {
            ViewBag.Boletos = _FacturacionData.MtdListarBoletosConPasajero();

            ViewBag.TiposPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Tarjeta de Crédito", Text = "Tarjeta de Crédito" },
                new SelectListItem { Value = "Tarjeta de Débito", Text = "Tarjeta de Débito" },
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" }
            };

            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Value = "GTQ", Text = "GTQ" },
                new SelectListItem { Value = "EURO", Text = "EURO" },
                new SelectListItem { Value = "USD", Text = "USD" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Cancelada", Text = "Cancelada" },
                new SelectListItem { Value = "Pagada", Text = "Pagada" }
            };
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        [RequirePermission("Facturacion", "Editar")]
        public IActionResult Modificar(int CodigoFactura)
        {
            var oFacturacion = _FacturacionData.MtdBuscarFacturacion(CodigoFactura);
            if (oFacturacion == null || oFacturacion.IdFactura == 0)
            {
                TempData["Error"] = "La factura no existe.";
                return RedirectToAction("Listar");
            }

            CargarCombos();
            return View(oFacturacion);
        }

        // --- MODIFICAR FACTURACIÓN (POST) ---
        [HttpPost]
        [RequirePermission("Facturacion", "Editar")]
        public IActionResult Modificar(FacturacionModel oFacturacion)
        {
            if (ModelState.IsValid)
            {
                var respuesta = _FacturacionData.MtdEditarFacturacion(oFacturacion);

                if (respuesta)
                {
                    TempData["Success"] = "Factura modificada correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "Error al modificar la factura.");
                }
            }

            CargarCombos();
            return View(oFacturacion);
        }

        // --- MOSTRAR FORMULARIO ELIMINAR ---
        [RequirePermission("Facturacion", "Eliminar")]
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
        [RequirePermission("Facturacion", "Eliminar")]
        public IActionResult Eliminar(FacturacionModel oFacturacion)
        {
            try
            {
                var resultado = _FacturacionData.MtdEliminarFacturacionValidado(oFacturacion.IdFactura);

                if (resultado.Success)
                {
                    TempData["Success"] = "Factura eliminada correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    TempData["Error"] = resultado.ErrorMessage;
                    return View(oFacturacion);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar: " + ex.Message;
                return View(oFacturacion);
            }
        }
    }
}

