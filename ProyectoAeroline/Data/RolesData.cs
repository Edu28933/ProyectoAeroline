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
                    // Usar stored procedure
                    SqlCommand cmd = new SqlCommand("sp_RolesSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaRoles.Add(new RolesModel
                            {
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                NombreRol = dr["NombreRol"]?.ToString() ?? "",
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
                    SqlCommand cmd = new SqlCommand("sp_RolAgregar", conexion);
                    cmd.Parameters.AddWithValue("@NombreRol", oRol.NombreRol);
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
                    SqlCommand cmd = new SqlCommand("sp_RolBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", IdRol);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oRol.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oRol.NombreRol = dr["NombreRol"]?.ToString() ?? "";
                            oRol.Descripcion = null; // La tabla no tiene este campo
                            oRol.Estado = dr["FechaEliminacion"] == DBNull.Value ? "Activo" : "Eliminado"; // Usar eliminación lógica
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
                    SqlCommand cmd = new SqlCommand("sp_RolModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", oRol.IdRol);
                    cmd.Parameters.AddWithValue("@NombreRol", oRol.NombreRol);
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
                    SqlCommand cmd = new SqlCommand("sp_RolEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", IdRol);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
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

