namespace ProyectoAeroline.Models
{
    public class MantenimientosModel
    {
        public int IdMantenimiento { get; set; }
        public int IdAvion { get; set; }
        public int? IdEmpleado { get; set; }          // Puede ser nulo (PENDIENTE)

        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }

        public string? Tipo { get; set; }
        public decimal Costo { get; set; }
        public decimal CostoExtra { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }

    }
}