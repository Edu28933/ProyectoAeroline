using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class ServiciosData
    {
        // Método que consulta todos los servicios
        public List<ServiciosModel> MtdConsultarServicios()
        {
            var listaServicios = new List<ServiciosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ServiciosSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaServicios.Add(new ServiciosModel
                            {
                                IdServicio = Convert.ToInt32(dr["IdServicio"]),
                                IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                Fecha = dr["Fecha"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["Fecha"]),
                                TipoServicio = dr["TipoServicio"].ToString() ?? string.Empty,
                                Costo = Convert.ToDecimal(dr["Costo"]),
                                Cantidad = dr["Cantidad"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["Cantidad"]),
                                CostoTotal = dr["CostoTotal"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CostoTotal"]),
                                Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaServicios;
        }

        // Método que agrega un servicio
        public bool MtdAgregarServicio(ServiciosModel oServicio)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ServicioAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oServicio.IdBoleto);
                    cmd.Parameters.AddWithValue("@Fecha", (object?)oServicio.Fecha ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoServicio", oServicio.TipoServicio);
                    cmd.Parameters.AddWithValue("@Costo", oServicio.Costo);
                    cmd.Parameters.AddWithValue("@Cantidad", (object?)oServicio.Cantidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostoTotal", (object?)oServicio.CostoTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oServicio.Estado ?? DBNull.Value);
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

        // Método que actualiza un servicio
        public bool MtdEditarServicio(ServiciosModel oServicio)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ServicioModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdServicio", oServicio.IdServicio);
                    cmd.Parameters.AddWithValue("@IdBoleto", oServicio.IdBoleto);
                    cmd.Parameters.AddWithValue("@Fecha", (object?)oServicio.Fecha ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoServicio", oServicio.TipoServicio);
                    cmd.Parameters.AddWithValue("@Costo", oServicio.Costo);
                    cmd.Parameters.AddWithValue("@Cantidad", (object?)oServicio.Cantidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostoTotal", (object?)oServicio.CostoTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oServicio.Estado ?? DBNull.Value);
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

        // Método que busca un servicio por su Id
        public ServiciosModel MtdBuscarServicio(int IdServicio)
        {
            var oServicio = new ServiciosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ServicioBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdServicio", IdServicio);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oServicio.IdServicio = Convert.ToInt32(dr["IdServicio"]);
                            oServicio.IdBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            oServicio.Fecha = dr["Fecha"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["Fecha"]);
                            oServicio.TipoServicio = dr["TipoServicio"].ToString() ?? string.Empty;
                            oServicio.Costo = Convert.ToDecimal(dr["Costo"]);
                            oServicio.Cantidad = dr["Cantidad"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["Cantidad"]);
                            oServicio.CostoTotal = dr["CostoTotal"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CostoTotal"]);
                            oServicio.Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oServicio;
        }

        // Método que elimina un servicio
        public bool MtdEliminarServicio(int IdServicio)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ServicioEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdServicio", IdServicio);
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

        // Método para listar boletos activos (para el dropdown)
        public List<BoletosModel> MtdListarBoletosActivos()
        {
            var lista = new List<BoletosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BoletosSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var estado = dr["Estado"]?.ToString();
                            if (estado == "Activo")
                            {
                                lista.Add(new BoletosModel
                                {
                                    IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                    IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                    IdPasajero = Convert.ToInt32(dr["IdPasajero"])
                                });
                            }
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
    }
}

