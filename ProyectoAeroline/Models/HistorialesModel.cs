namespace ProyectoAeroline.Models
{
    public class HistorialesModel
    {
        public int IdHistorial { get; set; }
        public int IdBoleto { get; set; }
        public int IdPasajero { get; set; }
        public int IdAerolinea { get; set; }
        public int IdVuelo { get; set; }
        public string? Observacion { get; set; }
    }
}

