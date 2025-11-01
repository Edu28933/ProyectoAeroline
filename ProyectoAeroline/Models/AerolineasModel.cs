namespace ProyectoAeroline.Models
{
    public class AerolineasModel
    {
        public int IdAerolinea { get; set; }
        public int? IdEmpleado { get; set; }
        public string? IATA { get; set; }
        public string? Nombre { get; set; }
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public int? Telefono { get; set; }
        public string? Estado { get; set; }
    }
}
