using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks; // <-- NUEVO
using System;                 // <-- por excepciones/DateTime

namespace ProyectoAeroline.Data
{
    public class UsuariosData
    {
        // ===== DTO liviano para los métodos nuevos =====
        public class UsuarioDto
        {
            public int IdUsuario { get; set; }
            public int IdRol { get; set; }
            public string? Nombre { get; set; }
            public string? Correo { get; set; }
            public string? Estado { get; set; }
            public string? NombreRol { get; set; } // puede venir null, el controlador ya pone "Usuario" por defecto
        }

        // =================== LO QUE YA TENÍAS ===================

        // Método que consulta todos los usuarios
        public List<UsuariosModel> MtdConsultarUsuarios()
        {
            var listaUsuarios = new List<UsuariosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuariosSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaUsuarios.Add(new UsuariosModel
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                Nombre = dr["Nombre"].ToString(),
                                Contraseña = dr["Contraseña"].ToString(), // en DB es 'Contraseña'
                                Estado = dr["Estado"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaUsuarios;
        }

        // Método que agrega un usuario
        public bool MtdAgregarUsuario(UsuariosModel oUsuario)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", oUsuario.IdRol);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@Contrasena", oUsuario.Contraseña);
                    cmd.Parameters.AddWithValue("@Estado", oUsuario.Estado);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                respuesta = false;
            }

            return respuesta;
        }

        // Método que actualiza un usuario
        public bool MtdEditarUsuario(UsuariosModel oUsuario)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdRol", oUsuario.IdRol);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@Contrasena", oUsuario.Contraseña);
                    cmd.Parameters.AddWithValue("@Estado", oUsuario.Estado);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                respuesta = false;
            }

            return respuesta;
        }

        // Método que busca un usuario por su Id
        public UsuariosModel MtdBuscarUsuario(int IdUsuario)
        {
            var oUsuario = new UsuariosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oUsuario.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                            oUsuario.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oUsuario.Nombre = dr["Nombre"].ToString();
                            oUsuario.Contraseña = dr["Contraseña"].ToString(); // en DB es 'Contraseña'
                            oUsuario.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oUsuario;
        }

        // Método que elimina un usuario
        public bool MtdEliminarUsuario(int IdUsuario)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                respuesta = false;
            }

            return respuesta;
        }

        // =================== NUEVOS MÉTODOS (ASÍNCRONOS) ===================

        // Buscar usuario por mapping externo (proveedor + providerKey)
        public async Task<UsuarioDto?> BuscarPorExternalLoginAsync(string provider, string providerKey)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_UsuariosExternalLogins_Buscar", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Proveedor", provider);
            cmd.Parameters.AddWithValue("@ProviderKey", providerKey);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new UsuarioDto
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    IdRol = dr["IdRol"] != DBNull.Value ? Convert.ToInt32(dr["IdRol"]) : 0,
                    Nombre = dr["Nombre"]?.ToString(),
                    Correo = dr["Correo"]?.ToString(),
                    Estado = dr["Estado"]?.ToString(),
                    // Si el SP expone NombreRol, lo tomamos; si no, queda null
                    NombreRol = dr.GetSchemaTable()?.Columns.Contains("NombreRol") == true ? dr["NombreRol"]?.ToString() : null
                };
            }
            return null;
        }

        // Buscar usuario por correo
        public async Task<UsuarioDto?> BuscarPorCorreoAsync(string correo)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_Usuarios_ObtenerPorCorreo", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Correo", correo);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new UsuarioDto
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    IdRol = dr["IdRol"] != DBNull.Value ? Convert.ToInt32(dr["IdRol"]) : 0,
                    Nombre = dr["Nombre"]?.ToString(),
                    Correo = dr["Correo"]?.ToString(),
                    Estado = dr["Estado"]?.ToString(),
                    NombreRol = dr.GetSchemaTable()?.Columns.Contains("NombreRol") == true ? dr["NombreRol"]?.ToString() : null
                };
            }
            return null;
        }

        // Crear usuario básico (para alta por Google/registro simple)
        public async Task<int> CrearUsuarioBasicoAsync(string nombre, string correo, int rolId, string estado)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_Usuarios_CrearBasico", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Correo", correo);
            cmd.Parameters.AddWithValue("@IdRol", rolId);
            cmd.Parameters.AddWithValue("@Estado", estado);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : 0;
            }
            return 0;
        }

        // Guardar mapping externo (proveedor→usuario)
        public async Task GuardarExternalLoginAsync(int usuarioId, string provider, string providerKey, string? emailProveedor, string? displayName, string? avatarUrl)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_UsuariosExternalLogins_Agregar", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);
            cmd.Parameters.AddWithValue("@Proveedor", provider);
            cmd.Parameters.AddWithValue("@ProviderKey", providerKey);
            cmd.Parameters.AddWithValue("@EmailProveedor", (object?)emailProveedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DisplayName", (object?)displayName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AvatarUrl", (object?)avatarUrl ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // Registrar acceso externo (opcional)
        public async Task RegistrarAccesoExternoAsync(string provider, string providerKey, string? ip)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_UsuariosExternalLogins_RegistrarAcceso", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Proveedor", provider);
            cmd.Parameters.AddWithValue("@ProviderKey", providerKey);
            cmd.Parameters.AddWithValue("@IP", (object?)ip ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // Crear token de reset (devuelve token e IdUsuario; puede devolver null por seguridad)
        public async Task<(string? token, int? idUsuario)> CrearTokenResetAsync(string correo, string? ip, string? userAgent)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_PasswordReset_CrearToken", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Correo", correo);
            cmd.Parameters.AddWithValue("@MinutosValidez", 30);
            cmd.Parameters.AddWithValue("@IpSolicitud", (object?)ip ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                string? token = dr["Token"] != DBNull.Value ? dr["Token"].ToString() : null;
                int? id = dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : (int?)null;
                return (token, id);
            }
            return (null, null);
        }

        // Consumir token de reset
        public async Task<(bool ok, string code)> ConsumirTokenResetAsync(string token, string nuevaContrasena)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_PasswordReset_Consumir", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@NuevaContrasena", nuevaContrasena);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                bool ok = dr["Ok"] != DBNull.Value && Convert.ToInt32(dr["Ok"]) == 1;
                string code = dr["Codigo"]?.ToString() ?? "";
                return (ok, code);
            }
            return (false, "ERROR_DESCONOCIDO");
        }

        // Confirmar verificación de email (si lo implementaste)
        public async Task<(bool ok, string code)> ConfirmarVerificacionAsync(string token)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_EmailVerificacion_Confirmar", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Token", token);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                bool ok = dr["Ok"] != DBNull.Value && Convert.ToInt32(dr["Ok"]) == 1;
                string code = dr["Codigo"]?.ToString() ?? "";
                return (ok, code);
            }
            return (false, "ERROR_DESCONOCIDO");
        }
    }
}
