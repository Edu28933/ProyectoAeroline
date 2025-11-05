using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class AerolineasController : Controller
    {
        AerolineasData _AerolineasData = new AerolineasData();

        [RequirePermission("Aerolineas", "Ver")]
        public IActionResult Listar()
        {
            var lista = _AerolineasData.MtdConsultarAerolineas();
            return View(lista);
        }

        [RequirePermission("Aerolineas", "Crear")]
        public IActionResult Guardar()
        {
            var oAerolinea = new AerolineasModel();
            
            // Cargar Listas Auxiliares (igual que Aeropuertos)
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = new List<string>();
            
            return View(oAerolinea);
        }

        [HttpPost]
        [RequirePermission("Aerolineas", "Crear")]
        public IActionResult Guardar(AerolineasModel oAerolinea)
        {
            // Asignar valores automáticos si no están establecidos
            if (string.IsNullOrEmpty(oAerolinea.IATA) && !string.IsNullOrEmpty(oAerolinea.Pais))
            {
                oAerolinea.IATA = ObtenerCodigoIATAPorPais(oAerolinea.Pais);
            }
            if (string.IsNullOrEmpty(oAerolinea.Direccion) && !string.IsNullOrEmpty(oAerolinea.Pais) && !string.IsNullOrEmpty(oAerolinea.Ciudad))
            {
                oAerolinea.Direccion = ObtenerDireccionPorPaisYCiudad(oAerolinea.Pais, oAerolinea.Ciudad);
            }
            if (oAerolinea.Telefono == null && !string.IsNullOrEmpty(oAerolinea.Pais))
            {
                oAerolinea.Telefono = ObtenerTelefonoPorPais(oAerolinea.Pais);
            }

            var respuesta = _AerolineasData.MtdAgregarAerolinea(oAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");

            // Recargar combos si falla
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAerolinea.Pais) ?? new List<string>();
            
            return View(oAerolinea);
        }

        [RequirePermission("Aerolineas", "Editar")]
        public IActionResult Modificar(int IdAerolinea)
        {
            var oAerolinea = _AerolineasData.MtdBuscarAerolinea(IdAerolinea);
            
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAerolinea.Pais) ?? new List<string>();
            
            return View(oAerolinea);
        }

        [HttpPost]
        [RequirePermission("Aerolineas", "Editar")]
        public IActionResult Modificar(AerolineasModel oAerolinea)
        {
            // Asignar valores automáticos siempre según país y ciudad
            if (!string.IsNullOrEmpty(oAerolinea.Pais))
            {
                // Siempre actualizar IATA si hay país
                oAerolinea.IATA = ObtenerCodigoIATAPorPais(oAerolinea.Pais);
                
                // Actualizar teléfono si hay país
                if (oAerolinea.Telefono == null)
                {
                    oAerolinea.Telefono = ObtenerTelefonoPorPais(oAerolinea.Pais);
                }
            }
            if (!string.IsNullOrEmpty(oAerolinea.Pais) && !string.IsNullOrEmpty(oAerolinea.Ciudad))
            {
                // Siempre actualizar dirección si hay país y ciudad
                oAerolinea.Direccion = ObtenerDireccionPorPaisYCiudad(oAerolinea.Pais, oAerolinea.Ciudad);
            }

            var respuesta = _AerolineasData.MtdEditarAerolinea(oAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");

            // Si falla, recargar combos
            ViewBag.Paises = ObtenerPaises() ?? new List<string>();
            ViewBag.Estados = ObtenerEstados() ?? new List<string>();
            ViewBag.Ciudades = ObtenerCiudadesPorPais(oAerolinea.Pais) ?? new List<string>();

            return View(oAerolinea);
        }

        [RequirePermission("Aerolineas", "Eliminar")]
        public IActionResult Eliminar(int IdAerolinea)
        {
            var oAerolinea = _AerolineasData.MtdBuscarAerolinea(IdAerolinea);
            return View(oAerolinea);
        }

        [HttpPost]
        [RequirePermission("Aerolineas", "Eliminar")]
        public IActionResult Eliminar(AerolineasModel oAerolinea)
        {
            var respuesta = _AerolineasData.MtdEliminarAerolinea(oAerolinea.IdAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }

        // Método JSON para Ajax: obtener ciudades por país
        [RequirePermission("Aerolineas", "Ver")]
        public JsonResult ObtenerCiudades(string pais)
        {
            var ciudades = ObtenerCiudadesPorPais(pais);
            return Json(ciudades);
        }

        // Método JSON para Ajax: obtener código IATA según país
        [RequirePermission("Aerolineas", "Ver")]
        public JsonResult ObtenerCodigoIATA(string pais)
        {
            var iata = ObtenerCodigoIATAPorPais(pais);
            return Json(new { iata = iata });
        }

        // Método JSON para Ajax: obtener dirección según país y ciudad
        [RequirePermission("Aerolineas", "Ver")]
        public JsonResult ObtenerDireccion(string pais, string ciudad)
        {
            var direccion = ObtenerDireccionPorPaisYCiudad(pais, ciudad);
            return Json(new { direccion = direccion });
        }

        // Método JSON para Ajax: obtener teléfono según país
        [RequirePermission("Aerolineas", "Ver")]
        public JsonResult ObtenerTelefono(string pais)
        {
            var telefono = ObtenerTelefonoPorPais(pais);
            return Json(new { telefono = telefono });
        }

        // ------------------ Métodos auxiliares (igual que Aeropuertos) ------------------

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
    }
}
