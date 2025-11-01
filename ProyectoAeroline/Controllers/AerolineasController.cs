using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Data;
using ProyectoAeroline.Models;

namespace ProyectoAeroline.Controllers
{
    public class AerolineasController : Controller
    {
        AerolineasData _AerolineasData = new AerolineasData();

        public IActionResult Listar()
        {
            var lista = _AerolineasData.MtdConsultarAerolineas();
            return View(lista);
        }

        public IActionResult Guardar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Guardar(AerolineasModel oAerolinea)
        {
            var respuesta = _AerolineasData.MtdAgregarAerolinea(oAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }

        public IActionResult Modificar(int IdAerolinea)
        {
            var oAerolinea = _AerolineasData.MtdBuscarAerolinea(IdAerolinea);
            return View(oAerolinea);
        }

        [HttpPost]
        public IActionResult Modificar(AerolineasModel oAerolinea)
        {
            var respuesta = _AerolineasData.MtdEditarAerolinea(oAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }

        public IActionResult Eliminar(int IdAerolinea)
        {
            var oAerolinea = _AerolineasData.MtdBuscarAerolinea(IdAerolinea);
            return View(oAerolinea);
        }

        [HttpPost]
        public IActionResult Eliminar(AerolineasModel oAerolinea)
        {
            var respuesta = _AerolineasData.MtdEliminarAerolinea(oAerolinea.IdAerolinea);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }
    }
}
