namespace ProyectoAeroline.Models
{
    public class AeropuertosModel
    {
        public int IdAeropuerto { get; set; }   // PK
        public int IdEmpleado { get; set; }     // FK - se usará en un ComboBox para seleccionar el empleado
        public string? IATA { get; set; }        // Código IATA del aeropuerto
        public string? Nombre { get; set; }      // Nombre del aeropuerto
        public string? Pais { get; set; }       // País donde se ubica
        public string? Ciudad { get; set; }     // Ciudad donde se ubica
        public string? Direccion { get; set; }  // Dirección exacta
        public int? Telefono { get; set; }      // Número telefónico (opcional)
        public string? Estado { get; set; }     // Estado (Activo/Inactivo)
    }
}
