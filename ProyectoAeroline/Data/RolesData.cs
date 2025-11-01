using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class RolesData
    {
        // Método que consulta todos los roles
        public List<RolesModel> MtdConsultarRoles()
        {
            var listaRoles = new List<RolesModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SELECT [IdRol], [NombreRol], [Descripcion], [Estado] FROM [dbo].[Roles] WHERE [Estado] != 'Eliminado' OR [Estado] IS NULL ORDER BY [IdRol]", conexion)
                    {
                        CommandType = CommandType.Text
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaRoles.Add(new RolesModel
                            {
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                NombreRol = dr["NombreRol"]?.ToString() ?? "",
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                                Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Activo"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaRoles;
        }

        // Método que agrega un rol
        public bool MtdAgregarRol(RolesModel oRol)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO [dbo].[Roles] ([NombreRol], [Descripcion], [Estado])
                        VALUES (@NombreRol, @Descripcion, @Estado)
                    ", conexion);
                    cmd.Parameters.AddWithValue("@NombreRol", oRol.NombreRol);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oRol.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oRol.Estado ?? "Activo");
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return respuesta;
        }

        // Método que busca un rol
        public RolesModel MtdBuscarRol(int IdRol)
        {
            var oRol = new RolesModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SELECT [IdRol], [NombreRol], [Descripcion], [Estado] FROM [dbo].[Roles] WHERE [IdRol] = @IdRol", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", IdRol);
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oRol.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oRol.NombreRol = dr["NombreRol"]?.ToString() ?? "";
                            oRol.Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null;
                            oRol.Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Activo";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oRol;
        }

        // Método que edita un rol
        public bool MtdEditarRol(RolesModel oRol)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE [dbo].[Roles]
                        SET [NombreRol] = @NombreRol,
                            [Descripcion] = @Descripcion,
                            [Estado] = @Estado
                        WHERE [IdRol] = @IdRol
                    ", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", oRol.IdRol);
                    cmd.Parameters.AddWithValue("@NombreRol", oRol.NombreRol);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oRol.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oRol.Estado ?? "Activo");
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return respuesta;
        }

        // Método que elimina un rol
        public bool MtdEliminarRol(int IdRol)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Verificar si hay usuarios usando este rol
                    using (var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM [dbo].[Usuarios] WHERE [IdRol] = @IdRol", conexion))
                    {
                        cmdCheck.Parameters.AddWithValue("@IdRol", IdRol);
                        var count = (int)cmdCheck.ExecuteScalar();
                        
                        if (count > 0)
                        {
                            // Si hay usuarios, solo cambiar el estado
                            SqlCommand cmd = new SqlCommand(@"
                                UPDATE [dbo].[Roles]
                                SET [Estado] = 'Inactivo'
                                WHERE [IdRol] = @IdRol
                            ", conexion);
                            cmd.Parameters.AddWithValue("@IdRol", IdRol);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // Si no hay usuarios, eliminar físicamente
                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Roles] WHERE [IdRol] = @IdRol", conexion);
                            cmd.Parameters.AddWithValue("@IdRol", IdRol);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar rol: {ex.Message}");
                respuesta = false;
            }

            return respuesta;
        }
    }
}

