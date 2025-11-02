using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class EscalasController : Controller
    {
        private readonly EscalasData _EscalasData = new EscalasData();

        // ✅ LISTAR
        public IActionResult Listar()
        {
            var lista = _EscalasData.MtdConsultarEscalas();
            return View(lista);
        }

        // ✅ GET: GUARDAR
        public IActionResult Guardar()
        {
            CargarCombos();
            return View(new EscalasModel());
        }

        // ✅ POST: GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(EscalasModel oEscala)
        {
            // Inicializar modelo si es null
            if (oEscala == null)
                oEscala = new EscalasModel();

            // Convertir strings a TimeSpan si vienen del formulario
            try
            {
                if (Request.Form.ContainsKey("HoraLlegadaString"))
                {
                    var horaLlegadaStr = Request.Form["HoraLlegadaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaLlegadaStr))
                    {
                        if (TimeSpan.TryParse(horaLlegadaStr, out TimeSpan horaLlegada))
                            oEscala.HoraLlegada = horaLlegada;
                        else
                            ModelState.AddModelError("HoraLlegadaString", "Formato de hora de llegada inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraLlegadaString", "La hora de llegada es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("HoraSalidaString"))
                {
                    var horaSalidaStr = Request.Form["HoraSalidaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaSalidaStr))
                    {
                        if (TimeSpan.TryParse(horaSalidaStr, out TimeSpan horaSalida))
                            oEscala.HoraSalida = horaSalida;
                        else
                            ModelState.AddModelError("HoraSalidaString", "Formato de hora de salida inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraSalidaString", "La hora de salida es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("TiempoEsperaString"))
                {
                    var tiempoEsperaStr = Request.Form["TiempoEsperaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(tiempoEsperaStr))
                    {
                        if (TimeSpan.TryParse(tiempoEsperaStr, out TimeSpan tiempoEspera))
                            oEscala.TiempoEspera = tiempoEspera;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar las horas: " + ex.Message);
            }

            // Validar que IdVuelo esté seleccionado
            if (oEscala.IdVuelo <= 0)
            {
                ModelState.AddModelError("IdVuelo", "Debe seleccionar un vuelo.");
            }

            // Validar que IdAeropuerto esté seleccionado
            if (oEscala.IdAeropuerto <= 0)
            {
                ModelState.AddModelError("IdAeropuerto", "Debe seleccionar un aeropuerto.");
            }

            ValidarHoras(oEscala);
            ValidarTiempoEspera(oEscala);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oEscala);
            }

            try
            {
                var respuesta = _EscalasData.MtdAgregarEscala(oEscala);
                if (respuesta)
                    return RedirectToAction("Listar");
                
                ModelState.AddModelError("", "No se pudo guardar la escala.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
            }

            CargarCombos();
            return View(oEscala);
        }

        // ✅ GET: MODIFICAR
        public IActionResult Modificar(int IdEscala)
        {
            if (IdEscala <= 0)
                return RedirectToAction("Listar");

            var oEscala = _EscalasData.MtdBuscarEscala(IdEscala);
            if (oEscala == null)
            {
                TempData["Error"] = "La escala no existe o fue eliminada.";
                return RedirectToAction("Listar");
            }

            CargarCombos();
            return View(oEscala);
        }

        // ✅ POST: MODIFICAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modificar(EscalasModel oEscala)
        {
            // Inicializar modelo si es null
            if (oEscala == null)
            {
                ModelState.AddModelError("", "Los datos de la escala son requeridos.");
                CargarCombos();
                return View(new EscalasModel());
            }

            // Convertir strings a TimeSpan si vienen del formulario
            try
            {
                if (Request.Form.ContainsKey("HoraLlegadaString"))
                {
                    var horaLlegadaStr = Request.Form["HoraLlegadaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaLlegadaStr))
                    {
                        if (TimeSpan.TryParse(horaLlegadaStr, out TimeSpan horaLlegada))
                            oEscala.HoraLlegada = horaLlegada;
                        else
                            ModelState.AddModelError("HoraLlegadaString", "Formato de hora de llegada inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraLlegadaString", "La hora de llegada es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("HoraSalidaString"))
                {
                    var horaSalidaStr = Request.Form["HoraSalidaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaSalidaStr))
                    {
                        if (TimeSpan.TryParse(horaSalidaStr, out TimeSpan horaSalida))
                            oEscala.HoraSalida = horaSalida;
                        else
                            ModelState.AddModelError("HoraSalidaString", "Formato de hora de salida inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraSalidaString", "La hora de salida es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("TiempoEsperaString"))
                {
                    var tiempoEsperaStr = Request.Form["TiempoEsperaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(tiempoEsperaStr))
                    {
                        if (TimeSpan.TryParse(tiempoEsperaStr, out TimeSpan tiempoEspera))
                            oEscala.TiempoEspera = tiempoEspera;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar las horas: " + ex.Message);
            }

            // Validar que IdVuelo esté seleccionado
            if (oEscala.IdVuelo <= 0)
            {
                ModelState.AddModelError("IdVuelo", "Debe seleccionar un vuelo.");
            }

            // Validar que IdAeropuerto esté seleccionado
            if (oEscala.IdAeropuerto <= 0)
            {
                ModelState.AddModelError("IdAeropuerto", "Debe seleccionar un aeropuerto.");
            }

            ValidarHoras(oEscala);
            ValidarTiempoEspera(oEscala);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                // Recargar datos completos de la escala si hay errores
                if (oEscala.IdEscala > 0)
                {
                    var escalaActual = _EscalasData.MtdBuscarEscala(oEscala.IdEscala);
                    if (escalaActual != null)
                    {
                        // Mantener los valores del formulario pero actualizar descripciones
                        escalaActual.IdVuelo = oEscala.IdVuelo;
                        escalaActual.IdAeropuerto = oEscala.IdAeropuerto;
                        escalaActual.HoraLlegada = oEscala.HoraLlegada;
                        escalaActual.HoraSalida = oEscala.HoraSalida;
                        escalaActual.TiempoEspera = oEscala.TiempoEspera;
                        escalaActual.Estado = oEscala.Estado;
                        return View(escalaActual);
                    }
                }
                return View(oEscala);
            }

            try
            {
                var respuesta = _EscalasData.MtdEditarEscala(oEscala);
                if (respuesta)
                    return RedirectToAction("Listar");
                
                ModelState.AddModelError("", "No se pudo actualizar la escala.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
            }

            CargarCombos();
            return View(oEscala);
        }

        // ✅ GET: ELIMINAR
        public IActionResult Eliminar(int IdEscala)
        {
            var oEscala = _EscalasData.MtdBuscarEscala(IdEscala);
            if (oEscala == null)
                return NotFound();

            return View(oEscala);
        }

        // ✅ POST: ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(EscalasModel oEscala)
        {
            if (oEscala == null || oEscala.IdEscala <= 0)
            {
                TempData["Error"] = "ID de escala inválido.";
                return RedirectToAction("Listar");
            }

            try
            {
                var respuesta = _EscalasData.MtdEliminarEscala(oEscala.IdEscala);
                if (respuesta)
                {
                    TempData["Success"] = "Escala eliminada correctamente.";
                    return RedirectToAction("Listar");
                }
                
                TempData["Error"] = "No se pudo eliminar la escala.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
            }

            // Recargar datos si hubo error
            var escalaActual = _EscalasData.MtdBuscarEscala(oEscala.IdEscala);
            if (escalaActual == null)
            {
                TempData["Error"] = "La escala no se encontró.";
                return RedirectToAction("Listar");
            }
            return View(escalaActual);
        }

        // 🧩 MÉTODO PRIVADO PARA COMBOS
        private void CargarCombos()
        {
            ViewBag.Vuelos = _EscalasData.ObtenerVuelosParaCombo();
            ViewBag.Aeropuertos = _EscalasData.ObtenerAeropuertosParaCombo();
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Activo", Value = "Activo" },
                new SelectListItem { Text = "Inactivo", Value = "Inactivo" },
                new SelectListItem { Text = "Retrasado", Value = "Retrasado" },
                new SelectListItem { Text = "Cancelado", Value = "Cancelado" }
            };
        }

        // 🧩 MÉTODO PRIVADO PARA VALIDAR HORAS
        private void ValidarHoras(EscalasModel oEscala)
        {
            // HoraLlegada debe ser menor que HoraSalida (el avión llega, espera, y luego sale)
            if (oEscala.HoraLlegada >= oEscala.HoraSalida)
            {
                ModelState.AddModelError("HoraSalidaString", "La hora de salida debe ser mayor que la hora de llegada.");
            }
        }

        // 🧩 MÉTODO PRIVADO PARA VALIDAR TIEMPO DE ESPERA
        private void ValidarTiempoEspera(EscalasModel oEscala)
        {
            // TiempoEspera no puede ser negativo
            if (oEscala.TiempoEspera.HasValue && oEscala.TiempoEspera < TimeSpan.Zero)
            {
                ModelState.AddModelError("TiempoEsperaString", "El tiempo de espera no puede ser negativo.");
            }
        }
    }
}

