using System.ComponentModel.DataAnnotations;

namespace ProyectoAeroline.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;
    }
}
