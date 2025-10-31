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
        public DateTime? FechaUltimoMantenimiento { get; set; }
        public int RangoKm { get; set; }
        public string? Estado { get; set; }


        //PARA PODER LLAMAR A LOS CAMPOS DE LA BDD
        public class AerolineaModel
        {
            public int IdAerolinea { get; set; }
            public string? Nombre { get; set; }
        }


        // Combos
        public static List<string> Tipos = new() { "Comercial", "Carga", "Privado" };

        public static List<string> Modelos = new()
        {
            "Boeing 737",
            "Airbus A320",
            "Cessna 172",
            "Embraer E190"
        };

        public static List<int> Capacidades = new() { 50, 100, 150, 200, 250 };

        public static List<string> Estados = new() { "Activo", "Mantenimiento", "Inactivo" };

        // RangoKm sugerido por modelo
        public static Dictionary<string, int> RangosPorModelo = new()
        {
            { "Boeing 737", 5600 },
            { "Airbus A320", 6100 },
            { "Cessna 172", 1200 },
            { "Embraer E190", 4500 }
        };

    }
}
