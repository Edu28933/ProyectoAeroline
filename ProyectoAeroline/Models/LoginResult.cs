namespace ProyectoAeroline.Models
{
    public class LoginResult
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;

        // opcionales
        public bool IsAuthenticated { get; set; }
        public string? Message { get; set; }
    }
}
