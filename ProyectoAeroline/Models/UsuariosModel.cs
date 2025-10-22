namespace ProyectoAeroline.Models
{
    public class UsuariosModel
    {
        //prop, doble tabular y crea la sintaxis
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string Contraseña { get; set; }
        public string? Estado { get; set; }
    }
}
