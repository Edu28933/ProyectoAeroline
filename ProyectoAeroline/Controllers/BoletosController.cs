using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using ProyectoAeroline.Services;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class BoletosController : Controller
    {
        // Instancia de la clase con la conexión y stored procedures
        BoletosData _BoletosData = new BoletosData();
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public BoletosController(IPdfService pdfService, IEmailService emailService, IConfiguration configuration)
        {
            _pdfService = pdfService;
            _emailService = emailService;
            _configuration = configuration;
        }

        // --- LISTAR BOLETOS ---
        [RequirePermission("Boletos", "Ver")]
        public IActionResult Listar()
        {
            var oListaBoletos = _BoletosData.MtdConsultarBoletos();
            return View(oListaBoletos);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        [RequirePermission("Boletos", "Crear")]
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
        [RequirePermission("Boletos", "Crear")]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(BoletosModel oBoleto)
        {
            // Validaciones básicas
            if (oBoleto.IdVuelo == 0)
            {
                ModelState.AddModelError("IdVuelo", "Debe seleccionar un vuelo.");
            }

            if (oBoleto.IdPasajero == 0)
            {
                ModelState.AddModelError("IdPasajero", "Debe seleccionar un pasajero.");
            }

            if (string.IsNullOrWhiteSpace(oBoleto.Estado))
            {
                ModelState.AddModelError("Estado", "Debe seleccionar un estado.");
            }

            if (oBoleto.Precio <= 0)
            {
                ModelState.AddModelError("Precio", "El precio debe ser mayor a cero.");
            }

            if (oBoleto.Total <= 0)
            {
                ModelState.AddModelError("Total", "El total debe ser mayor a cero.");
            }

            if (oBoleto.FechaCompra == default(DateTime))
            {
                ModelState.AddModelError("FechaCompra", "Debe ingresar una fecha de compra válida.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var respuesta = _BoletosData.MtdAgregarBoleto(oBoleto);
                    if (respuesta)
                    {
                        TempData["Success"] = "Boleto guardado correctamente.";
                        return RedirectToAction("Listar");
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo guardar el boleto. Verifique los datos e intente nuevamente.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al guardar el boleto: {ex.Message}";
                    // Log del error para debugging
                    System.Diagnostics.Debug.WriteLine($"Error al guardar boleto: {ex}");
                }
            }
            else
            {
                // Si hay errores de validación, mostrar mensaje
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Por favor, corrija los siguientes errores: " + string.Join(", ", errores);
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
        [RequirePermission("Boletos", "Editar")]
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
        [RequirePermission("Boletos", "Editar")]
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
        [RequirePermission("Boletos", "Eliminar")]
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
        [RequirePermission("Boletos", "Eliminar")]
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
        [RequirePermission("Boletos", "Ver")]
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
        [RequirePermission("Boletos", "Ver")]
        public JsonResult ObtenerTipoPasajero(int idPasajero)
        {
            var tipoPasajero = _BoletosData.MtdObtenerTipoPasajero(idPasajero);
            return Json(new { tipoPasajero = tipoPasajero ?? "" });
        }

        [HttpGet]
        [RequirePermission("Boletos", "Ver")]
        public JsonResult ObtenerCapacidadAvion(int idVuelo)
        {
            var capacidad = _BoletosData.MtdObtenerCapacidadAvionPorVuelo(idVuelo);
            return Json(new { capacidad = capacidad });
        }

        [HttpGet]
        [RequirePermission("Boletos", "Ver")]
        public JsonResult ObtenerAsientosOcupados(int idVuelo, int? idBoletoExcluir = null)
        {
            var asientosOcupados = _BoletosData.MtdObtenerAsientosOcupados(idVuelo, idBoletoExcluir);
            return Json(new { asientosOcupados = asientosOcupados });
        }

        [HttpGet]
        [RequirePermission("Boletos", "Ver")]
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

        // --- GENERAR PDF Y MOSTRAR VISTA ---
        [HttpGet]
        [RequirePermission("Boletos", "Ver")]
        public IActionResult GenerarBoleto(int idBoleto)
        {
            var boletoInfo = _BoletosData.MtdObtenerBoletoCompleto(idBoleto);
            
            if (boletoInfo == null)
            {
                TempData["Error"] = "No se encontró el boleto especificado.";
                return RedirectToAction("Listar");
            }

            return View("VerPDF", new VerBoletoViewModel 
            { 
                IdBoleto = idBoleto,
                Boleto = boletoInfo.Boleto,
                Vuelo = boletoInfo.Vuelo,
                Pasajero = boletoInfo.Pasajero
            });
        }

        // --- DESCARGAR PDF ---
        [HttpGet]
        [RequirePermission("Boletos", "Ver")]
        public IActionResult DescargarBoleto(int idBoleto)
        {
            var boletoInfo = _BoletosData.MtdObtenerBoletoCompleto(idBoleto);
            
            if (boletoInfo == null)
            {
                TempData["Error"] = "No se encontró el boleto especificado.";
                return RedirectToAction("Listar");
            }

            var pdfBytes = _pdfService.GenerarPdfBoleto(boletoInfo);
            var fileName = $"Boleto_{idBoleto}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            // Forzar descarga del PDF
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(pdfBytes, "application/pdf", fileName);
        }

        // --- ENVIAR BOLETO POR EMAIL ---
        [HttpPost]
        [RequirePermission("Boletos", "Ver")]
        public async Task<IActionResult> EnviarBoleto(int idBoleto, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Por favor, ingrese un correo electrónico válido.";
                return RedirectToAction("GenerarBoleto", new { idBoleto });
            }

            var boletoInfo = _BoletosData.MtdObtenerBoletoCompleto(idBoleto);
            
            if (boletoInfo == null)
            {
                TempData["Error"] = "No se encontró el boleto especificado.";
                return RedirectToAction("Listar");
            }

            try
            {
                // Generar PDF
                var pdfBytes = _pdfService.GenerarPdfBoleto(boletoInfo);
                var fileName = $"Boleto_{idBoleto}.pdf";

                // Generar token de confirmación (usando un GUID simple)
                var confirmacionToken = Guid.NewGuid().ToString("N");
                
                // Guardar token en sesión o base de datos (por simplicidad, usaremos el idBoleto)
                // En producción, deberías guardar esto en una tabla de tokens con expiración
                HttpContext.Session.SetString($"BoletoToken_{idBoleto}", confirmacionToken);

                // URL de confirmación
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var confirmarUrl = $"{baseUrl}/Boletos/ConfirmarBoleto?idBoleto={idBoleto}&token={confirmacionToken}";

                // Crear HTML del email
                var emailBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                            .content {{ background-color: #f8f9fa; padding: 20px; }}
                            .button {{ background-color: #28a745; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; display: inline-block; margin-top: 20px; }}
                            .footer {{ text-align: center; padding: 20px; color: #6c757d; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h1>🎫 Boleto de Vuelo</h1>
                            </div>
                            <div class=""content"">
                                <p>Estimado/a {boletoInfo.Pasajero?.Nombres} {boletoInfo.Pasajero?.Apellidos},</p>
                                <p>Le enviamos su boleto de vuelo adjunto. Para confirmar su boleto, por favor haga clic en el siguiente botón:</p>
                                <p style=""text-align: center;"">
                                    <a href=""{confirmarUrl}"" class=""button"">✅ Confirmar Boleto</a>
                                </p>
                                <p>O copie y pegue el siguiente enlace en su navegador:</p>
                                <p><a href=""{confirmarUrl}"">{confirmarUrl}</a></p>
                                <p><strong>ID de Boleto:</strong> #{idBoleto}</p>
                                <p><strong>Vuelo:</strong> {boletoInfo.Vuelo?.NumeroVuelo}</p>
                                <p><strong>Origen:</strong> {boletoInfo.Vuelo?.AeropuertoOrigen}</p>
                                <p><strong>Destino:</strong> {boletoInfo.Vuelo?.AeropuertoDestino}</p>
                            </div>
                            <div class=""footer"">
                                <p>Gracias por elegir nuestra aerolínea. ¡Buen viaje!</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                // Enviar email con adjunto
                await _emailService.SendWithAttachmentAsync(
                    email,
                    $"Boleto de Vuelo #{idBoleto}",
                    emailBody,
                    pdfBytes,
                    fileName
                );

                TempData["Success"] = $"Boleto enviado correctamente a {email}";
                return RedirectToAction("GenerarBoleto", new { idBoleto });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al enviar el correo: {ex.Message}";
                return RedirectToAction("GenerarBoleto", new { idBoleto });
            }
        }

        // --- CONFIRMAR BOLETO DESDE EMAIL ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmarBoleto(int idBoleto, string token)
        {
            // Validar que el token no esté vacío (seguridad básica)
            if (string.IsNullOrWhiteSpace(token) || token.Length < 10)
            {
                ViewBag.Error = "Enlace de confirmación inválido o expirado.";
                return View("ConfirmarBoletoResult");
            }

            // Validar que el boleto existe
            var boleto = _BoletosData.MtdBuscarBoleto(idBoleto);
            
            if (boleto == null || boleto.IdBoleto == 0)
            {
                ViewBag.Error = "El boleto no existe o ya fue eliminado.";
                return View("ConfirmarBoletoResult");
            }

            // Validar que el boleto está en estado Pendiente
            if (boleto.Estado != "Pendiente")
            {
                ViewBag.Error = $"El boleto ya está {boleto.Estado} y no puede ser confirmado nuevamente.";
                ViewBag.Boleto = boleto;
                return View("ConfirmarBoletoResult");
            }

            // Confirmar el boleto
            var confirmado = _BoletosData.MtdConfirmarBoleto(idBoleto);
            
            if (confirmado)
            {
                ViewBag.Success = true;
                ViewBag.Message = "¡Boleto confirmado exitosamente!";
                ViewBag.Boleto = _BoletosData.MtdBuscarBoleto(idBoleto); // Recargar para obtener estado actualizado
            }
            else
            {
                ViewBag.Error = "No se pudo confirmar el boleto. Por favor, contacte con el soporte de la aerolínea.";
                ViewBag.Boleto = boleto;
            }

            // IMPORTANTE: Esta es una página pública sin acceso al sistema
            // No redirigir a ninguna página protegida
            return View("ConfirmarBoletoResult");
        }

    }

    // ViewModel para la vista VerPDF
    public class VerBoletoViewModel
    {
        public int IdBoleto { get; set; }
        public BoletosModel? Boleto { get; set; }
        public VuelosModel? Vuelo { get; set; }
        public PasajerosModel? Pasajero { get; set; }
    }
}
