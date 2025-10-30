using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class AvionesController : Controller
    {
        // Instancia de la clase con la conexion y stored procedures
        AvionesData _AvionesData = new AvionesData();

        private string GenerarPlaca()
        {
            var random = new Random();
            string letras = new string(Enumerable.Range(0, 3)
                .Select(_ => (char)random.Next('A', 'Z' + 1)).ToArray());
            string numeros = random.Next(1000, 9999).ToString();
            return $"{letras}-{numeros}"; // Ejemplo: ABC-1234
        }

        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaAviones = _AvionesData.MtdConsultarAviones();
            return View(oListaAviones);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            // Obtener aerolíneas activas desde el usp
            var aerolineas = _AvionesData.MtdObtenerAerolineas();
            ViewBag.Aerolineas = new SelectList(aerolineas, "IdAerolinea", "Nombre");

            ViewBag.Tipos = new SelectList(AvionesModel.Tipos);
            ViewBag.Modelos = new SelectList(AvionesModel.Modelos);
            ViewBag.Capacidades = new SelectList(AvionesModel.Capacidades);
            ViewBag.Estados = new SelectList(AvionesModel.Estados);

            var model = new AvionesModel
            {
                Placa = GenerarPlaca(),
                FechaUltimoMantenimiento = DateTime.Now
            };

            return View(model);
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(AvionesModel oAviones)
        {
            oAviones.Placa ??= GenerarPlaca();
            var respuesta = _AvionesData.MtdAgregarAvion(oAviones);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View(oAviones);

        }


        // Muestra el formulario llamador Modificar
        public IActionResult Modificar(int CodigoAvion)
        {
            var oAvion = _AvionesData.MtdBuscarAvion(CodigoAvion);
            var aerolineas = _AvionesData.MtdObtenerAerolineas();
            ViewBag.Aerolineas = new SelectList(aerolineas, "IdAerolinea", "Nombre", oAvion.IdAerolinea);

            ViewBag.Tipos = new SelectList(AvionesModel.Tipos, oAvion.Tipo);
            ViewBag.Modelos = new SelectList(AvionesModel.Modelos, oAvion.Modelo);
            ViewBag.Capacidades = new SelectList(AvionesModel.Capacidades, oAvion.Capacidad);
            ViewBag.Estados = new SelectList(AvionesModel.Estados, oAvion.Estado);

            return View(oAvion);

            /*En SelectList del Modificar se pasa oAvion.IdAerolinea para que el dropdown muestre 
             * seleccionada la aerolínea actual.*/

            //var oAvion = _AvionesData.MtdBuscarAvion(CodigoAvion);
            //return View(oAvion);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(AvionesModel oAvion)
        {
            var respuesta = _AvionesData.MtdEditarAvion(oAvion);
            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View(oAvion);
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
