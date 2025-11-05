using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class HorariosController : Controller
    {
        private readonly HorariosData _HorariosData = new HorariosData();

        // ✅ LISTAR
        [RequirePermission("Horarios", "Ver")]
        public IActionResult Listar()
        {
            var lista = _HorariosData.MtdConsultarHorarios();
            return View(lista);
        }

        // ✅ GET: GUARDAR
        [RequirePermission("Horarios", "Crear")]
        public IActionResult Guardar()
        {
            CargarCombos();
            return View(new HorariosModel());
        }

        // ✅ POST: GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Horarios", "Crear")]
        public IActionResult Guardar(HorariosModel oHorario)
        {
            // Inicializar modelo si es null
            if (oHorario == null)
                oHorario = new HorariosModel();

            // Convertir strings a TimeSpan si vienen del formulario
            try
            {
                if (Request.Form.ContainsKey("HoraSalidaString"))
                {
                    var horaSalidaStr = Request.Form["HoraSalidaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaSalidaStr))
                    {
                        if (TimeSpan.TryParse(horaSalidaStr, out TimeSpan horaSalida))
                            oHorario.HoraSalida = horaSalida;
                        else
                            ModelState.AddModelError("HoraSalidaString", "Formato de hora de salida inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraSalidaString", "La hora de salida es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("HoraLlegadaString"))
                {
                    var horaLlegadaStr = Request.Form["HoraLlegadaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaLlegadaStr))
                    {
                        if (TimeSpan.TryParse(horaLlegadaStr, out TimeSpan horaLlegada))
                            oHorario.HoraLlegada = horaLlegada;
                        else
                            ModelState.AddModelError("HoraLlegadaString", "Formato de hora de llegada inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraLlegadaString", "La hora de llegada es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("TiempoEsperaString"))
                {
                    var tiempoEsperaStr = Request.Form["TiempoEsperaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(tiempoEsperaStr))
                    {
                        if (TimeSpan.TryParse(tiempoEsperaStr, out TimeSpan tiempoEspera))
                            oHorario.TiempoEspera = tiempoEspera;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar las horas: " + ex.Message);
            }

            // Validar que IdVuelo esté seleccionado
            if (oHorario.IdVuelo <= 0)
            {
                ModelState.AddModelError("IdVuelo", "Debe seleccionar un vuelo.");
            }

            ValidarHoras(oHorario);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oHorario);
            }

            try
            {
                var respuesta = _HorariosData.MtdAgregarHorario(oHorario);
                if (respuesta)
                    return RedirectToAction("Listar");
                
                ModelState.AddModelError("", "No se pudo guardar el horario.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
            }

            CargarCombos();
            return View(oHorario);
        }

        // ✅ GET: MODIFICAR
        [RequirePermission("Horarios", "Editar")]
        public IActionResult Modificar(int IdHorario)
        {
            if (IdHorario <= 0)
                return RedirectToAction("Listar");

            var oHorario = _HorariosData.MtdBuscarHorario(IdHorario);
            if (oHorario == null)
            {
                TempData["Error"] = "El horario no existe o fue eliminado.";
                return RedirectToAction("Listar");
            }

            CargarCombos();
            return View(oHorario);
        }

        // ✅ POST: MODIFICAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Horarios", "Editar")]
        public IActionResult Modificar(HorariosModel oHorario)
        {
            // Inicializar modelo si es null
            if (oHorario == null)
            {
                ModelState.AddModelError("", "Los datos del horario son requeridos.");
                CargarCombos();
                return View(new HorariosModel());
            }

            // Convertir strings a TimeSpan si vienen del formulario
            try
            {
                if (Request.Form.ContainsKey("HoraSalidaString"))
                {
                    var horaSalidaStr = Request.Form["HoraSalidaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaSalidaStr))
                    {
                        if (TimeSpan.TryParse(horaSalidaStr, out TimeSpan horaSalida))
                            oHorario.HoraSalida = horaSalida;
                        else
                            ModelState.AddModelError("HoraSalidaString", "Formato de hora de salida inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraSalidaString", "La hora de salida es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("HoraLlegadaString"))
                {
                    var horaLlegadaStr = Request.Form["HoraLlegadaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(horaLlegadaStr))
                    {
                        if (TimeSpan.TryParse(horaLlegadaStr, out TimeSpan horaLlegada))
                            oHorario.HoraLlegada = horaLlegada;
                        else
                            ModelState.AddModelError("HoraLlegadaString", "Formato de hora de llegada inválido.");
                    }
                    else
                    {
                        ModelState.AddModelError("HoraLlegadaString", "La hora de llegada es requerida.");
                    }
                }

                if (Request.Form.ContainsKey("TiempoEsperaString"))
                {
                    var tiempoEsperaStr = Request.Form["TiempoEsperaString"].ToString();
                    if (!string.IsNullOrWhiteSpace(tiempoEsperaStr))
                    {
                        if (TimeSpan.TryParse(tiempoEsperaStr, out TimeSpan tiempoEspera))
                            oHorario.TiempoEspera = tiempoEspera;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar las horas: " + ex.Message);
            }

            // Validar que IdVuelo esté seleccionado
            if (oHorario.IdVuelo <= 0)
            {
                ModelState.AddModelError("IdVuelo", "Debe seleccionar un vuelo.");
            }

            ValidarHoras(oHorario);

            if (!ModelState.IsValid)
            {
                CargarCombos();
                // Recargar datos completos del horario si hay errores
                if (oHorario.IdHorario > 0)
                {
                    var horarioActual = _HorariosData.MtdBuscarHorario(oHorario.IdHorario);
                    if (horarioActual != null)
                    {
                        // Mantener los valores del formulario pero actualizar datos del vuelo
                        horarioActual.IdVuelo = oHorario.IdVuelo;
                        horarioActual.HoraSalida = oHorario.HoraSalida;
                        horarioActual.HoraLlegada = oHorario.HoraLlegada;
                        horarioActual.TiempoEspera = oHorario.TiempoEspera;
                        horarioActual.Estado = oHorario.Estado;
                        return View(horarioActual);
                    }
                }
                return View(oHorario);
            }

            try
            {
                var respuesta = _HorariosData.MtdEditarHorario(oHorario);
                if (respuesta)
                    return RedirectToAction("Listar");
                
                ModelState.AddModelError("", "No se pudo actualizar el horario.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
            }

            CargarCombos();
            return View(oHorario);
        }

        // ✅ GET: ELIMINAR
        [RequirePermission("Horarios", "Eliminar")]
        public IActionResult Eliminar(int IdHorario)
        {
            var oHorario = _HorariosData.MtdBuscarHorario(IdHorario);
            if (oHorario == null)
                return NotFound();

            return View(oHorario);
        }

        // ✅ POST: ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Horarios", "Eliminar")]
        public IActionResult Eliminar(HorariosModel oHorario)
        {
            if (oHorario == null || oHorario.IdHorario <= 0)
            {
                ModelState.AddModelError("", "ID de horario inválido.");
                return RedirectToAction("Listar");
            }

            try
            {
                var respuesta = _HorariosData.MtdEliminarHorario(oHorario.IdHorario);
                if (respuesta)
                    return RedirectToAction("Listar");
                
                ModelState.AddModelError("", "No se pudo eliminar el horario.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar: " + ex.Message);
            }

            // Recargar datos si hubo error
            var horarioActual = _HorariosData.MtdBuscarHorario(oHorario.IdHorario);
            return View(horarioActual ?? oHorario);
        }

        // 🧩 MÉTODO PRIVADO PARA COMBOS
        private void CargarCombos()
        {
            ViewBag.Vuelos = _HorariosData.ObtenerVuelosParaCombo();
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Activo", Value = "Activo" },
                new SelectListItem { Text = "Inactivo", Value = "Inactivo" },
                new SelectListItem { Text = "Retrasado", Value = "Retrasado" },
                new SelectListItem { Text = "Cancelado", Value = "Cancelado" }
            };
        }

        // 🧩 MÉTODO PRIVADO PARA VALIDAR HORAS
        private void ValidarHoras(HorariosModel oHorario)
        {
            if (oHorario.HoraSalida >= oHorario.HoraLlegada)
            {
                ModelState.AddModelError("HoraLlegada", "La hora de llegada debe ser mayor que la hora de salida.");
            }
        }
    }
}

