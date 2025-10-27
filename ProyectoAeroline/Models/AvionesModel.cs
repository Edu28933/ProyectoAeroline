namespace ProyectoAeroline.Models
{
    public class AvionesModel
    {
        public int IdAvion { get; set; }
        public int IdAerolinea { get; set; }     // FK - se usará como ComboBox
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public string? Tipo { get; set; }
        public int Capacidad { get; set; }
        public DateTime FechaUltimoMantenimiento { get; set; }
        public int RangoKm { get; set; }
        public string? Estado { get; set; }
    }
}
