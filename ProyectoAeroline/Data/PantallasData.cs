using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class PantallasData
    {
        // Método que consulta todas las pantallas
        public List<PantallasModel> MtdConsultarPantallas()
        {
            var listaPantallas = new List<PantallasModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT [IdPantalla], [NombrePantalla], [Ruta], [Icono], [Descripcion], [Estado] 
                        FROM [dbo].[Pantallas] 
                        WHERE [Estado] != 'Eliminado' OR [Estado] IS NULL 
                        ORDER BY [IdPantalla]
                    ", conexion)
                    {
                        CommandType = CommandType.Text
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaPantallas.Add(new PantallasModel
                            {
                                IdPantalla = Convert.ToInt32(dr["IdPantalla"]),
                                NombrePantalla = dr["NombrePantalla"]?.ToString() ?? "",
                                Ruta = dr["Ruta"] != DBNull.Value ? dr["Ruta"].ToString() : null,
                                Icono = dr["Icono"] != DBNull.Value ? dr["Icono"].ToString() : null,
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

            return listaPantallas;
        }

        // Método que agrega una pantalla
        public bool MtdAgregarPantalla(PantallasModel oPantalla)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [Ruta], [Icono], [Descripcion], [Estado])
                        VALUES (@NombrePantalla, @Ruta, @Icono, @Descripcion, @Estado)
                    ", conexion);
                    cmd.Parameters.AddWithValue("@NombrePantalla", oPantalla.NombrePantalla);
                    cmd.Parameters.AddWithValue("@Ruta", (object?)oPantalla.Ruta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Icono", (object?)oPantalla.Icono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oPantalla.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oPantalla.Estado ?? "Activo");
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

        // Método que busca una pantalla
        public PantallasModel MtdBuscarPantalla(int IdPantalla)
        {
            var oPantalla = new PantallasModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT [IdPantalla], [NombrePantalla], [Ruta], [Icono], [Descripcion], [Estado] 
                        FROM [dbo].[Pantallas] 
                        WHERE [IdPantalla] = @IdPantalla
                    ", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", IdPantalla);
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oPantalla.IdPantalla = Convert.ToInt32(dr["IdPantalla"]);
                            oPantalla.NombrePantalla = dr["NombrePantalla"]?.ToString() ?? "";
                            oPantalla.Ruta = dr["Ruta"] != DBNull.Value ? dr["Ruta"].ToString() : null;
                            oPantalla.Icono = dr["Icono"] != DBNull.Value ? dr["Icono"].ToString() : null;
                            oPantalla.Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null;
                            oPantalla.Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Activo";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oPantalla;
        }

        // Método que edita una pantalla
        public bool MtdEditarPantalla(PantallasModel oPantalla)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE [dbo].[Pantallas]
                        SET [NombrePantalla] = @NombrePantalla,
                            [Ruta] = @Ruta,
                            [Icono] = @Icono,
                            [Descripcion] = @Descripcion,
                            [Estado] = @Estado
                        WHERE [IdPantalla] = @IdPantalla
                    ", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", oPantalla.IdPantalla);
                    cmd.Parameters.AddWithValue("@NombrePantalla", oPantalla.NombrePantalla);
                    cmd.Parameters.AddWithValue("@Ruta", (object?)oPantalla.Ruta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Icono", (object?)oPantalla.Icono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oPantalla.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oPantalla.Estado ?? "Activo");
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

        // Método que elimina una pantalla
        public bool MtdEliminarPantalla(int IdPantalla)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Primero eliminar relaciones en RolesPantallas si existen
                    using (var cmdDeleteRel = new SqlCommand(@"
                        DELETE FROM [dbo].[RolesPantallas] 
                        WHERE [IdPantalla] = @IdPantalla
                    ", conexion))
                    {
                        cmdDeleteRel.Parameters.AddWithValue("@IdPantalla", IdPantalla);
                        cmdDeleteRel.ExecuteNonQuery();
                    }
                    
                    // Ahora eliminar la pantalla
                    SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Pantallas] WHERE [IdPantalla] = @IdPantalla", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", IdPantalla);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar pantalla: {ex.Message}");
                respuesta = false;
            }

            return respuesta;
        }
    }
}

