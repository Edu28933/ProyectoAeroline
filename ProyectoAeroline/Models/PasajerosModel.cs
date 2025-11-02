using System.ComponentModel.DataAnnotations;

namespace ProyectoAeroline.Models
{
    public class PasajerosModel
    {
        public int IdPasajero { get; set; }
        
        [Required(ErrorMessage = "Los nombres son requeridos")]
        [StringLength(45, ErrorMessage = "Los nombres no pueden exceder 45 caracteres")]
        public string? Nombres { get; set; }
        
        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(45, ErrorMessage = "Los apellidos no pueden exceder 45 caracteres")]
        public string? Apellidos { get; set; }
        
        [Required(ErrorMessage = "El pasaporte es requerido")]
        [StringLength(13, MinimumLength = 6, ErrorMessage = "El pasaporte debe tener entre 6 y 13 caracteres")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "El pasaporte solo puede contener letras mayúsculas y números")]
        public string? Pasaporte { get; set; }
        
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(45, ErrorMessage = "El correo no puede exceder 45 caracteres")]
        public string? Correo { get; set; }
        
        [StringLength(45, ErrorMessage = "La dirección no puede exceder 45 caracteres")]
        public string? Direccion { get; set; }
        
        public int? Telefono { get; set; }
        
        [Required(ErrorMessage = "El país es requerido")]
        [StringLength(45, ErrorMessage = "El país no puede exceder 45 caracteres")]
        public string? Pais { get; set; }
        
        [Required(ErrorMessage = "El tipo de pasajero es requerido")]
        [StringLength(45, ErrorMessage = "El tipo de pasajero no puede exceder 45 caracteres")]
        public string? TipoPasajero { get; set; }
        
        [StringLength(45, ErrorMessage = "La nacionalidad no puede exceder 45 caracteres")]
        public string? Nacionalidad { get; set; }
        
        public int? ContactoEmergencia { get; set; }
        
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
    }
}
