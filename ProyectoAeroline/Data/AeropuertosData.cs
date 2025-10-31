using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;
using static ProyectoAeroline.Models.AvionesModel;
namespace ProyectoAeroline.Data
{
    public class AeropuertosData
    {
        // Método que consulta todos los aeropuertos
        public List<AeropuertosModel> MtdConsultarAeropuertos()
        {
            var listaAeropuertos = new List<AeropuertosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarAeropuertos", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaAeropuertos.Add(new AeropuertosModel
                            {
                                IdAeropuerto = Convert.ToInt32(dr["IdAeropuerto"]),
                                IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]),
                                IATA = dr["IATA"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Pais = dr["Pais"].ToString(),
                                Ciudad = dr["Ciudad"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                Telefono = Convert.ToInt32(dr["Telefono"]),
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

            return listaAeropuertos;
        }

        // Método que agrega un aeropuerto
        public bool MtdAgregarAeropuerto(AeropuertosModel oAeropuerto)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarAeropuerto", conexion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", oAeropuerto.IdEmpleado);
                    cmd.Parameters.AddWithValue("@IATA", oAeropuerto.IATA);
                    cmd.Parameters.AddWithValue("@Nombre", oAeropuerto.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", oAeropuerto.Pais);
                    cmd.Parameters.AddWithValue("@Ciudad", oAeropuerto.Ciudad);
                    cmd.Parameters.AddWithValue("@Direccion", oAeropuerto.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", oAeropuerto.Telefono);
                    cmd.Parameters.AddWithValue("@Estado", oAeropuerto.Estado);
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

        // Método que actualiza un aeropuerto
        public bool MtdEditarAeropuerto(AeropuertosModel oAeropuerto)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarAeropuerto", conexion);
                    cmd.Parameters.AddWithValue("@IdAeropuerto", oAeropuerto.IdAeropuerto);
                    cmd.Parameters.AddWithValue("@IdEmpleado", oAeropuerto.IdEmpleado);
                    cmd.Parameters.AddWithValue("@IATA", oAeropuerto.IATA);
                    cmd.Parameters.AddWithValue("@Nombre", oAeropuerto.Nombre);
                    cmd.Parameters.AddWithValue("@Pais", oAeropuerto.Pais);
                    cmd.Parameters.AddWithValue("@Ciudad", oAeropuerto.Ciudad);
                    cmd.Parameters.AddWithValue("@Direccion", oAeropuerto.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", oAeropuerto.Telefono);
                    cmd.Parameters.AddWithValue("@Estado", oAeropuerto.Estado);
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

        // Método que busca un aeropuerto por su Id
        public AeropuertosModel MtdBuscarAeropuerto(int IdAeropuerto)
        {
            var oAeropuerto = new AeropuertosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarAeropuerto", conexion);
                    cmd.Parameters.AddWithValue("@IdAeropuerto", IdAeropuerto);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oAeropuerto.IdAeropuerto = Convert.ToInt32(dr["IdAeropuerto"]);
                            oAeropuerto.IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]);
                            oAeropuerto.IATA = dr["IATA"].ToString();
                            oAeropuerto.Nombre = dr["Nombre"].ToString();
                            oAeropuerto.Pais = dr["Pais"].ToString();
                            oAeropuerto.Ciudad = dr["Ciudad"].ToString();
                            oAeropuerto.Direccion = dr["Direccion"].ToString();
                            oAeropuerto.Telefono = Convert.ToInt32(dr["Telefono"]);
                            oAeropuerto.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oAeropuerto;
        }

        // Método que elimina un aeropuerto
        public bool MtdEliminarAeropuerto(int IdAeropuerto)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarAeropuerto", conexion);
                    cmd.Parameters.AddWithValue("@IdAeropuerto", IdAeropuerto);
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

        //MÉTODO QUE BUSCA Y LISTA LOS ID PARA AGREGAR
        public List<EmpleadosModel> MtdObtenerEmpleados()
        {
            List<EmpleadosModel> lista = new List<EmpleadosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("usp_EmpleadosListar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new EmpleadosModel
                            {
                                IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]),
                                Nombre = dr["Nombre"].ToString()  // viene como "1 - Juan Pérez"
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
