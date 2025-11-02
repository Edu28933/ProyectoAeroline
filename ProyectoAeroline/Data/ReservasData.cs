using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class ReservasData
    {
        // Método que consulta todas las reservas
        public List<ReservasModel> MtdConsultarReservas()
        {
            var listaReservas = new List<ReservasModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReservasSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaReservas.Add(new ReservasModel
                            {
                                IdReserva = Convert.ToInt32(dr["IdReserva"]),
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                FechaReserva = Convert.ToDateTime(dr["FechaReserva"]),
                                MontoAnticipo = dr["MontoAnticipo"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["MontoAnticipo"]),
                                FechaVuelo = dr["FechaVuelo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaVuelo"]),
                                Observaciones = dr["Observaciones"] == DBNull.Value ? null : dr["Observaciones"].ToString(),
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

            return listaReservas;
        }

        // Método que agrega una reserva
        public bool MtdAgregarReserva(ReservasModel oReserva)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReservaAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdPasajero", oReserva.IdPasajero);
                    cmd.Parameters.AddWithValue("@IdVuelo", oReserva.IdVuelo);
                    cmd.Parameters.AddWithValue("@FechaReserva", oReserva.FechaReserva);
                    cmd.Parameters.AddWithValue("@MontoAnticipo", (object?)oReserva.MontoAnticipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaVuelo", (object?)oReserva.FechaVuelo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", (object?)oReserva.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oReserva.Estado ?? DBNull.Value);
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

        // Método que actualiza una reserva
        public bool MtdEditarReserva(ReservasModel oReserva)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReservaModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdReserva", oReserva.IdReserva);
                    cmd.Parameters.AddWithValue("@IdPasajero", oReserva.IdPasajero);
                    cmd.Parameters.AddWithValue("@IdVuelo", oReserva.IdVuelo);
                    cmd.Parameters.AddWithValue("@FechaReserva", oReserva.FechaReserva);
                    cmd.Parameters.AddWithValue("@MontoAnticipo", (object?)oReserva.MontoAnticipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaVuelo", (object?)oReserva.FechaVuelo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Observaciones", (object?)oReserva.Observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oReserva.Estado ?? DBNull.Value);
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

        // Método que busca una reserva por su Id
        public ReservasModel MtdBuscarReserva(int IdReserva)
        {
            var oReserva = new ReservasModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReservaBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdReserva", IdReserva);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oReserva.IdReserva = Convert.ToInt32(dr["IdReserva"]);
                            oReserva.IdPasajero = Convert.ToInt32(dr["IdPasajero"]);
                            oReserva.IdVuelo = Convert.ToInt32(dr["IdVuelo"]);
                            oReserva.FechaReserva = Convert.ToDateTime(dr["FechaReserva"]);
                            oReserva.MontoAnticipo = dr["MontoAnticipo"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["MontoAnticipo"]);
                            oReserva.FechaVuelo = dr["FechaVuelo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaVuelo"]);
                            oReserva.Observaciones = dr["Observaciones"] == DBNull.Value ? null : dr["Observaciones"].ToString();
                            oReserva.Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oReserva;
        }

        // Método que elimina una reserva
        public bool MtdEliminarReserva(int IdReserva)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ReservaEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdReserva", IdReserva);
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

        // Método para listar pasajeros activos (para el dropdown)
        public List<PasajerosModel> MtdListarPasajerosActivos()
        {
            var lista = new List<PasajerosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("usp_ListarPasajerosActivos", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PasajerosModel
                            {
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                Nombres = dr["Nombres"].ToString() ?? string.Empty,
                                Apellidos = dr["Apellidos"].ToString() ?? string.Empty
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

        // Método para listar vuelos activos (para el dropdown)
        public List<VuelosModel> MtdListarVuelosActivos()
        {
            var lista = new List<VuelosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("usp_ListarVuelosActivos", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new VuelosModel
                            {
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                AeropuertoOrigen = dr["AeropuertoOrigen"].ToString() ?? string.Empty,
                                AeropuertoDestino = dr["AeropuertoDestino"].ToString() ?? string.Empty
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
    }
}

