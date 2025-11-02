using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class ReservasController : Controller
    {
        ReservasData _ReservasData = new ReservasData();

        // --- LISTAR RESERVAS ---
        public IActionResult Listar()
        {
            var oListaReservas = _ReservasData.MtdConsultarReservas();
            return View(oListaReservas);
        }

        // --- MOSTRAR FORMULARIO GUARDAR ---
        public IActionResult Guardar()
        {
            ViewBag.Pasajeros = _ReservasData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Vuelos = _ReservasData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" }
            };

            return View();
        }

        // --- GUARDAR RESERVA (POST) ---
        [HttpPost]
        public IActionResult Guardar(ReservasModel oReserva)
        {
            if (ModelState.IsValid)
            {
                var respuesta = _ReservasData.MtdAgregarReserva(oReserva);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Pasajeros = _ReservasData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Vuelos = _ReservasData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" }
            };

            return View(oReserva);
        }

        // --- MOSTRAR FORMULARIO MODIFICAR ---
        public IActionResult Modificar(int CodigoReserva)
        {
            var oReserva = _ReservasData.MtdBuscarReserva(CodigoReserva);

            ViewBag.Pasajeros = _ReservasData.MtdListarPasajerosActivos()
                .Select(p => new SelectListItem
                {
                    Value = p.IdPasajero.ToString(),
                    Text = $"{p.Nombres} {p.Apellidos}"
                }).ToList();

            ViewBag.Vuelos = _ReservasData.MtdListarVuelosActivos()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVuelo.ToString(),
                    Text = $"{v.AeropuertoOrigen} → {v.AeropuertoDestino}"
                }).ToList();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Confirmado", Text = "Confirmado" },
                new SelectListItem { Value = "Cancelado", Text = "Cancelado" },
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" }
            };

            return View(oReserva);
        }

        // --- MODIFICAR RESERVA (POST) ---
        [HttpPost]
        public IActionResult Modificar(ReservasModel oReserva)
        {
            var respuesta = _ReservasData.MtdEditarReserva(oReserva);

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
        public IActionResult Eliminar(int CodigoReserva)
        {
            var reserva = _ReservasData.MtdBuscarReserva(CodigoReserva);

            if (reserva == null || reserva.IdReserva == 0)
            {
                TempData["Error"] = "La reserva no existe o ya fue eliminada.";
                return RedirectToAction("Listar");
            }

            return View(reserva);
        }

        // --- ELIMINAR RESERVA (POST) ---
        [HttpPost]
        public IActionResult Eliminar(ReservasModel oReserva)
        {
            try
            {
                bool respuesta = _ReservasData.MtdEliminarReserva(oReserva.IdReserva);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Reserva eliminada correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar la reserva.");
                    return View(oReserva);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
                return View(oReserva);
            }
        }
    }
}

