using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class HistorialesController : Controller
    {
        HistorialesData _HistorialesData = new HistorialesData();

        // --- LISTAR HISTORIALES ---
        public IActionResult Listar()
        {
            var oListaHistoriales = _HistorialesData.MtdConsultarHistoriales();
            return View(oListaHistoriales);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        public IActionResult Guardar()
        {
            ViewBag.Boletos = _HistorialesData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Pasajeros = _HistorialesData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Aerolineas = _HistorialesData.MtdListarAerolineasActivas()
                .Select(a => new SelectListItem
                {
                    Value = a.IdAerolinea.ToString(),
                    Text = a.Nombre
                }).ToList();

            ViewBag.Vuelos = _HistorialesData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            return View();
        }

        // --- GUARDAR HISTORIAL (POST) ---
        [HttpPost]
        public IActionResult Guardar(HistorialesModel oHistorial)
        {
            if (ModelState.IsValid)
            {
                var respuesta = _HistorialesData.MtdAgregarHistorial(oHistorial);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Boletos = _HistorialesData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Pasajeros = _HistorialesData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Aerolineas = _HistorialesData.MtdListarAerolineasActivas()
                .Select(a => new SelectListItem
                {
                    Value = a.IdAerolinea.ToString(),
                    Text = a.Nombre
                }).ToList();

            ViewBag.Vuelos = _HistorialesData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            return View(oHistorial);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        public IActionResult Modificar(int CodigoHistorial)
        {
            var oHistorial = _HistorialesData.MtdBuscarHistorial(CodigoHistorial);

            ViewBag.Boletos = _HistorialesData.MtdListarBoletosActivos()
                .Select(b => new SelectListItem
                {
                    Value = b.IdBoleto.ToString(),
                    Text = $"Boleto {b.IdBoleto} - Vuelo {b.IdVuelo}"
                }).ToList();

            ViewBag.Pasajeros = _HistorialesData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Aerolineas = _HistorialesData.MtdListarAerolineasActivas()
                .Select(a => new SelectListItem
                {
                    Value = a.IdAerolinea.ToString(),
                    Text = a.Nombre
                }).ToList();

            ViewBag.Vuelos = _HistorialesData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            return View(oHistorial);
        }

        // --- MODIFICAR HISTORIAL (POST) ---
        [HttpPost]
        public IActionResult Modificar(HistorialesModel oHistorial)
        {
            var respuesta = _HistorialesData.MtdEditarHistorial(oHistorial);

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
        public IActionResult Eliminar(int CodigoHistorial)
        {
            var historial = _HistorialesData.MtdBuscarHistorial(CodigoHistorial);

            if (historial == null || historial.IdHistorial == 0)
            {
                TempData["Error"] = "El historial no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(historial);
        }

        // --- ELIMINAR HISTORIAL (POST) ---
        [HttpPost]
        public IActionResult Eliminar(HistorialesModel oHistorial)
        {
            try
            {
                bool respuesta = _HistorialesData.MtdEliminarHistorial(oHistorial.IdHistorial);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Historial eliminado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar el historial.");
                    return View(oHistorial);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oHistorial);
            }
        }
    }
}

