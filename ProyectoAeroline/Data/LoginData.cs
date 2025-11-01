using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ProyectoAeroline.Seguridad;

namespace ProyectoAeroline.Data
{
    public class LoginData
    {
        private readonly IConfiguration _cfg;
        private readonly string _cn;

        public LoginData(IConfiguration cfg)
        {
            _cfg = cfg;
            _cn = _cfg.GetConnectionString("CadenaSQL")!;
        }

        // Método que valida usuario verificando el hash de la contraseña
        public async Task<LoginResult?> ValidarUsuarioAsync(string correo, string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return null;

            using var cn = new SqlConnection(_cn);
            
            // Obtener usuario y su hash de contraseña de la base de datos
            // Buscar tanto por correo como por nombre de usuario
            using var cmd = new SqlCommand(@"
                SELECT 
                    U.[IdUsuario],
                    U.[IdRol],
                    U.[Nombre],
                    U.[Correo],
                    U.[Contraseña],
                    R.[NombreRol]
                FROM [dbo].[Usuarios] U
                LEFT JOIN [dbo].[Roles] R ON U.[IdRol] = R.[IdRol]
                WHERE (U.[Correo] = @Buscar OR U.[Nombre] = @Buscar)
                  AND U.[Estado] = 'Activo'
            ", cn);

            cmd.Parameters.AddWithValue("@Buscar", correo);
            await cn.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                var storedPasswordHash = rd["Contraseña"]?.ToString() ?? "";
                
                // Verificar la contraseña:
                // 1. Si es 'EXTERNAL_LOGIN', no permite login con contraseña (solo Google)
                // PERO si el usuario tiene EXTERNAL_LOGIN, puede usar "Olvidé mi contraseña" para establecer una
                // Aquí simplemente rechazamos el login con contraseña porque no tiene una establecida aún
                if (storedPasswordHash == "EXTERNAL_LOGIN")
                {
                    return null; // Usuario solo puede entrar con Google, o debe establecer contraseña usando "Olvidé mi contraseña"
                }

                // 2. Si la contraseña almacenada es igual a la ingresada (texto plano), permitir acceso
                // Esto es para compatibilidad con usuarios antiguos
                if (storedPasswordHash == password)
                {
                    return new LoginResult
                    {
                        IdUsuario = rd.GetInt32(rd.GetOrdinal("IdUsuario")),
                        IdRol = rd.GetInt32(rd.GetOrdinal("IdRol")),
                        Nombre = rd["Nombre"]?.ToString() ?? "",
                        Correo = rd["Correo"]?.ToString() ?? "",
                        NombreRol = rd["NombreRol"]?.ToString() ?? ""
                    };
                }

                // 3. Si no coincide en texto plano, intentar verificar como hash moderno
                // Los hashes tienen ~71-88 caracteres en Base64
                if (storedPasswordHash.Length >= 50 && 
                    !storedPasswordHash.Contains(" ") && 
                    Regex.IsMatch(storedPasswordHash, @"^[A-Za-z0-9+/=]+$"))
                {
                    try
                    {
                        // Intentar verificar como hash moderno
                        if (PasswordHasher.Verify(password, storedPasswordHash))
                        {
                            return new LoginResult
                            {
                                IdUsuario = rd.GetInt32(rd.GetOrdinal("IdUsuario")),
                                IdRol = rd.GetInt32(rd.GetOrdinal("IdRol")),
                                Nombre = rd["Nombre"]?.ToString() ?? "",
                                Correo = rd["Correo"]?.ToString() ?? "",
                                NombreRol = rd["NombreRol"]?.ToString() ?? ""
                            };
                        }
                    }
                    catch (Exception)
                    {
                        // Si falla la verificación (formato incorrecto de Base64), no permitir acceso
                    }
                }
            }

            return null;
        }
    }

    public class LoginResult
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string NombreRol { get; set; } = "";
    }
}
