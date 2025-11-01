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
                    // Usar stored procedure
                    SqlCommand cmd = new SqlCommand("sp_PantallasSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaPantallas.Add(new PantallasModel
                            {
                                IdPantalla = Convert.ToInt32(dr["IdPantalla"]),
                                NombrePantalla = dr["NombrePantalla"]?.ToString() ?? "",
                                Ruta = null, // La tabla no tiene este campo
                                Icono = null, // La tabla no tiene este campo
                                Descripcion = null, // La tabla no tiene este campo
                                Estado = dr["FechaEliminacion"] == DBNull.Value ? "Activo" : "Eliminado" // Usar eliminación lógica
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
                    SqlCommand cmd = new SqlCommand("sp_PantallaAgregar", conexion);
                    cmd.Parameters.AddWithValue("@NombrePantalla", oPantalla.NombrePantalla);
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", DBNull.Value); // Puedes obtenerlo del contexto si lo necesitas
                    cmd.CommandType = CommandType.StoredProcedure;
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
                    SqlCommand cmd = new SqlCommand("sp_PantallaBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", IdPantalla);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oPantalla.IdPantalla = Convert.ToInt32(dr["IdPantalla"]);
                            oPantalla.NombrePantalla = dr["NombrePantalla"]?.ToString() ?? "";
                            oPantalla.Ruta = null; // La tabla no tiene este campo
                            oPantalla.Icono = null; // La tabla no tiene este campo
                            oPantalla.Descripcion = null; // La tabla no tiene este campo
                            oPantalla.Estado = dr["FechaEliminacion"] == DBNull.Value ? "Activo" : "Eliminado"; // Usar eliminación lógica
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
                    SqlCommand cmd = new SqlCommand("sp_PantallaModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", oPantalla.IdPantalla);
                    cmd.Parameters.AddWithValue("@NombrePantalla", oPantalla.NombrePantalla);
                    cmd.Parameters.AddWithValue("@UsuarioActualizacion", DBNull.Value); // Puedes obtenerlo del contexto si lo necesitas
                    cmd.CommandType = CommandType.StoredProcedure;
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
                    SqlCommand cmd = new SqlCommand("sp_PantallaEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdPantalla", IdPantalla);
                    cmd.CommandType = CommandType.StoredProcedure;
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

