using Microsoft.AspNetCore.Mvc;
using ProyectoAeroline.Models;
using ProyectoAeroline.Data;

namespace ProyectoAeroline.Controllers
{
    public class UsuariosController : Controller
    {

        // Instancia de la clase con la conexion y stored procedures
        UsuariosData _UsuariosData = new UsuariosData();



        // Muestra el formulario principal con la lista de datos

        public IActionResult Listar()
        {

            var oListaUsuarios = _UsuariosData.MtdConsultarUsuarios();
            return View(oListaUsuarios);
        }



        // Muestra el formulario llamador Guardar
        public IActionResult Guardar()
        {
            return View();
        }

        // Almacena los datos del formulario Guardar
        [HttpPost]
        public IActionResult Guardar(UsuariosModel oUsuario)
        {
            var respuesta = _UsuariosData.MtdAgregarUsuario(oUsuario);

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
        public IActionResult Modificar(int CodigoUsuario)
        {
            var oUsuario = _UsuariosData.MtdBuscarUsuario(CodigoUsuario);
            return View(oUsuario);
        }

        // Almacena los datos del formulario Editar
        [HttpPost]
        public IActionResult Modificar(UsuariosModel oUsuario)
        {
            var respuesta = _UsuariosData.MtdEditarUsuario(oUsuario);

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
        // GET: Usuarios/Eliminar/5
        public IActionResult Eliminar(int CodigoUsuario)
        {
            var oUsuario = _UsuariosData.MtdBuscarUsuario(CodigoUsuario);
            return View(oUsuario);
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        public IActionResult Eliminar(UsuariosModel oUsuario)
        {
            var respuesta = _UsuariosData.MtdEliminarUsuario(oUsuario.IdUsuario);

            if (respuesta)
                return RedirectToAction("Listar");
            else
                return View();
        }



    }
}
