using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class VuelosController : Controller
    {
        private readonly VuelosData _VuelosData = new VuelosData();

        // ✅ LISTAR
        public IActionResult Listar()
        {
            var lista = _VuelosData.MtdConsultarVuelos();
            return View(lista);
        }

        // ✅ GET: GUARDAR
        public IActionResult Guardar()
        {
            CargarCombos();
            return View();
        }

        // ✅ POST: GUARDAR
        [HttpPost]
        public IActionResult Guardar(VuelosModel oVuelo)
        {
            // Si se seleccionó IdAerolinea pero no se llenó Aerolinea, obtener el nombre
            if (oVuelo.IdAerolinea.HasValue && string.IsNullOrEmpty(oVuelo.Aerolinea))
            {
                var aerolineas = _VuelosData.ObtenerAerolineasActivas();
                var aerolineaSeleccionada = aerolineas.FirstOrDefault(a => a.Value == oVuelo.IdAerolinea.Value.ToString());
                if (aerolineaSeleccionada != null)
                {
                    var texto = aerolineaSeleccionada.Text;
                    var nombre = texto.Substring(texto.IndexOf('-') + 2);
                    oVuelo.Aerolinea = nombre;
                }
            }

            ValidarFechas(oVuelo);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oVuelo);
            }

            // El precio ya debería estar calculado en el cliente
            // Si no está, calcularlo
            if (!oVuelo.Precio.HasValue || oVuelo.Precio == 0)
            {
                oVuelo.Precio = _VuelosData.ObtenerPrecioRuta(oVuelo.AeropuertoOrigen, oVuelo.AeropuertoDestino);
            }

            var respuesta = _VuelosData.MtdAgregarVuelo(oVuelo);

            if (respuesta)
                return RedirectToAction("Listar");

            CargarCombos();
            return View(oVuelo);
        }

        // ✅ GET: MODIFICAR
        public IActionResult Modificar(int IdVuelo)
        {
            var oVuelo = _VuelosData.MtdBuscarVuelo(IdVuelo);
            if (oVuelo == null)
                return NotFound();

            // Si hay nombre de aerolínea, buscar el IdAerolinea correspondiente
            if (!string.IsNullOrEmpty(oVuelo.Aerolinea))
            {
                var aerolineas = _VuelosData.ObtenerAerolineasActivas();
                var aerolineaEncontrada = aerolineas.FirstOrDefault(a => 
                    a.Text.Contains(oVuelo.Aerolinea, StringComparison.OrdinalIgnoreCase));
                if (aerolineaEncontrada != null && int.TryParse(aerolineaEncontrada.Value, out int idAerolinea))
                {
                    oVuelo.IdAerolinea = idAerolinea;
                }
            }

            CargarCombos();
            return View(oVuelo);
        }

        // ✅ POST: MODIFICAR
        [HttpPost]
        public IActionResult Modificar(VuelosModel oVuelo)
        {
            // Si se seleccionó IdAerolinea pero no se llenó Aerolinea, obtener el nombre
            if (oVuelo.IdAerolinea.HasValue && string.IsNullOrEmpty(oVuelo.Aerolinea))
            {
                var aerolineas = _VuelosData.ObtenerAerolineasActivas();
                var aerolineaSeleccionada = aerolineas.FirstOrDefault(a => a.Value == oVuelo.IdAerolinea.Value.ToString());
                if (aerolineaSeleccionada != null)
                {
                    var texto = aerolineaSeleccionada.Text;
                    var nombre = texto.Substring(texto.IndexOf('-') + 2);
                    oVuelo.Aerolinea = nombre;
                }
            }

            ValidarFechas(oVuelo);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oVuelo);
            }

            // El precio ya debería estar calculado en el cliente
            // Si no está, calcularlo
            if (!oVuelo.Precio.HasValue || oVuelo.Precio == 0)
            {
                oVuelo.Precio = _VuelosData.ObtenerPrecioRuta(oVuelo.AeropuertoOrigen, oVuelo.AeropuertoDestino);
            }

            var respuesta = _VuelosData.MtdEditarVuelo(oVuelo);

            if (respuesta)
                return RedirectToAction("Listar");

            CargarCombos();
            return View(oVuelo);
        }

        // ✅ GET: ELIMINAR
        public IActionResult Eliminar(int IdVuelo)
        {
            var oVuelo = _VuelosData.MtdBuscarVuelo(IdVuelo);
            if (oVuelo == null)
                return NotFound();

            return View(oVuelo);
        }

        // ✅ POST: ELIMINAR
        [HttpPost]
        public IActionResult Eliminar(VuelosModel oVuelo)
        {
            var respuesta = _VuelosData.MtdEliminarVuelo(oVuelo.IdVuelo);
            if (respuesta)
                return RedirectToAction("Listar");

            return View(oVuelo);
        }

        // ✅ API para el precio dinámico
        [HttpGet]
        [Route("api/vuelos/precio")]
        public IActionResult ObtenerPrecio(string origen, string destino)
        {
            if (string.IsNullOrEmpty(origen) || string.IsNullOrEmpty(destino))
                return Json(new { precio = 0 });

            var precio = _VuelosData.ObtenerPrecioRuta(origen, destino);
            return Json(new { precio });
        }

        // API para obtener código IATA de aeropuerto
        [HttpGet]
        public JsonResult ObtenerCodigoIATA(string nombreAeropuerto)
        {
            if (string.IsNullOrEmpty(nombreAeropuerto))
                return Json(new { iata = "" });

            var iata = _VuelosData.ObtenerCodigoIATAAeropuerto(nombreAeropuerto);
            return Json(new { iata });
        }

        // API para obtener capacidad del avión
        [HttpGet]
        public JsonResult ObtenerCapacidadAvion(int idAvion)
        {
            if (idAvion <= 0)
                return Json(new { capacidad = 0 });

            var capacidad = _VuelosData.ObtenerCapacidadAvion(idAvion);
            return Json(new { capacidad });
        }

        // 🧩 MÉTODO PRIVADO PARA COMBOS
        private void CargarCombos()
        {
            ViewBag.Aviones = _VuelosData.ObtenerAvionesActivos();
            ViewBag.Aeropuertos = _VuelosData.ObtenerAeropuertosActivos();
            ViewBag.Aerolineas = _VuelosData.ObtenerAerolineasActivas();
            ViewBag.Monedas = new List<SelectListItem>
            {
                new SelectListItem { Text = "GTQ", Value = "GTQ" },
                new SelectListItem { Text = "EURO", Value = "EURO" },
                new SelectListItem { Text = "USD", Value = "USD" }
            };
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Activo", Value = "Activo" },
                new SelectListItem { Text = "Inactivo", Value = "Inactivo" },
                new SelectListItem { Text = "Retrasado", Value = "Retrasado" },
                new SelectListItem { Text = "Cancelado", Value = "Cancelado" },
                new SelectListItem { Text = "En vuelo", Value = "En vuelo" },
                new SelectListItem { Text = "Aterrizado", Value = "Aterrizado" }
            };
        }

        // 🧩 MÉTODO PRIVADO PARA VALIDAR FECHAS
        private void ValidarFechas(VuelosModel oVuelo)
        {
            if (oVuelo.FechaHoraSalida < DateTime.Now)
                ModelState.AddModelError("FechaHoraSalida", "La fecha de salida no puede ser menor a hoy.");

            if (oVuelo.FechaHoraLlegada < oVuelo.FechaHoraSalida)
                ModelState.AddModelError("FechaHoraLlegada", "La fecha de llegada no puede ser menor que la salida.");
        }
    }
}
