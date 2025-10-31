using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

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

        // Método que valida usuario usando tu SP sp_UsuarioLogin_Secure
        public async Task<LoginResult?> ValidarUsuarioAsync(string correo, string? password)
        {
            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("dbo.sp_UsuarioLogin_Secure", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Correo", (object?)correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Password", (object?)password ?? DBNull.Value);

            await cn.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
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
