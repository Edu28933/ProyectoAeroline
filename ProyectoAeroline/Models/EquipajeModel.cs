namespace ProyectoAeroline.Models
{
    public class EquipajeModel
    {
        public int IdEquipaje { get; set; }
        public int IdBoleto { get; set; }
        public decimal Peso { get; set; }
        public string? Dimensiones { get; set; }
        public string? CaracteristicasEspeciales { get; set; }
        public decimal? Monto { get; set; }
        public decimal? CostoExtra { get; set; }
        public string? Estado { get; set; }
    }
}

