using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks; // <-- NUEVO
using System;                 // <-- por excepciones/DateTime
using System.Linq;            // <-- para LINQ
using Microsoft.AspNetCore.Mvc.Rendering;

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
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : null,
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
                    // Buscar directamente con SQL para asegurar que encontramos el usuario incluso si tiene FechaEliminacion
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT 
                            IdUsuario,
                            IdRol,
                            Nombre,
                            Contraseña,
                            Correo,
                            Estado,
                            FechaEliminacion
                        FROM Usuarios
                        WHERE IdUsuario = @IdUsuario", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oUsuario.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                            oUsuario.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oUsuario.Nombre = dr["Nombre"].ToString();
                            oUsuario.Contraseña = dr["Contraseña"].ToString();
                            oUsuario.Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : null;
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
        // Retorna tupla (bool, string) para indicar éxito y mensaje de error si aplica
        public (bool Success, string ErrorMessage) MtdEliminarUsuario(int IdUsuario)
        {
            bool respuesta = false;
            string mensajeError = string.Empty;
            var conn = new Conexion();

             using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Primero eliminar registros relacionados en UsuariosExternalLogins
                    // Esto es necesario para usuarios creados con Google
                    using (var cmdDeleteExternal = new SqlCommand(@"
                        DELETE FROM [dbo].[UsuariosExternalLogins] 
                        WHERE [IdUsuario] = @IdUsuario
                    ", conexion))
                    {
                        cmdDeleteExternal.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                        cmdDeleteExternal.ExecuteNonQuery();
                    }
                    
                    // También eliminar tokens de GoogleLoginTokens si existen
                    using (var cmdDeleteTokens = new SqlCommand(@"
                        DELETE FROM [dbo].[GoogleLoginTokens] 
                        WHERE [Email] IN (
                            SELECT [Correo] FROM [dbo].[Usuarios] WHERE [IdUsuario] = @IdUsuario
                        )
                    ", conexion))
                    {
                        cmdDeleteTokens.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                        cmdDeleteTokens.ExecuteNonQuery();
                    }
                    
                    // También eliminar tokens de PasswordResetTokens si existen
                    using (var cmdDeletePasswordTokens = new SqlCommand(@"
                        DELETE FROM [dbo].[PasswordResetTokens] 
                        WHERE [IdUsuario] = @IdUsuario
                    ", conexion))
                    {
                        cmdDeletePasswordTokens.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                        cmdDeletePasswordTokens.ExecuteNonQuery();
                    }
                    
                    // Ahora eliminar el usuario usando el stored procedure que valida todo
                    using (var cmd = new SqlCommand("sp_UsuarioEliminar", conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;
                        cmd.ExecuteNonQuery();
                    }
                }

                respuesta = true;
           

            return (respuesta, mensajeError);
        }

        // Método legacy que mantiene compatibilidad (retorna solo bool)
        public bool MtdEliminarUsuarioLegacy(int IdUsuario)
        {
            var (success, _) = MtdEliminarUsuario(IdUsuario);
            return success;
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
        public async Task<int> CrearUsuarioBasicoAsync(string nombre, string correo, int rolId, string estado, string? passwordHash = null)
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new SqlConnection(conn.GetConnectionString());
                await conexion.OpenAsync();

                // Intentar usar el stored procedure primero
                try
                {
                    using var cmd = new SqlCommand("usp_Usuarios_CrearBasico", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    cmd.Parameters.AddWithValue("@IdRol", rolId);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    // Si no hay passwordHash, se usa 'EXTERNAL_LOGIN' para usuarios de Google
                    var passwordToSave = passwordHash ?? "EXTERNAL_LOGIN";
                    cmd.Parameters.AddWithValue("@Contraseña", passwordToSave);

                    using var dr = await cmd.ExecuteReaderAsync();
                    if (await dr.ReadAsync())
                    {
                        return dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : 0;
                    }
                }
                catch (SqlException sqlEx)
                {
                    // Si el stored procedure no existe, hacer INSERT directo
                    if (sqlEx.Number == 2812) // Object not found
                    {
                        // Fallback: INSERT directo si el SP no existe
                        using var cmdInsert = new SqlCommand(@"
                            IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Correo] = @Correo)
                            BEGIN
                                INSERT INTO [dbo].[Usuarios] ([IdRol], [Nombre], [Contraseña], [Estado], [Correo], [FechaCreacion])
                                VALUES (@IdRol, @Nombre, @Contraseña, @Estado, @Correo, GETDATE());
                                SELECT SCOPE_IDENTITY() AS IdUsuario;
                            END
                            ELSE
                            BEGIN
                                SELECT [IdUsuario] FROM [dbo].[Usuarios] WHERE [Correo] = @Correo;
                            END
                        ", conexion);
                        
                        cmdInsert.Parameters.AddWithValue("@Nombre", nombre);
                        cmdInsert.Parameters.AddWithValue("@Correo", correo);
                        cmdInsert.Parameters.AddWithValue("@IdRol", rolId);
                        cmdInsert.Parameters.AddWithValue("@Estado", estado);
                        var passwordToSave = passwordHash ?? "EXTERNAL_LOGIN";
                        cmdInsert.Parameters.AddWithValue("@Contraseña", passwordToSave);
                        
                        var result = await cmdInsert.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                    else
                    {
                        throw; // Re-lanzar si es otro error
                    }
                }
                
                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearUsuarioBasicoAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
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
        // Si yaVieneHasheada es true, se guarda directamente; si es false, el SP podría hashearla (depende de la implementación del SP)
        public async Task<(bool ok, string code)> ConsumirTokenResetAsync(string token, string nuevaContrasena, bool yaVieneHasheada = false)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_PasswordReset_Consumir", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.Parameters.AddWithValue("@NuevaContrasena", nuevaContrasena);
            // Si el SP necesita saber si ya viene hasheada, podríamos agregar otro parámetro
            // Por ahora, asumimos que el SP acepta el valor directamente

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

        // =================== GOOGLE LOGIN CONFIRMATION ===================

        // Crear token de confirmación para login con Google
        // Guarda temporalmente la información del usuario hasta que confirme
        public async Task<string?> CrearTokenConfirmacionGoogleAsync(
            string providerKey,
            string email,
            string displayName,
            string? ip,
            string? userAgent)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_GoogleLogin_CrearTokenConfirmacion", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProviderKey", providerKey);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@DisplayName", displayName ?? email);
            cmd.Parameters.AddWithValue("@MinutosValidez", 60); // 1 hora para confirmar
            cmd.Parameters.AddWithValue("@IpSolicitud", (object?)ip ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return dr["Token"] != DBNull.Value ? dr["Token"].ToString() : null;
            }
            return null;
        }

        // Confirmar login con Google y crear el usuario
        // Retorna los datos del usuario creado o null si falla
        public async Task<UsuarioDto?> ConfirmarGoogleLoginAsync(string token)
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new SqlConnection(conn.GetConnectionString());
                await conexion.OpenAsync();

                using var cmd = new SqlCommand("usp_GoogleLogin_Confirmar", conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Token", token);

                using var dr = await cmd.ExecuteReaderAsync();
                
                // Leer el resultset (ahora solo hay uno)
                if (await dr.ReadAsync())
                {
                    bool ok = dr["Ok"] != DBNull.Value && Convert.ToInt32(dr["Ok"]) == 1;
                    if (ok && dr["IdUsuario"] != DBNull.Value)
                    {
                        return new UsuarioDto
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            IdRol = dr["IdRol"] != DBNull.Value ? Convert.ToInt32(dr["IdRol"]) : ObtenerIdRolUsuarioPorDefecto(), // Rol Usuario por defecto
                            Nombre = dr["Nombre"]?.ToString(),
                            Correo = dr["Correo"]?.ToString(),
                            Estado = dr["Estado"]?.ToString() ?? "Activo",
                            NombreRol = dr.GetSchemaTable()?.Columns.Contains("NombreRol") == true ? dr["NombreRol"]?.ToString() : "Usuario"
                        };
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                System.Diagnostics.Debug.WriteLine($"Error en ConfirmarGoogleLoginAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        // Verificar si un token de confirmación de Google es válido (sin consumirlo)
        public async Task<bool> VerificarTokenConfirmacionGoogleAsync(string token)
        {
            var conn = new Conexion();
            using var conexion = new SqlConnection(conn.GetConnectionString());
            await conexion.OpenAsync();

            using var cmd = new SqlCommand("usp_GoogleLogin_VerificarToken", conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Token", token);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return dr["Valido"] != DBNull.Value && Convert.ToInt32(dr["Valido"]) == 1;
            }
            return false;
        }

        // Método auxiliar para obtener el IdRol de Usuario por defecto
        private int ObtenerIdRolUsuarioPorDefecto()
        {
            try
            {
                var conn = new Conexion();
                using var conexion = new SqlConnection(conn.GetConnectionString());
                conexion.Open();
                
                using var cmd = new SqlCommand("SELECT TOP 1 [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Usuario' AND [FechaEliminacion] IS NULL ORDER BY [IdRol]", conexion);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                // Si falla, retornar 5 como fallback (asumiendo que Usuario siempre será 5 o el último)
            }
            return 5; // Fallback si no se encuentra
        }

        // --- LISTAR ROLES ACTIVOS PARA COMBOBOX ---
        public List<SelectListItem> MtdListarRolesActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT IdRol, NombreRol 
                        FROM Roles 
                        WHERE FechaEliminacion IS NULL
                        ORDER BY NombreRol", conexion);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = dr["IdRol"].ToString(),
                                Text = $"{dr["IdRol"]} - {dr["NombreRol"]}"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar roles activos: {ex.Message}");
            }

            return lista;
        }

        // --- VERIFICAR SI EL NOMBRE DE USUARIO YA EXISTE ---
        public bool MtdVerificarNombreExiste(string nombre, int? idUsuarioExcluir = null)
        {
            var conn = new Conexion();
            
            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Usuarios 
                        WHERE Nombre = @Nombre 
                          AND FechaEliminacion IS NULL";
                    
                    if (idUsuarioExcluir.HasValue)
                    {
                        query += " AND IdUsuario != @IdUsuario";
                    }
                    
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    
                    if (idUsuarioExcluir.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuarioExcluir.Value);
                    }
                    
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar nombre de usuario: {ex.Message}");
                return false;
            }
        }

        // --- VERIFICAR SI EL USUARIO YA FUE ELIMINADO ---
        public bool MtdVerificarUsuarioEliminado(int idUsuario)
        {
            var conn = new Conexion();
            
            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM Usuarios 
                        WHERE IdUsuario = @IdUsuario 
                          AND FechaEliminacion IS NOT NULL", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar si el usuario fue eliminado: {ex.Message}");
                return false;
            }
        }
    }
}
