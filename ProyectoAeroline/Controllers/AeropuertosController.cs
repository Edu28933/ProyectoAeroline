using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class AeropuertosController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        AeropuertosData _AeropuertosData = new AeropuertosData();


        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaAeropuertos = _AeropuertosData.MtdConsultarAeropuertos();
            return View(oListaAeropuertos);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            //Cargar Aerolíneas Activas
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre");


            //Generar código IATA
            var oAeropuerto = new AeropuertosModel();
            oAeropuerto.IATA = GenerarIATA();

            //Cargar Listas Auxiliares
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = new List<string>();
            return View(oAeropuerto);
            //return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(AeropuertosModel oAeropuertos)
        {
            var respuesta = _AeropuertosData.MtdAgregarAeropuerto(oAeropuertos);

            if (respuesta)
                return RedirectToAction("Listar");

            // Recargar combos si falla
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre");


            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAeropuertos.Pais) ?? new List<string>();

            return View(oAeropuertos);
        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoAeropuerto)
        {
            // CBOX PARA PODER LLAMAR A LA BDD POR MEDIO DE UN USP Y LLENARSE CON LOS EMPLEADOS ACTIVOS

            var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre", oAeropuerto.IdEmpleado);

           // var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);

            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAeropuerto.Pais) ?? new List<string>();

            return View(oAeropuerto);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(AeropuertosModel oAeropuerto)
        {

            var respuesta = _AeropuertosData.MtdEditarAeropuerto(oAeropuerto);

            if (respuesta)
                return RedirectToAction("Listar");
            // Si falla, recargar combos
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre", oAeropuerto.IdEmpleado);

            // Si falla, recargar combos
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAeropuerto.Pais) ?? new List<string>();

            return View(oAeropuerto);
        }

        // Método JSON para Ajax: obtener ciudades por país
        public JsonResult ObtenerCiudades(string pais)
        {
            var ciudades = ObtenerCiudadesPorPais(pais);
            return Json(ciudades);
        }

        // ------------------ Métodos auxiliares ------------------

        private string GenerarIATA()
        {
            var letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rnd = new Random();
            return new string(Enumerable.Range(0, 3).Select(x => letras[rnd.Next(letras.Length)]).ToArray());
        }

        private List<string> ObtenerPaises() => new List<string>
        {
            "Guatemala", "Belice", "Honduras", "El Salvador", "Nicaragua", "Costa Rica", "Panamá"
        };

        private List<string> ObtenerEstados() => new List<string>
        {
            "Activo", "Inactivo"
        };

        private List<string> ObtenerCiudadesPorPais(string? pais)
        {

            var lista = new List<string>();

            if (string.IsNullOrEmpty(pais))
                return lista;

            switch (pais)
            {
                case "Guatemala":
                    lista = new List<string> { "Ciudad de Guatemala", "Quetzaltenango", "Antigua" };
                    break;
                case "Belice":
                    lista = new List<string> { "Belmopán", "Ciudad de Belice" };
                    break;
                case "Honduras":
                    lista = new List<string> { "Tegucigalpa", "San Pedro Sula" };
                    break;
                case "El Salvador":
                    lista = new List<string> { "San Salvador", "Santa Ana" };
                    break;
                case "Nicaragua":
                    lista = new List<string> { "Managua", "León" };
                    break;
                case "Costa Rica":
                    lista = new List<string> { "San José", "Alajuela" };
                    break;
                case "Panamá":
                    lista = new List<string> { "Ciudad de Panamá", "Colón" };
                    break;
            }

            return lista;
        }



        // Muestra el formulario llamador Eliminar
        // GET: Empleados/Eliminar/5
        public IActionResult Eliminar(int CodigoAeropuerto)
        {
            var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);
            return View(oAeropuerto);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(AeropuertosModel oAeropuerto)
        {
            var respuesta = _AeropuertosData.MtdEliminarAeropuerto(oAeropuerto.IdAeropuerto);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }
    }
}
