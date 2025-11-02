namespace ProyectoAeroline.Models
{
    public class EscalasModel
    {
        public int IdEscala { get; set; }
        public int IdVuelo { get; set; }
        public int IdAeropuerto { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan? TiempoEspera { get; set; }
        
        // Propiedades auxiliares para formularios
        public string HoraLlegadaString 
        { 
            get => HoraLlegada.ToString(@"hh\:mm"); 
            set 
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (TimeSpan.TryParse(value, out TimeSpan ts))
                        HoraLlegada = ts;
                }
            }
        }
        
        public string HoraSalidaString 
        { 
            get => HoraSalida.ToString(@"hh\:mm"); 
            set 
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (TimeSpan.TryParse(value, out TimeSpan ts))
                        HoraSalida = ts;
                }
            }
        }
        
        public string? TiempoEsperaString 
        { 
            get => TiempoEspera?.ToString(@"hh\:mm"); 
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    TiempoEspera = null;
                else if (TimeSpan.TryParse(value, out TimeSpan ts))
                    TiempoEspera = ts;
            }
        }
        
        public string? Estado { get; set; }
        
        // Campos de auditoría
        public string? UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public TimeSpan? HoraCreacion { get; set; }
        public string? UsuarioActualizacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public TimeSpan? HoraActualizacion { get; set; }
        public string? UsuarioEliminacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public TimeSpan? HoraEliminacion { get; set; }
        
        // Campos adicionales para mostrar en vistas
        public string? DescripcionVuelo { get; set; }  // (IdVuelo + AeropuertoOrigen + AeropuertoDestino)
        public string? DescripcionAeropuerto { get; set; }  // (IdAeropuerto + Nombre)
    }
}

