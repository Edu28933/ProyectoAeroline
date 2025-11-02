namespace ProyectoAeroline.Models
{
    public class FacturacionModel
    {
        public int IdFactura { get; set; }
        public int IdBoleto { get; set; }
        public DateTime FechaEmision { get; set; }
        public TimeSpan? HoraEmision { get; set; }
        public string? Descripcion { get; set; }
        public string? TipoPago { get; set; }
        public string? Moneda { get; set; }
        public decimal? Monto { get; set; }
        public decimal? Impuesto { get; set; }
        public decimal MontoFactura { get; set; }
        public decimal? MontoTotal { get; set; }
        public int? NumeroAutorizacion { get; set; }
        public string? Estado { get; set; }
    }
}

