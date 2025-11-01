using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(MantenimientosModel oMantenimientos)
        {
            var respuesta = _MantenimientosData.MtdAgregarMantenimiento(oMantenimientos);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
            }
        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoMantenimiento)
        {
            var oMantenimiento = _MantenimientosData.MtdBuscarMantenimiento(CodigoMantenimiento);
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
            var oMantenimiento = _MantenimientosData.MtdBuscarMantenimiento(CodigoMantenimiento);
            return View(oMantenimiento);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(MantenimientosModel oMantenimiento)
        {
            var respuesta = _MantenimientosData.MtdEliminarMantenimiento(oMantenimiento.IdMantenimiento);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }
    }
}
