using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Models;
using ProyectoAeroline.Data;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using System.IO; // Necesario para Path y FileStream
using System.Threading.Tasks; // Necesario para usar async/await

namespace ProyectoAeroline.Controllers
{
    public class EmpleadosController : Controller
    {

        // Instancia de la clase con la conexion y stored procedures
        EmpleadosData _EmpleadosData = new EmpleadosData();

        // ----------------------------------------------------
        // CAMBIO 1: Declarar y obtener IWebHostEnvironment
        private readonly IWebHostEnvironment _webHostEnvironment;

        // CAMBIO 2: Constructor para inyectar IWebHostEnvironment
        public EmpleadosController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        // ----------------------------------------------------


        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaEmpleados = _EmpleadosData.MtdConsultarEmpleados();
            return View(oListaEmpleados);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            return View();
        }
        
        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(EmpleadosModel oEmpleados)
        {
            // 1️⃣ Verificar si se subió una foto
            if (oEmpleados.FotoArchivo != null && oEmpleados.FotoArchivo.Length > 0)
            {
                // Ruta base: wwwroot/img/empleados
                string carpetaFotos = Path.Combine(_webHostEnvironment.WebRootPath, "img", "empleados");

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
                oEmpleados.FotoRuta = "/img/empleados/" + nombreArchivo;
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
                return View();
            }
        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            return View(oEmpleado);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(EmpleadosModel oEmpleado)
        {
            // 🆕 Manejo de la foto si se sube una nueva
            if (oEmpleado.FotoArchivo != null && oEmpleado.FotoArchivo.Length > 0)
            {
                string carpetaFotos = Path.Combine(_webHostEnvironment.WebRootPath, "img", "empleados");

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
                oEmpleado.FotoRuta = "/img/empleados/" + nombreArchivo;
            }

            var respuesta = _EmpleadosData.MtdEditarEmpleado(oEmpleado);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
            }
        }


        // Muestra el formulario llamador Eliminar
        // GET: Empleados/Eliminar/5
        public IActionResult Eliminar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            return View(oEmpleado);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(EmpleadosModel oEmpleado)
        {
            var respuesta = _EmpleadosData.MtdEliminarEmpleado(oEmpleado.IdEmpleado);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }


    }
}
