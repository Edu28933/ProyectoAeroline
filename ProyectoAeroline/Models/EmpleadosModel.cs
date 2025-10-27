using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAeroline.Models
{
    public class EmpleadosModel
    {
        //prop, doble tabular y crea la sintaxis
        public int IdEmpleado { get; set; }
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Cargo { get; set; }
        public string? Licencia { get; set; }
        public int Telefono { get; set; }
        public string? Correo { get; set; }
        public double Salario { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaIngreso { get; set; }
        public int ContactoEmergencia { get; set; }
        public string? Estado { get; set; }
        // 1. Campo para almacenar la ruta en la base de datos
        public string? FotoRuta { get; set; }

        // 2. Campo para recibir el archivo subido desde el formulario (No se mapea a DB)
        [NotMapped]
        public IFormFile? FotoArchivo { get; set; }

    }
}