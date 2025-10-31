namespace ProyectoAeroline.Models
{
    public class AerolineasModel
    {
        public int IdAerolinea { get; set; }   // PK
        public int? IdEmpleado { get; set; }   // FK opcional
        public string? IATA { get; set; }
        public string? Nombre { get; set; }
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public int? Telefono { get; set; }     // Puede ser null
        public string? Estado { get; set; }
    }
}
