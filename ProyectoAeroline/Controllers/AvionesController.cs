using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class AvionesController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        AvionesData _AvionesData = new AvionesData();



        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaAviones = _AvionesData.MtdConsultarAviones();
            return View(oListaAviones);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(AvionesModel oAviones)
        {
            var respuesta = _AvionesData.MtdAgregarAvion(oAviones);

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
        public IActionResult Modificar(int CodigoAvion)
        {
            var oAvion = _AvionesData.MtdBuscarAvion(CodigoAvion);
            return View(oAvion);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(AvionesModel oAvion)
        {
            var respuesta = _AvionesData.MtdEditarAvion(oAvion);

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
        // GET: Aviones/Eliminar/5
        public IActionResult Eliminar(int CodigoAvion)
        {
            var oAvion = _AvionesData.MtdBuscarAvion(CodigoAvion);
            return View(oAvion);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(AvionesModel oAvion)
        {
            var respuesta = _AvionesData.MtdEliminarAvion(oAvion.IdAvion);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }
    }
}
