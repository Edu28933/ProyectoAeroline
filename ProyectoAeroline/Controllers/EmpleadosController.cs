using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Models;
using ProyectoAeroline.Data;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using System.IO; // Necesario para Path y FileStream
using System.Threading.Tasks; // Necesario para usar async/await
using ProyectoAeroline.Services;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {

        // Instancia de la clase con la conexion y stored procedures
        EmpleadosData _EmpleadosData = new EmpleadosData();

        // ----------------------------------------------------
        // CAMBIO 1: Declarar y obtener IWebHostEnvironment
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        // CAMBIO 2: Constructor para inyectar IWebHostEnvironment
        public EmpleadosController(IWebHostEnvironment webHostEnvironment, IPdfService pdfService, IEmailService emailService)
        {
            _webHostEnvironment = webHostEnvironment;
            _pdfService = pdfService;
            _emailService = emailService;
        }
        // ----------------------------------------------------


        // Muestra el formulario principal con la lista de datos
        [RequirePermission("Empleados", "Ver")]
        public IActionResult Listar()
        {

            var oListaEmpleados = _EmpleadosData.MtdConsultarEmpleados();
            return View(oListaEmpleados);
        }



        // Muestra el formulario llamador Guardar
        [RequirePermission("Empleados", "Crear")]
        public IActionResult Guardar()
        {
            ViewBag.Usuarios = _EmpleadosData.MtdListarUsuariosActivos();
            return View();
        }
        
        // Almacena los datos del formulario Guardar
        [HttpPost]
        [RequirePermission("Empleados", "Crear")]
        public IActionResult Guardar(EmpleadosModel oEmpleados)
        {
            // 1️⃣ Verificar si se subió una foto
            if (oEmpleados.FotoArchivo != null && oEmpleados.FotoArchivo.Length > 0)
            {
                // Ruta base: wwwroot/img/empleados
                string carpetaFotos = Path.Combine(_webHostEnvironment.WebRootPath, "img", "Empleados");

                // Crear carpeta si no existe
                if (!Directory.Exists(carpetaFotos))
                {
                    Directory.CreateDirectory(carpetaFotos);
                }

                // Generar nombre único
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(oEmpleados.FotoArchivo.FileName);
                string rutaArchivo = Path.Combine(carpetaFotos, nombreArchivo);

                // Guardar archivo en disco
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    oEmpleados.FotoArchivo.CopyTo(stream);
                }

                // Guardar la ruta relativa en la BD
                oEmpleados.FotoRuta = "/img/Empleados/" + nombreArchivo;
            }

            // 2️⃣ Llamar al método Data para guardar el empleado
            var respuesta = _EmpleadosData.MtdAgregarEmpleado(oEmpleados);

            // 3️⃣ Redirigir según resultado
            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                // Recargar combos si hay error
                ViewBag.Usuarios = _EmpleadosData.MtdListarUsuariosActivos();
                return View(oEmpleados);
            }
        }


        // Muestra el formulario llamador Modificar
        [RequirePermission("Empleados", "Editar")]
        public IActionResult Modificar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            ViewBag.Usuarios = _EmpleadosData.MtdListarUsuariosActivos();
            return View(oEmpleado);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        [RequirePermission("Empleados", "Editar")]
        public IActionResult Modificar(EmpleadosModel oEmpleado)
        {
            // 🆕 Manejo de la foto si se sube una nueva
            if (oEmpleado.FotoArchivo != null && oEmpleado.FotoArchivo.Length > 0)
            {
                string carpetaFotos = Path.Combine(_webHostEnvironment.WebRootPath, "img", "Empleados");

                // Crear carpeta si no existe
                if (!Directory.Exists(carpetaFotos))
                {
                    Directory.CreateDirectory(carpetaFotos);
                }

                // Generar nombre único
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(oEmpleado.FotoArchivo.FileName);
                string rutaArchivo = Path.Combine(carpetaFotos, nombreArchivo);

                // Guardar la nueva foto en disco
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    oEmpleado.FotoArchivo.CopyTo(stream);
                }

                // Actualizar ruta en BD
                oEmpleado.FotoRuta = "/img/Empleados/" + nombreArchivo;
            }

            var respuesta = _EmpleadosData.MtdEditarEmpleado(oEmpleado);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                // Recargar combos si hay error
                ViewBag.Usuarios = _EmpleadosData.MtdListarUsuariosActivos();
                return View(oEmpleado);
            }
        }


        // Muestra el formulario llamador Eliminar
        // GET: Empleados/Eliminar/5
        [RequirePermission("Empleados", "Eliminar")]
        public IActionResult Eliminar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            return View(oEmpleado);
        }

        // POST: Empleados/Eliminar
        [HttpPost]
        [RequirePermission("Empleados", "Eliminar")]
        public IActionResult Eliminar(EmpleadosModel oEmpleado)
        {
            var respuesta = _EmpleadosData.MtdEliminarEmpleado(oEmpleado.IdEmpleado);

            if (respuesta)
            {
                TempData["Success"] = "Empleado eliminado correctamente.";
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el empleado. Por favor, verifique que su estado sea 'Inactivo'.";
                var empleadoActualizado = _EmpleadosData.MtdBuscarEmpleado(oEmpleado.IdEmpleado);
                return View(empleadoActualizado ?? oEmpleado);
            }
        }

        // --- GENERAR PDF Y MOSTRAR VISTA ---
        [HttpGet]
        [RequirePermission("Empleados", "Ver")]
        public IActionResult GenerarPerfil(int idEmpleado)
        {
            var empleado = _EmpleadosData.MtdBuscarEmpleado(idEmpleado);
            
            if (empleado == null || empleado.IdEmpleado == 0)
            {
                TempData["Error"] = "No se encontró el empleado especificado.";
                return RedirectToAction("Listar");
            }

            return View("VerPDF", empleado);
        }

        // --- DESCARGAR PDF ---
        [HttpGet]
        [RequirePermission("Empleados", "Ver")]
        public IActionResult DescargarPerfil(int idEmpleado)
        {
            var empleado = _EmpleadosData.MtdBuscarEmpleado(idEmpleado);
            
            if (empleado == null || empleado.IdEmpleado == 0)
            {
                TempData["Error"] = "No se encontró el empleado especificado.";
                return RedirectToAction("Listar");
            }

            var pdfBytes = _pdfService.GenerarPdfEmpleado(empleado);
            var fileName = $"Perfil_Empleado_{empleado.Nombre?.Replace(" ", "_")}_{idEmpleado}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            // Forzar descarga del PDF
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            return File(pdfBytes, "application/pdf", fileName);
        }

        // --- ENVIAR PERFIL POR EMAIL ---
        [HttpPost]
        [RequirePermission("Empleados", "Ver")]
        public async Task<IActionResult> EnviarPerfil(int idEmpleado)
        {
            var empleado = _EmpleadosData.MtdBuscarEmpleado(idEmpleado);
            
            if (empleado == null || empleado.IdEmpleado == 0)
            {
                TempData["Error"] = "No se encontró el empleado especificado.";
                return RedirectToAction("GenerarPerfil", new { idEmpleado });
            }

            // Validar que el empleado tenga correo electrónico
            if (string.IsNullOrWhiteSpace(empleado.Correo))
            {
                TempData["Error"] = "El empleado no tiene un correo electrónico registrado.";
                return RedirectToAction("GenerarPerfil", new { idEmpleado });
            }

            try
            {
                // Generar PDF
                var pdfBytes = _pdfService.GenerarPdfEmpleado(empleado);
                var fileName = $"Perfil_Empleado_{empleado.Nombre?.Replace(" ", "_")}_{idEmpleado}.pdf";

                // Crear HTML del email
                var emailBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                            .content {{ background-color: #f8f9fa; padding: 20px; }}
                            .footer {{ text-align: center; padding: 20px; color: #6c757d; }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <h1>👤 Perfil de Empleado</h1>
                            </div>
                            <div class=""content"">
                                <p>Estimado/a <strong>{empleado.Nombre}</strong>,</p>
                                <p>Este es su perfil de empleado. Por favor, encuentre el documento PDF adjunto con toda su información.</p>
                                <p><strong>Información del Empleado:</strong></p>
                                <ul>
                                    <li><strong>ID:</strong> {empleado.IdEmpleado}</li>
                                    <li><strong>Cargo:</strong> {empleado.Cargo ?? "N/A"}</li>
                                    <li><strong>Estado:</strong> {empleado.Estado ?? "N/A"}</li>
                                </ul>
                                <p>Por favor, conserve este documento para sus registros personales.</p>
                            </div>
                            <div class=""footer"">
                                <p>Proyecto Aerolínea - Recursos Humanos</p>
                                <p>Este es un correo automático, por favor no responda.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                // Enviar email con adjunto (usando el correo del empleado)
                await _emailService.SendWithAttachmentAsync(
                    empleado.Correo,
                    $"Perfil de Empleado - {empleado.Nombre}",
                    emailBody,
                    pdfBytes,
                    fileName
                );

                TempData["Success"] = $"Perfil enviado correctamente a {empleado.Correo}";
                return RedirectToAction("GenerarPerfil", new { idEmpleado });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al enviar el correo: {ex.Message}";
                return RedirectToAction("GenerarPerfil", new { idEmpleado });
            }
        }

    }
}
