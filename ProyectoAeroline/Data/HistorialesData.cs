using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class HistorialesData
    {
        // Método que consulta todos los historiales
        public List<HistorialesModel> MtdConsultarHistoriales()
        {
            var listaHistoriales = new List<HistorialesModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_HistorialesSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaHistoriales.Add(new HistorialesModel
                            {
                                IdHistorial = Convert.ToInt32(dr["IdHistorial"]),
                                IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                Observacion = dr["Observacion"] == DBNull.Value ? null : dr["Observacion"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaHistoriales;
        }

        // Método que agrega un historial
        public bool MtdAgregarHistorial(HistorialesModel oHistorial)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_HistorialAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oHistorial.IdBoleto);
                    cmd.Parameters.AddWithValue("@IdPasajero", oHistorial.IdPasajero);
                    cmd.Parameters.AddWithValue("@IdAerolinea", oHistorial.IdAerolinea);
                    cmd.Parameters.AddWithValue("@IdVuelo", oHistorial.IdVuelo);
                    cmd.Parameters.AddWithValue("@Observacion", (object?)oHistorial.Observacion ?? DBNull.Value);
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

        // Método que actualiza un historial
        public bool MtdEditarHistorial(HistorialesModel oHistorial)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_HistorialModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdHistorial", oHistorial.IdHistorial);
                    cmd.Parameters.AddWithValue("@IdBoleto", oHistorial.IdBoleto);
                    cmd.Parameters.AddWithValue("@IdPasajero", oHistorial.IdPasajero);
                    cmd.Parameters.AddWithValue("@IdAerolinea", oHistorial.IdAerolinea);
                    cmd.Parameters.AddWithValue("@IdVuelo", oHistorial.IdVuelo);
                    cmd.Parameters.AddWithValue("@Observacion", (object?)oHistorial.Observacion ?? DBNull.Value);
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

        // Método que busca un historial por su Id
        public HistorialesModel MtdBuscarHistorial(int IdHistorial)
        {
            var oHistorial = new HistorialesModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_HistorialBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdHistorial", IdHistorial);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oHistorial.IdHistorial = Convert.ToInt32(dr["IdHistorial"]);
                            oHistorial.IdBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            oHistorial.IdPasajero = Convert.ToInt32(dr["IdPasajero"]);
                            oHistorial.IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]);
                            oHistorial.IdVuelo = Convert.ToInt32(dr["IdVuelo"]);
                            oHistorial.Observacion = dr["Observacion"] == DBNull.Value ? null : dr["Observacion"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oHistorial;
        }

        // Método que elimina un historial
        public bool MtdEliminarHistorial(int IdHistorial)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_HistorialEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdHistorial", IdHistorial);
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

        // Método para listar aerolíneas activas (para el dropdown)
        public List<AerolineasModel> MtdListarAerolineasActivas()
        {
            var lista = new List<AerolineasModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("usp_AerolineasListar", conexion)
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
                                Nombre = dr["Nombre"].ToString() ?? string.Empty
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

