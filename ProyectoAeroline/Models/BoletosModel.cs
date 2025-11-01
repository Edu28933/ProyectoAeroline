namespace ProyectoAeroline.Models
{
    public class BoletosModel
    {
        public int IdBoleto { get; set; }
        public int IdVuelo { get; set; }
        public int IdPasajero { get; set; }

        public string? NumeroAsiento { get; set; }     // AsignacionAsiento en el ER
        public string? Clase { get; set; }            // ClaseAsiento en el ER

        public decimal Precio { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? Impuesto { get; set; }
        public decimal Total { get; set; }

        public string? Reembolso { get; set; }
        public DateTime FechaCompra { get; set; }
        public string? Estado { get; set; }
    }
}
