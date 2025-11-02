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
            ValidarFechas(oVuelo);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oVuelo);
            }

            oVuelo.Precio = _VuelosData.ObtenerPrecioRuta(oVuelo.AeropuertoOrigen, oVuelo.AeropuertoDestino);
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

            CargarCombos();
            return View(oVuelo);
        }

        // ✅ POST: MODIFICAR
        [HttpPost]
        public IActionResult Modificar(VuelosModel oVuelo)
        {
            ValidarFechas(oVuelo);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oVuelo);
            }

            oVuelo.Precio = _VuelosData.ObtenerPrecioRuta(oVuelo.AeropuertoOrigen, oVuelo.AeropuertoDestino);
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
            try
            {
                var respuesta = _VuelosData.MtdEliminarVuelo(oVuelo.IdVuelo);
                
                if (respuesta == "OK")
                {
                    TempData["Mensaje"] = "Vuelo eliminado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", respuesta);
                    return View(oVuelo);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oVuelo);
            }
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

        // 🧩 MÉTODO PRIVADO PARA COMBOS
        private void CargarCombos()
        {
            ViewBag.Aviones = _VuelosData.ObtenerAvionesActivos();
            ViewBag.Aeropuertos = _VuelosData.ObtenerAeropuertosActivos();
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
