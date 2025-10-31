using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

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
                var respuesta = _MantenimientosData.MtdAgregarMantenimiento(oMantenimientos);
                if (respuesta)
                    return RedirectToAction("Listar");
            }

            // Recargar combos si hay error
            ViewBag.Aviones = _MantenimientosData.MtdListarAvionesActivos();
            ViewBag.Empleados = _MantenimientosData.MtdListarEmpleadosActivos();
            return View(oMantenimientos);

        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoMantenimiento)
        {
            var oMantenimiento = _MantenimientosData.MtdBuscarMantenimiento(CodigoMantenimiento);

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

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(MantenimientosModel oMantenimiento)
        {
            var respuesta = _MantenimientosData.MtdEditarMantenimiento(oMantenimiento);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
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
                TempData["Error"] = "El mantenimiento no existe o ya fue eliminado.";
                return RedirectToAction("Listar");
            }

            return View(mantenimiento);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(MantenimientosModel oMantenimiento)
        {
            try
            {
                bool respuesta = _MantenimientosData.MtdEliminarMantenimiento(oMantenimiento.IdMantenimiento);

                if (respuesta)
                {
                    TempData["Mensaje"] = "Mantenimiento eliminado correctamente.";
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
                ModelState.AddModelError("", ex.Message);
                return View(oMantenimiento);
            }
        }
    }
}