using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;


namespace ProyectoAeroline.Data
{
    public class VuelosData
    {
        public List<VuelosModel> MtdConsultarVuelos()
        {
            var lista = new List<VuelosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarVuelos", conexion)
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
                                IdAvion = Convert.ToInt32(dr["IdAvion"]),
                                Aerolinea = dr["Aerolinea"].ToString(),
                                NumeroVuelo = dr["NumeroVuelo"].ToString(),
                                AeropuertoOrigen = dr["AeropuertoOrigen"].ToString(),
                                CodigoIATAOrigen = dr["CodigoIATAOrigen"].ToString(),
                                AeropuertoDestino = dr["AeropuertoDestino"].ToString(),
                                CodigoIATADestino = dr["CodigoIATADestino"].ToString(),
                                FechaHoraSalida = dr["FechaHoraSalida"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraSalida"]) : null,
                                FechaHoraLlegada = dr["FechaHoraLlegada"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraLlegada"]) : null,
                                Clase = dr["Clase"].ToString(),
                                AsientosDisponibles = dr["AsientosDisponibles"] != DBNull.Value ? Convert.ToInt32(dr["AsientosDisponibles"]) : null,
                                Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : null,
                                Moneda = dr["Moneda"].ToString(),
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

        public bool MtdAgregarVuelo(VuelosModel oVuelo)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarVuelo", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdAvion", oVuelo.IdAvion);
                    cmd.Parameters.AddWithValue("@Aerolinea", oVuelo.Aerolinea ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroVuelo", oVuelo.NumeroVuelo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AeropuertoOrigen", oVuelo.AeropuertoOrigen);
                    cmd.Parameters.AddWithValue("@CodigoIATAOrigen", oVuelo.CodigoIATAOrigen ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AeropuertoDestino", oVuelo.AeropuertoDestino);
                    cmd.Parameters.AddWithValue("@CodigoIATADestino", oVuelo.CodigoIATADestino ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaHoraSalida", oVuelo.FechaHoraSalida ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaHoraLlegada", oVuelo.FechaHoraLlegada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Clase", oVuelo.Clase ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AsientosDisponibles", oVuelo.AsientosDisponibles ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", oVuelo.Precio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Moneda", oVuelo.Moneda ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oVuelo.Estado ?? (object)DBNull.Value);

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

        public bool MtdEditarVuelo(VuelosModel oVuelo)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarVuelo", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdVuelo", oVuelo.IdVuelo);
                    cmd.Parameters.AddWithValue("@IdAvion", oVuelo.IdAvion);
                    cmd.Parameters.AddWithValue("@Aerolinea", oVuelo.Aerolinea ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroVuelo", oVuelo.NumeroVuelo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AeropuertoOrigen", oVuelo.AeropuertoOrigen);
                    cmd.Parameters.AddWithValue("@CodigoIATAOrigen", oVuelo.CodigoIATAOrigen ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AeropuertoDestino", oVuelo.AeropuertoDestino);
                    cmd.Parameters.AddWithValue("@CodigoIATADestino", oVuelo.CodigoIATADestino ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaHoraSalida", oVuelo.FechaHoraSalida ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaHoraLlegada", oVuelo.FechaHoraLlegada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Clase", oVuelo.Clase ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AsientosDisponibles", oVuelo.AsientosDisponibles ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", oVuelo.Precio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Moneda", oVuelo.Moneda ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oVuelo.Estado ?? (object)DBNull.Value);

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

        public VuelosModel MtdBuscarVuelo(int IdVuelo)
        {
            var oVuelo = new VuelosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarVuelo", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdVuelo", IdVuelo);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oVuelo.IdVuelo = Convert.ToInt32(dr["IdVuelo"]);
                            oVuelo.IdAvion = Convert.ToInt32(dr["IdAvion"]);
                            oVuelo.Aerolinea = dr["Aerolinea"].ToString();
                            oVuelo.NumeroVuelo = dr["NumeroVuelo"].ToString();
                            oVuelo.AeropuertoOrigen = dr["AeropuertoOrigen"].ToString();
                            oVuelo.CodigoIATAOrigen = dr["CodigoIATAOrigen"].ToString();
                            oVuelo.AeropuertoDestino = dr["AeropuertoDestino"].ToString();
                            oVuelo.CodigoIATADestino = dr["CodigoIATADestino"].ToString();
                            oVuelo.FechaHoraSalida = dr["FechaHoraSalida"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraSalida"]) : null;
                            oVuelo.FechaHoraLlegada = dr["FechaHoraLlegada"] != DBNull.Value ? Convert.ToDateTime(dr["FechaHoraLlegada"]) : null;
                            oVuelo.Clase = dr["Clase"].ToString();
                            oVuelo.AsientosDisponibles = dr["AsientosDisponibles"] != DBNull.Value ? Convert.ToInt32(dr["AsientosDisponibles"]) : null;
                            oVuelo.Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : null;
                            oVuelo.Moneda = dr["Moneda"].ToString();
                            oVuelo.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oVuelo;
        }

        public bool MtdEliminarVuelo(int IdVuelo)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarVuelo", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdVuelo", IdVuelo);
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

        // Obtener aviones activos
        public List<SelectListItem> ObtenerAvionesActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("usp_ObtenerAvionesActivos", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = dr["IdAvion"].ToString(),
                            Text = $"{dr["IdAvion"]} - {dr["Placa"]}"
                        });
                    }
                }
            }

            return lista;
        }

        // Obtener aeropuertos activos
        public List<SelectListItem> ObtenerAeropuertosActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("usp_ObtenerAeropuertosActivos", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = dr["Nombre"].ToString(), // o dr["IdAeropuerto"].ToString() si prefieres
                            Text = $"{dr["IdAeropuerto"]} - {dr["Nombre"]}"
                        });
                    }
                }
            }

            return lista;
        }

        // Obtener precio según ruta
        public decimal ObtenerPrecioRuta(string origen, string destino)
        {
            decimal precio = 0;
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("usp_ObtenerPrecioRuta", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Origen", origen);
                cmd.Parameters.AddWithValue("@Destino", destino);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    precio = Convert.ToDecimal(result);
            }

            return precio;
        }

    }
}
