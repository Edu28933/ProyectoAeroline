using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Models;
using ProyectoAeroline.Data;

namespace ProyectoAeroline.Controllers
{
    public class EmpleadosController : Controller
    {

        // Instancia de la clase con la conexion y stored procedures
        EmpleadosData _EmpleadosData = new EmpleadosData();



        // Muestra el formulario principal con la lista de datos
        
        public IActionResult Listar()
        {

            var oListaEmpleados = _EmpleadosData.MtdConsultarEmpleados();
            return View(oListaEmpleados);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            return View();
        }
        
        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(EmpleadosModel oEmpleados)
        {
            var respuesta = _EmpleadosData.MtdAgregarEmpleado(oEmpleados);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
            }
        }


        // Muestra el formulario llamador Guardar
        public IActionResult Modificar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            return View(oEmpleado);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(EmpleadosModel oEmpleado)
        {
            var respuesta = _EmpleadosData.MtdEditarEmpleado(oEmpleado);

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
        public IActionResult Eliminar(int CodigoEmpleado)
        {
            var oEmpleado = _EmpleadosData.MtdBuscarEmpleado(CodigoEmpleado);
            return View(oEmpleado);
        }
/*
        // Envia los datos del formulario Eliminar
        [HttpPost]
        public IActionResult Eliminar(EmpleadosModel oEmpleado)
        {
            var respuesta = _EmpleadosData.MtdEliminarEmpleado(oEmpleado.>IdEmpleado);

            if (respuesta == true)
            {
                return RedirectToAction("Listar");
            }
            else
            {
                return View();
            }
        }
*/

    }
}
