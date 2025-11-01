namespace ProyectoAeroline.Models
{
    public class RolPantallaOpcionModel
    {
        public int IdRolPantallaOpcion { get; set; }
        public int IdRol { get; set; }
        public int IdPantalla { get; set; }
        public bool Ver { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Eliminar { get; set; }
        public string? Estado { get; set; }
        
        // Campos adicionales para mostrar información relacionada
        public string? NombreRol { get; set; }
        public string? NombrePantalla { get; set; }
    }
}

