namespace ProyectoAeroline.Models
{
    public class ServiciosModel
    {
        public int IdServicio { get; set; }
        public int IdBoleto { get; set; }
        public DateTime? Fecha { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public int? Cantidad { get; set; }
        public decimal? CostoTotal { get; set; }
        public string? Estado { get; set; }
    }
}

