using System.ComponentModel.DataAnnotations;

namespace ProyectoAeroline.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme su contraseña")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
