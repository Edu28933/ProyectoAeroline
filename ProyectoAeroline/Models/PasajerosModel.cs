namespace ProyectoAeroline.Models
{
    public class PasajerosModel
    {
        public int IdPasajero { get; set; }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Pasaporte { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public int? Telefono { get; set; }
        public string? Pais { get; set; }
        public string? TipoPasajero { get; set; }
        public string? Nacionalidad { get; set; }
        public int? ContactoEmergencia { get; set; }
        public string? Estado { get; set; }
    }
}

