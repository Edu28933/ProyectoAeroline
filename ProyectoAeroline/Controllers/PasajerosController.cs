using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using Microsoft.AspNetCore.Authorization;
using ProyectoAeroline.Attributes;

namespace ProyectoAeroline.Controllers
{
    [Authorize]
    public class PasajerosController : Controller
    {
        PasajerosData _PasajerosData = new PasajerosData();

        // ✅ GET: LISTAR
        [RequirePermission("Pasajeros", "Ver")]
        public IActionResult Listar()
        {
            var oListaPasajeros = _PasajerosData.MtdConsultarPasajeros();
            return View(oListaPasajeros);
        }

        // ✅ GET: GUARDAR
        [RequirePermission("Pasajeros", "Crear")]
        public IActionResult Guardar()
        {
            CargarCombos(null);
            return View(new PasajerosModel());
        }

        // ✅ POST: GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Pasajeros", "Crear")]
        public IActionResult Guardar(PasajerosModel oPasajero)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return View(oPasajero);
            }

            // Validación adicional del pasaporte
            if (!string.IsNullOrWhiteSpace(oPasajero.Pasaporte))
            {
                oPasajero.Pasaporte = oPasajero.Pasaporte.ToUpper().Trim();
                
                // Validar formato: solo letras y números
                if (!System.Text.RegularExpressions.Regex.IsMatch(oPasajero.Pasaporte, @"^[A-Z0-9]+$"))
                {
                    ModelState.AddModelError("Pasaporte", "El pasaporte solo puede contener letras mayúsculas y números.");
                    CargarCombos(oPasajero);
                    return View(oPasajero);
                }

                // Validar longitud
                if (oPasajero.Pasaporte.Length < 6 || oPasajero.Pasaporte.Length > 13)
                {
                    ModelState.AddModelError("Pasaporte", "El pasaporte debe tener entre 6 y 13 caracteres.");
                    CargarCombos(oPasajero);
                    return View(oPasajero);
                }
            }

            try
            {
                var respuesta = _PasajerosData.MtdAgregarPasajero(oPasajero);
                if (respuesta)
                {
                    TempData["Success"] = "Pasajero agregado correctamente.";
                    return RedirectToAction("Listar");
                }

                ModelState.AddModelError("", "No se pudo agregar el pasajero.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al agregar: " + ex.Message);
            }

            CargarCombos(oPasajero);
            return View(oPasajero);
        }

        // ✅ GET: MODIFICAR
        [RequirePermission("Pasajeros", "Editar")]
        public IActionResult Modificar(int IdPasajero)
        {
            if (IdPasajero <= 0)
            {
                TempData["Error"] = "ID de pasajero inválido.";
                return RedirectToAction("Listar");
            }

            var oPasajero = _PasajerosData.MtdBuscarPasajero(IdPasajero);
            if (oPasajero == null || oPasajero.IdPasajero == 0)
            {
                TempData["Error"] = "Pasajero no encontrado.";
                return RedirectToAction("Listar");
            }

            CargarCombos(oPasajero);
            return View(oPasajero);
        }

        // ✅ POST: MODIFICAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Pasajeros", "Editar")]
        public IActionResult Modificar(PasajerosModel oPasajero)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(oPasajero);
                return View(oPasajero);
            }

            // Validación adicional del pasaporte
            if (!string.IsNullOrWhiteSpace(oPasajero.Pasaporte))
            {
                oPasajero.Pasaporte = oPasajero.Pasaporte.ToUpper().Trim();
                
                if (!System.Text.RegularExpressions.Regex.IsMatch(oPasajero.Pasaporte, @"^[A-Z0-9]+$"))
                {
                    ModelState.AddModelError("Pasaporte", "El pasaporte solo puede contener letras mayúsculas y números.");
                    CargarCombos(oPasajero);
                    return View(oPasajero);
                }

                if (oPasajero.Pasaporte.Length < 6 || oPasajero.Pasaporte.Length > 12)
                {
                    ModelState.AddModelError("Pasaporte", "El pasaporte debe tener entre 6 y 12 caracteres.");
                    CargarCombos(oPasajero);
                    return View(oPasajero);
                }
            }

            try
            {
                var respuesta = _PasajerosData.MtdEditarPasajero(oPasajero);
                if (respuesta)
                {
                    TempData["Success"] = "Pasajero actualizado correctamente.";
                    return RedirectToAction("Listar");
                }

                ModelState.AddModelError("", "No se pudo actualizar el pasajero.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
            }

            CargarCombos(oPasajero);
            return View(oPasajero);
        }

        // ✅ GET: ELIMINAR
        [RequirePermission("Pasajeros", "Eliminar")]
        public IActionResult Eliminar(int IdPasajero)
        {
            if (IdPasajero <= 0)
            {
                TempData["Error"] = "ID de pasajero inválido.";
                return RedirectToAction("Listar");
            }

            var oPasajero = _PasajerosData.MtdBuscarPasajero(IdPasajero);
            if (oPasajero == null || oPasajero.IdPasajero == 0)
            {
                TempData["Error"] = "El pasajero no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(oPasajero);
        }

        // ✅ POST: ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Pasajeros", "Eliminar")]
        public IActionResult Eliminar(PasajerosModel oPasajero)
        {
            if (oPasajero == null || oPasajero.IdPasajero <= 0)
            {
                TempData["Error"] = "ID de pasajero inválido.";
                return RedirectToAction("Listar");
            }

            try
            {
                var respuesta = _PasajerosData.MtdEliminarPasajero(oPasajero.IdPasajero);
                if (respuesta)
                {
                    TempData["Success"] = "Pasajero eliminado correctamente.";
                    return RedirectToAction("Listar");
                }

                TempData["Error"] = "No se pudo eliminar el pasajero.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
            }

            // Recargar datos si hubo error
            var pasajeroActual = _PasajerosData.MtdBuscarPasajero(oPasajero.IdPasajero);
            if (pasajeroActual == null)
            {
                TempData["Error"] = "El pasajero no se encontró.";
                return RedirectToAction("Listar");
            }
            return View(pasajeroActual);
        }

        // 🧩 MÉTODO PRIVADO PARA COMBOS
        private void CargarCombos(PasajerosModel? model = null)
        {
            // Cargar lista de países (usar la misma para País y Nacionalidad)
            var paises = _PasajerosData.ObtenerPaises();
            ViewBag.Paises = new SelectList(paises, model?.Pais);
            ViewBag.Nacionalidades = new SelectList(paises, model?.Nacionalidad);

            // Cargar tipos de pasajero
            ViewBag.TiposPasajero = new List<SelectListItem>
            {
                new SelectListItem { Value = "Corporativo", Text = "Corporativo" },
                new SelectListItem { Value = "VIP", Text = "VIP" },
                new SelectListItem { Value = "Turista", Text = "Turista" }
            };

            // Cargar estados
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Activo", Text = "Activo" },
                new SelectListItem { Value = "Inactivo", Text = "Inactivo" }
            };
        }
    }
}

