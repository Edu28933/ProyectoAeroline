using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class AerolineasData
    {
        // Consultar todas las aerolíneas
        public List<AerolineasModel> MtdConsultarAerolineas()
        {
            var lista = new List<AerolineasModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarAerolineas", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new AerolineasModel
                            {
                                IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]),
                                IdEmpleado = dr["IdEmpleado"] != DBNull.Value ? Convert.ToInt32(dr["IdEmpleado"]) : null,
                                IATA = dr["IATA"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Pais = dr["Pais"].ToString(),
                                Ciudad = dr["Ciudad"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                Telefono = dr["Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Telefono"]) : null,
                                Estado = dr["Estado"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return lista;
        }

        // Agregar una aerolínea
        public bool MtdAgregarAerolinea(AerolineasModel oAerolinea)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarAerolinea", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpleado", oAerolinea.IdEmpleado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IATA", oAerolinea.IATA);
                    cmd.Parameters.AddWithValue("@Nombre", oAerolinea.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", oAerolinea.Pais ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", oAerolinea.Ciudad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Direccion", oAerolinea.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", oAerolinea.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oAerolinea.Estado ?? (object)DBNull.Value);
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

        // Editar una aerolínea
        public bool MtdEditarAerolinea(AerolineasModel oAerolinea)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarAerolinea", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAerolinea", oAerolinea.IdAerolinea);
                    cmd.Parameters.AddWithValue("@IdEmpleado", oAerolinea.IdEmpleado ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IATA", oAerolinea.IATA);
                    cmd.Parameters.AddWithValue("@Nombre", oAerolinea.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", oAerolinea.Pais ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Ciudad", oAerolinea.Ciudad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Direccion", oAerolinea.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", oAerolinea.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oAerolinea.Estado ?? (object)DBNull.Value);
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

        // Buscar una aerolínea por Id
        public AerolineasModel MtdBuscarAerolinea(int IdAerolinea)
        {
            var oAerolinea = new AerolineasModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarAerolinea", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAerolinea", IdAerolinea);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oAerolinea.IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]);
                            oAerolinea.IdEmpleado = dr["IdEmpleado"] != DBNull.Value ? Convert.ToInt32(dr["IdEmpleado"]) : null;
                            oAerolinea.IATA = dr["IATA"].ToString();
                            oAerolinea.Nombre = dr["Nombre"].ToString();
                            oAerolinea.Pais = dr["Pais"].ToString();
                            oAerolinea.Ciudad = dr["Ciudad"].ToString();
                            oAerolinea.Direccion = dr["Direccion"].ToString();
                            oAerolinea.Telefono = dr["Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Telefono"]) : null;
                            oAerolinea.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oAerolinea;
        }

        // Eliminar una aerolínea
        public bool MtdEliminarAerolinea(int IdAerolinea)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarAerolinea", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAerolinea", IdAerolinea);
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
    }
}
