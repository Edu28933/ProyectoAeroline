using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class AeropuertosController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        AeropuertosData _AeropuertosData = new AeropuertosData();


        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaAeropuertos = _AeropuertosData.MtdConsultarAeropuertos();
            return View(oListaAeropuertos);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(AeropuertosModel oAeropuertos)
        {
            var respuesta = _AeropuertosData.MtdAgregarAeropuerto(oAeropuertos);

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
        public IActionResult Modificar(int CodigoAeropuerto)
        {
            var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);
            return View(oAeropuerto);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(AeropuertosModel oAeropuerto)
        {
            var respuesta = _AeropuertosData.MtdEditarAeropuerto(oAeropuerto);

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
        // GET: Empleados/Eliminar/5
        public IActionResult Eliminar(int CodigoAeropuerto)
        {
            var oAeropuerto = _AeropuertosData.MtdBuscarAeropuerto(CodigoAeropuerto);
            return View(oAeropuerto);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(AeropuertosModel oAeropuerto)
        {
            var respuesta = _AeropuertosData.MtdEliminarAeropuerto(oAeropuerto.IdEmpleado);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }
    }
}
