using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class RolPantallaOpcionData
    {
        // Método que consulta todos los RolPantallaOpcion con información relacionada
        public List<RolPantallaOpcionModel> MtdConsultarRolPantallaOpciones()
        {
            var lista = new List<RolPantallaOpcionModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    // Usar stored procedure
                    SqlCommand cmd = new SqlCommand("sp_RolPantallaOpcionesSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120 // Aumentar timeout a 2 minutos
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new RolPantallaOpcionModel
                            {
                                IdRolPantallaOpcion = Convert.ToInt32(dr["IdRolPantallaOpcion"]),
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                IdPantalla = Convert.ToInt32(dr["IdPantalla"]),
                                Ver = dr["Ver"] != DBNull.Value && Convert.ToBoolean(dr["Ver"]),
                                Crear = dr["Crear"] != DBNull.Value && Convert.ToBoolean(dr["Crear"]),
                                Editar = dr["Editar"] != DBNull.Value && Convert.ToBoolean(dr["Editar"]),
                                Eliminar = dr["Eliminar"] != DBNull.Value && Convert.ToBoolean(dr["Eliminar"]),
                                // Mapear Estado desde FechaEliminacion o el campo Estado si existe
                                Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Activo",
                                NombreRol = dr["NombreRol"] != DBNull.Value ? dr["NombreRol"].ToString() : null,
                                NombrePantalla = dr["NombrePantalla"] != DBNull.Value ? dr["NombrePantalla"].ToString() : null
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en MtdConsultarRolPantallaOpciones: {ex.Message}");
                }
            }

            return lista;
        }

        // Método que agrega un RolPantallaOpcion
        public bool MtdAgregarRolPantallaOpcion(RolPantallaOpcionModel oRolPantallaOpcion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_RolPantallaOpcionAgregar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120 // Aumentar timeout a 2 minutos
                    };
                    cmd.Parameters.AddWithValue("@IdRol", oRolPantallaOpcion.IdRol);
                    cmd.Parameters.AddWithValue("@IdPantalla", oRolPantallaOpcion.IdPantalla);
                    cmd.Parameters.AddWithValue("@Ver", oRolPantallaOpcion.Ver);
                    cmd.Parameters.AddWithValue("@Crear", oRolPantallaOpcion.Crear);
                    cmd.Parameters.AddWithValue("@Editar", oRolPantallaOpcion.Editar);
                    cmd.Parameters.AddWithValue("@Eliminar", oRolPantallaOpcion.Eliminar);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oRolPantallaOpcion.Estado ?? "Activo");
                    cmd.Parameters.AddWithValue("@UsuarioCreacion", DBNull.Value); // Puedes obtenerlo del contexto si lo necesitas
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MtdAgregarRolPantallaOpcion: {ex.Message}");
            }

            return respuesta;
        }

        // Método que busca un RolPantallaOpcion
        public RolPantallaOpcionModel MtdBuscarRolPantallaOpcion(int IdRolPantallaOpcion)
        {
            var oRolPantallaOpcion = new RolPantallaOpcionModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_RolPantallaOpcionBuscar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120 // Aumentar timeout a 2 minutos
                    };
                    cmd.Parameters.AddWithValue("@IdRolPantallaOpcion", IdRolPantallaOpcion);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oRolPantallaOpcion.IdRolPantallaOpcion = Convert.ToInt32(dr["IdRolPantallaOpcion"]);
                            oRolPantallaOpcion.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oRolPantallaOpcion.IdPantalla = Convert.ToInt32(dr["IdPantalla"]);
                            oRolPantallaOpcion.Ver = dr["Ver"] != DBNull.Value && Convert.ToBoolean(dr["Ver"]);
                            oRolPantallaOpcion.Crear = dr["Crear"] != DBNull.Value && Convert.ToBoolean(dr["Crear"]);
                            oRolPantallaOpcion.Editar = dr["Editar"] != DBNull.Value && Convert.ToBoolean(dr["Editar"]);
                            oRolPantallaOpcion.Eliminar = dr["Eliminar"] != DBNull.Value && Convert.ToBoolean(dr["Eliminar"]);
                            // Mapear Estado desde FechaEliminacion o el campo Estado si existe
                            oRolPantallaOpcion.Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "Activo";
                            oRolPantallaOpcion.NombreRol = dr["NombreRol"] != DBNull.Value ? dr["NombreRol"].ToString() : null;
                            oRolPantallaOpcion.NombrePantalla = dr["NombrePantalla"] != DBNull.Value ? dr["NombrePantalla"].ToString() : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MtdBuscarRolPantallaOpcion: {ex.Message}");
            }

            return oRolPantallaOpcion;
        }

        // Método que edita un RolPantallaOpcion
        public bool MtdEditarRolPantallaOpcion(RolPantallaOpcionModel oRolPantallaOpcion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_RolPantallaOpcionModificar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120 // Aumentar timeout a 2 minutos
                    };
                    cmd.Parameters.AddWithValue("@IdRolPantallaOpcion", oRolPantallaOpcion.IdRolPantallaOpcion);
                    cmd.Parameters.AddWithValue("@IdRol", oRolPantallaOpcion.IdRol);
                    cmd.Parameters.AddWithValue("@IdPantalla", oRolPantallaOpcion.IdPantalla);
                    cmd.Parameters.AddWithValue("@Ver", oRolPantallaOpcion.Ver);
                    cmd.Parameters.AddWithValue("@Crear", oRolPantallaOpcion.Crear);
                    cmd.Parameters.AddWithValue("@Editar", oRolPantallaOpcion.Editar);
                    cmd.Parameters.AddWithValue("@Eliminar", oRolPantallaOpcion.Eliminar);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oRolPantallaOpcion.Estado ?? "Activo");
                    cmd.Parameters.AddWithValue("@UsuarioActualizacion", DBNull.Value); // Puedes obtenerlo del contexto si lo necesitas
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MtdEditarRolPantallaOpcion: {ex.Message}");
            }

            return respuesta;
        }

        // Método que elimina un RolPantallaOpcion
        public bool MtdEliminarRolPantallaOpcion(int IdRolPantallaOpcion)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_RolPantallaOpcionEliminar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120 // Aumentar timeout a 2 minutos
                    };
                    cmd.Parameters.AddWithValue("@IdRolPantallaOpcion", IdRolPantallaOpcion);
                    cmd.Parameters.AddWithValue("@UsuarioEliminacion", DBNull.Value); // Puedes obtenerlo del contexto si lo necesitas
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MtdEliminarRolPantallaOpcion: {ex.Message}");
                respuesta = false;
            }

            return respuesta;
        }
    }
}

