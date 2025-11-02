using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;
using System.Linq;

namespace ProyectoAeroline.Controllers
{
    public class MantenimientosController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        MantenimientosData _MantenimientosData = new MantenimientosData();



        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaMantenimientos = _MantenimientosData.MtdConsultarMantenimientos();
            return View(oListaMantenimientos);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            ViewBag.Aviones = _MantenimientosData.MtdListarAvionesActivos();
            ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();

            // Tipos predefinidos
            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Preventivo", Text = "Preventivo" },
                new SelectListItem { Value = "Correctivo", Text = "Correctivo" },
                new SelectListItem { Value = "Inspección", Text = "Inspección" }
            };

            // Estados posibles
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Finalizado", Text = "Finalizado" }
            };

            return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(MantenimientosModel oMantenimientos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var respuesta = _MantenimientosData.MtdAgregarMantenimiento(oMantenimientos);
                    if (respuesta)
                    {
                        TempData["MensajeMantenimiento"] = "Mantenimiento agregado correctamente. El avión ha sido cambiado a 'Mantenimiento'.";
                        return RedirectToAction("Listar");
                    }
                    else
                    {
                        TempData["ErrorMantenimiento"] = "Error al agregar el mantenimiento.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMantenimiento"] = $"Error al agregar el mantenimiento: {ex.Message}";
                }
            }

            // Recargar combos si hay error
            ViewBag.Aviones = _MantenimientosData.MtdListarAvionesActivos();
            ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();
            
            // Tipos predefinidos
            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Preventivo", Text = "Preventivo" },
                new SelectListItem { Value = "Correctivo", Text = "Correctivo" },
                new SelectListItem { Value = "Inspección", Text = "Inspección" }
            };

            // Estados posibles
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Finalizado", Text = "Finalizado" }
            };
            
            return View(oMantenimientos);

        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoMantenimiento)
        {
            var oMantenimiento = _MantenimientosData.MtdBuscarMantenimiento(CodigoMantenimiento);

            // Obtener lista de aviones y asegurar que el avión actual esté seleccionado
            var listaAviones = _MantenimientosData.MtdListarAvionesActivos();
            
            // Si el avión del mantenimiento no está en la lista (puede estar inactivo), agregarlo
            if (oMantenimiento != null && oMantenimiento.IdAvion > 0)
            {
                var avionExiste = listaAviones.Any(a => a.Value == oMantenimiento.IdAvion.ToString());
                if (!avionExiste)
                {
                    // Obtener información del avión desde la BD
                    var avionInfo = _MantenimientosData.MtdObtenerInfoAvion(oMantenimiento.IdAvion);
                    if (avionInfo != null)
                    {
                        listaAviones.Insert(0, new SelectListItem
                        {
                            Value = oMantenimiento.IdAvion.ToString(),
                            Text = $"{oMantenimiento.IdAvion} - {avionInfo}",
                            Selected = true
                        });
                    }
                }
            }
            
            ViewBag.Aviones = new SelectList(listaAviones, "Value", "Text", oMantenimiento?.IdAvion);
            ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();

            ViewBag.Tipos = new List<SelectListItem>
            {
                new SelectListItem { Value = "Preventivo", Text = "Preventivo" },
                new SelectListItem { Value = "Correctivo", Text = "Correctivo" },
                new SelectListItem { Value = "Inspección", Text = "Inspección" }
            };

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                new SelectListItem { Value = "En proceso", Text = "En proceso" },
                new SelectListItem { Value = "Finalizado", Text = "Finalizado" }
            };

            return View(oMantenimiento);

        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(MantenimientosModel oMantenimiento)
        {
            try
            {
                var respuesta = _MantenimientosData.MtdEditarMantenimiento(oMantenimiento);

                if (respuesta == true)
                {
                    TempData["MensajeMantenimiento"] = "Mantenimiento modificado correctamente.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    TempData["ErrorMantenimiento"] = "Error al modificar el mantenimiento.";
                    
                    // Recargar combos si hay error
                    ViewBag.Aviones = _MantenimientosData.MtdListarAvionesActivos();
                    ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();

                    ViewBag.Tipos = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "Preventivo", Text = "Preventivo" },
                        new SelectListItem { Value = "Correctivo", Text = "Correctivo" },
                        new SelectListItem { Value = "Inspección", Text = "Inspección" }
                    };

                    ViewBag.Estados = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                        new SelectListItem { Value = "En proceso", Text = "En proceso" },
                        new SelectListItem { Value = "Finalizado", Text = "Finalizado" }
                    };
                    
                    return View(oMantenimiento);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMantenimiento"] = $"Error al modificar el mantenimiento: {ex.Message}";
                
                ViewBag.Aviones = _MantenimientosData.MtdListarAvionesActivos();
                ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();

                ViewBag.Tipos = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Preventivo", Text = "Preventivo" },
                    new SelectListItem { Value = "Correctivo", Text = "Correctivo" },
                    new SelectListItem { Value = "Inspección", Text = "Inspección" }
                };

                ViewBag.Estados = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Pendiente", Text = "Pendiente" },
                    new SelectListItem { Value = "En proceso", Text = "En proceso" },
                    new SelectListItem { Value = "Finalizado", Text = "Finalizado" }
                };
                
                return View(oMantenimiento);
            }
        }


        // Muestra el formulario llamador Eliminar
        // GET: Mantenimientos/Eliminar/5
        public IActionResult Eliminar(int CodigoMantenimiento)
        {
            // Buscar el mantenimiento por su código
            var mantenimiento = _MantenimientosData.MtdBuscarMantenimiento(CodigoMantenimiento);

            if (mantenimiento == null || mantenimiento.IdMantenimiento == 0)
            {
                TempData["ErrorMantenimiento"] = "El mantenimiento no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(mantenimiento);
        }

        // POST: Mantenimientos/Eliminar
        [HttpPost]
        public IActionResult Eliminar(MantenimientosModel oMantenimiento)
        {
            try
            {
                bool respuesta = _MantenimientosData.MtdEliminarMantenimiento(oMantenimiento.IdMantenimiento);

                if (respuesta)
                {
                    TempData["MensajeMantenimiento"] = "Mantenimiento eliminado correctamente. El estado del avión se actualizará automáticamente si no hay más mantenimientos pendientes.";
                    return RedirectToAction("Listar");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo eliminar el mantenimiento, su estado no lo permite.");
                    return View(oMantenimiento);
                }
            }
            catch (SqlException ex)
            {
                // Captura el mensaje del trigger (por ejemplo: "Solo se pueden eliminar mantenimientos en estado 'Finalizado'.")
                TempData["ErrorMantenimiento"] = $"Error al eliminar el mantenimiento: {ex.Message}";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                TempData["ErrorMantenimiento"] = $"Error al eliminar el mantenimiento: {ex.Message}";
                return RedirectToAction("Listar");
            }
        }
    }
}