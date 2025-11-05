using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Attributes;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class AeropuertosController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        AeropuertosData _AeropuertosData = new AeropuertosData();


        // Muestra el formulario principal con la lista de datos
        [RequirePermission("Aeropuertos", "Ver")]
        public IActionResult Listar()
        {

            var oListaAeropuertos = _AeropuertosData.MtdConsultarAeropuertos();
            return View(oListaAeropuertos);
        }



        // Muestra el formulario llamador Guardar
        [RequirePermission("Aeropuertos", "Crear")]
        public IActionResult Guardar()
        {
            //Cargar Aerolíneas Activas
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre");


            //No generar IATA automáticamente, se generará según el país seleccionado
            var oAeropuerto = new AeropuertosModel();
            oAeropuerto.IATA = "";

            //Cargar Listas Auxiliares
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = new List<string>();
            return View(oAeropuerto);
            //return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        [RequirePermission("Aeropuertos", "Crear")]
        public IActionResult Guardar(AeropuertosModel oAeropuertos)
        {
            // Asignar valores automáticos si no están establecidos
            if (string.IsNullOrEmpty(oAeropuertos.IATA) && !string.IsNullOrEmpty(oAeropuertos.Pais))
            {
                oAeropuertos.IATA = ObtenerCodigoIATAPorPais(oAeropuertos.Pais);
            }
            if (string.IsNullOrEmpty(oAeropuertos.Nombre) && !string.IsNullOrEmpty(oAeropuertos.Ciudad))
            {
                oAeropuertos.Nombre = ObtenerNombreAeropuertoPorCiudad(oAeropuertos.Ciudad);
            }
            if (string.IsNullOrEmpty(oAeropuertos.Direccion) && !string.IsNullOrEmpty(oAeropuertos.Pais) && !string.IsNullOrEmpty(oAeropuertos.Ciudad))
            {
                oAeropuertos.Direccion = ObtenerDireccionPorPaisYCiudad(oAeropuertos.Pais, oAeropuertos.Ciudad);
            }
            if (oAeropuertos.Telefono == null && !string.IsNullOrEmpty(oAeropuertos.Pais))
            {
                oAeropuertos.Telefono = ObtenerTelefonoPorPais(oAeropuertos.Pais);
            }

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
        [RequirePermission("Aeropuertos", "Editar")]
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
        [RequirePermission("Aeropuertos", "Editar")]
        public IActionResult Modificar(AeropuertosModel oAeropuerto)
        {
            // Asignar valores automáticos si no están establecidos o si cambió el país/ciudad
            if (!string.IsNullOrEmpty(oAeropuerto.Pais))
            {
                // Siempre actualizar IATA si hay país
                oAeropuerto.IATA = ObtenerCodigoIATAPorPais(oAeropuerto.Pais);
                
                // Actualizar teléfono si hay país
                if (oAeropuerto.Telefono == null)
                {
                    oAeropuerto.Telefono = ObtenerTelefonoPorPais(oAeropuerto.Pais);
                }
            }
            if (!string.IsNullOrEmpty(oAeropuerto.Ciudad))
            {
                // Siempre actualizar nombre del aeropuerto si hay ciudad
                oAeropuerto.Nombre = ObtenerNombreAeropuertoPorCiudad(oAeropuerto.Ciudad);
            }
            if (!string.IsNullOrEmpty(oAeropuerto.Pais) && !string.IsNullOrEmpty(oAeropuerto.Ciudad))
            {
                // Siempre actualizar dirección si hay país y ciudad
                oAeropuerto.Direccion = ObtenerDireccionPorPaisYCiudad(oAeropuerto.Pais, oAeropuerto.Ciudad);
            }

            var respuesta = _AeropuertosData.MtdEditarAeropuerto(oAeropuerto);

            if (respuesta)
                return RedirectToAction("Listar");
            
            // Si falla, recargar combos
            var empleados = _AeropuertosData.MtdObtenerEmpleados();
            ViewBag.Empleados = new SelectList(empleados, "IdEmpleado", "Nombre", oAeropuerto.IdEmpleado);

            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAeropuerto.Pais) ?? new List<string>();

            return View(oAeropuerto);
        }

        // Método JSON para Ajax: obtener ciudades por país
        [RequirePermission("Aeropuertos", "Ver")]
        public JsonResult ObtenerCiudades(string pais)
        {
            var ciudades = ObtenerCiudadesPorPais(pais);
            return Json(ciudades);
        }

        // Método JSON para Ajax: obtener código IATA según país
        [RequirePermission("Aeropuertos", "Ver")]
        public JsonResult ObtenerCodigoIATA(string pais)
        {
            var iata = ObtenerCodigoIATAPorPais(pais);
            return Json(new { iata = iata });
        }

        // Método JSON para Ajax: obtener dirección según país y ciudad
        [RequirePermission("Aeropuertos", "Ver")]
        public JsonResult ObtenerDireccion(string pais, string ciudad)
        {
            var direccion = ObtenerDireccionPorPaisYCiudad(pais, ciudad);
            return Json(new { direccion = direccion });
        }

        // Método JSON para Ajax: obtener teléfono según país
        [RequirePermission("Aeropuertos", "Ver")]
        public JsonResult ObtenerTelefono(string pais)
        {
            var telefono = ObtenerTelefonoPorPais(pais);
            return Json(new { telefono = telefono });
        }

        // Método JSON para Ajax: obtener nombre del aeropuerto según ciudad
        [RequirePermission("Aeropuertos", "Ver")]
        public JsonResult ObtenerNombreAeropuerto(string ciudad)
        {
            var nombre = ObtenerNombreAeropuertoPorCiudad(ciudad);
            return Json(new { nombre = nombre });
        }

        // ------------------ Métodos auxiliares ------------------

        // Obtener código IATA real según el país seleccionado
        private string ObtenerCodigoIATAPorPais(string? pais)
        {
            if (string.IsNullOrEmpty(pais))
                return "";

            // Códigos IATA reales de aeropuertos principales por país (Centroamérica)
            var codigosIATA = new Dictionary<string, string>
            {
                { "Guatemala", "GUA" },      // Aeropuerto Internacional La Aurora
                { "Belice", "BZE" },         // Aeropuerto Internacional Philip S. W. Goldson
                { "Honduras", "TGU" },       // Aeropuerto Internacional Toncontín (Tegucigalpa)
                { "El Salvador", "SAL" },    // Aeropuerto Internacional de El Salvador
                { "Nicaragua", "MGA" },      // Aeropuerto Internacional Augusto C. Sandino (Managua)
                { "Costa Rica", "SJO" },     // Aeropuerto Internacional Juan Santamaría (San José)
                { "Panamá", "PTY" }          // Aeropuerto Internacional de Tocumen
            };

            return codigosIATA.TryGetValue(pais, out string? codigo) ? codigo : "";
        }

        // Obtener dirección automática según país y ciudad
        private string ObtenerDireccionPorPaisYCiudad(string? pais, string? ciudad)
        {
            if (string.IsNullOrEmpty(pais) || string.IsNullOrEmpty(ciudad))
                return "";

            // Diccionario de direcciones por país y ciudad
            var direcciones = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "Guatemala", new Dictionary<string, string>
                    {
                        { "Ciudad de Guatemala", "Km 6.5, Zona 13, Ciudad de Guatemala" },
                        { "Quetzaltenango", "Boulevard Alameda, Zona 3, Quetzaltenango" },
                        { "Antigua", "3ra Avenida Norte #3, Antigua Guatemala" }
                    }
                },
                {
                    "Belice", new Dictionary<string, string>
                    {
                        { "Belmopán", "Ring Road, Belmopán" },
                        { "Ciudad de Belice", "Northern Highway, Ladyville, Ciudad de Belice" }
                    }
                },
                {
                    "Honduras", new Dictionary<string, string>
                    {
                        { "Tegucigalpa", "Boulevard Toncontín, Tegucigalpa" },
                        { "San Pedro Sula", "Carretera al Aeropuerto, San Pedro Sula" }
                    }
                },
                {
                    "El Salvador", new Dictionary<string, string>
                    {
                        { "San Salvador", "Km 40.5 Carretera al Aeropuerto, San Luis Talpa" },
                        { "Santa Ana", "Calle Libertad, Santa Ana" }
                    }
                },
                {
                    "Nicaragua", new Dictionary<string, string>
                    {
                        { "Managua", "Km 11 Carretera Norte, Managua" },
                        { "León", "Avenida Central, León" }
                    }
                },
                {
                    "Costa Rica", new Dictionary<string, string>
                    {
                        { "San José", "Alajuela, San José" },
                        { "Alajuela", "Aeropuerto Juan Santamaría, Alajuela" }
                    }
                },
                {
                    "Panamá", new Dictionary<string, string>
                    {
                        { "Ciudad de Panamá", "Avenida Domingo Díaz, Ciudad de Panamá" },
                        { "Colón", "Avenida Central, Colón" }
                    }
                }
            };

            if (direcciones.TryGetValue(pais, out var ciudadesDict) && ciudadesDict.TryGetValue(ciudad, out var direccion))
            {
                return direccion;
            }

            return $"Aeropuerto Principal, {ciudad}, {pais}";
        }

        // Obtener teléfono automático según país
        private int? ObtenerTelefonoPorPais(string? pais)
        {
            if (string.IsNullOrEmpty(pais))
                return null;

            // Teléfonos definidos por país (solo números principales, sin guiones)
            var telefonos = new Dictionary<string, int>
            {
                { "Guatemala", 23263300 },
                { "Belice", 2252043 },
                { "Honduras", 22332000 },
                { "El Salvador", 22331700 },
                { "Nicaragua", 22316300 },
                { "Costa Rica", 22432600 },
                { "Panamá", 2076700 }
            };

            return telefonos.TryGetValue(pais, out int telefono) ? telefono : null;
        }

        // Obtener nombre del aeropuerto automático según ciudad
        private string ObtenerNombreAeropuertoPorCiudad(string? ciudad)
        {
            if (string.IsNullOrEmpty(ciudad))
                return "";

            // Diccionario de nombres de aeropuertos por ciudad
            var nombresAeropuertos = new Dictionary<string, string>
            {
                { "Ciudad de Guatemala", "Aeropuerto Internacional La Aurora" },
                { "Quetzaltenango", "Aeropuerto de Quetzaltenango" },
                { "Antigua", "Aeropuerto de Antigua Guatemala" },
                { "Belmopán", "Aeropuerto Internacional de Belmopán" },
                { "Ciudad de Belice", "Aeropuerto Internacional Philip S. W. Goldson" },
                { "Tegucigalpa", "Aeropuerto Internacional Toncontín" },
                { "San Pedro Sula", "Aeropuerto Internacional Ramón Villeda Morales" },
                { "San Salvador", "Aeropuerto Internacional de El Salvador" },
                { "Santa Ana", "Aeropuerto Internacional de Santa Ana" },
                { "Managua", "Aeropuerto Internacional Augusto C. Sandino" },
                { "León", "Aeropuerto Internacional de León" },
                { "San José", "Aeropuerto Internacional Juan Santamaría" },
                { "Alajuela", "Aeropuerto Internacional Juan Santamaría" },
                { "Ciudad de Panamá", "Aeropuerto Internacional de Tocumen" },
                { "Colón", "Aeropuerto Internacional de Colón" }
            };

            return nombresAeropuertos.TryGetValue(ciudad, out string? nombre) ? nombre : $"Aeropuerto Internacional de {ciudad}";
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
        [RequirePermission("Aeropuertos", "Eliminar")]
        public IActionResult Eliminar(int CodigoAeropuerto)
        {
            var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);
            return View(oAeropuerto);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        [RequirePermission("Aeropuertos", "Eliminar")]
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
