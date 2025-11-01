using System.ComponentModel.DataAnnotations;

namespace ProyectoAeroline.Models
{
    public class ResetPasswordViewModel
    {
        // token que llega por querystring, lo guardamos en hidden
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la nueva contraseña")]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme su contraseña")]
        [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string Confirmacion { get; set; } = string.Empty;
    }
}
