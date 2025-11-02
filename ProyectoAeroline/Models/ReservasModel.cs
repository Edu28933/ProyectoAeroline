namespace ProyectoAeroline.Models
{
    public class ReservasModel
    {
        public int IdReserva { get; set; }
        public int IdPasajero { get; set; }
        public int IdVuelo { get; set; }
        public DateTime FechaReserva { get; set; }
        public decimal? MontoAnticipo { get; set; }
        public DateTime? FechaVuelo { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }
    }
}

