namespace ProyectoAeroline.Models
{
    public class VuelosModel
    {
        public int IdVuelo { get; set; }
        public int IdAvion { get; set; }
        public string? Aerolinea { get; set; } // Se mantiene como string pero se usará el nombre de la aerolínea del combo
        public string? NumeroVuelo { get; set; }
        public string AeropuertoOrigen { get; set; }
        public string? CodigoIATAOrigen { get; set; }
        public string AeropuertoDestino { get; set; }
        public string? CodigoIATADestino { get; set; }
        public DateTime? FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public string? Clase { get; set; }
        public int? AsientosDisponibles { get; set; }
        public decimal? Precio { get; set; }
        public string? Moneda { get; set; }
        public string? Estado { get; set; }
        // Campo auxiliar para el combo box
        public int? IdAerolinea { get; set; }
    }
}

