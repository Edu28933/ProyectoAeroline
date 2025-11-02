using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class BoletosData
    {
        // --- CONSULTAR TODOS LOS BOLETOS ---
        public List<BoletosModel> MtdConsultarBoletos()
        {
            var listaBoletos = new List<BoletosModel>();
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
                            listaBoletos.Add(new BoletosModel
                            {
                                IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                NumeroAsiento = dr["NumeroAsiento"].ToString(),
                                Clase = dr["Clase"] == DBNull.Value ? null : dr["Clase"].ToString(),
                                Precio = Convert.ToDecimal(dr["Precio"]),
                                Cantidad = dr["Cantidad"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["Cantidad"]),
                                Descuento = dr["Descuento"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Descuento"]),
                                Impuesto = dr["Impuesto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Impuesto"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Reembolso = dr["Reembolso"] == DBNull.Value ? null : dr["Reembolso"].ToString(),
                                FechaCompra = dr["FechaCompra"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["FechaCompra"]),
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

            return listaBoletos;
        }

        // --- AGREGAR BOLETO ---
        public bool MtdAgregarBoleto(BoletosModel oBoleto)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BoletoAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdVuelo", oBoleto.IdVuelo);
                    cmd.Parameters.AddWithValue("@IdPasajero", oBoleto.IdPasajero);
                    cmd.Parameters.AddWithValue("@NumeroAsiento", oBoleto.NumeroAsiento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Clase", oBoleto.Clase ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", oBoleto.Precio);
                    cmd.Parameters.AddWithValue("@Cantidad", (object?)oBoleto.Cantidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descuento", (object?)oBoleto.Descuento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", (object?)oBoleto.Impuesto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Total", oBoleto.Total);
                    cmd.Parameters.AddWithValue("@Reembolso", oBoleto.Reembolso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaCompra", oBoleto.FechaCompra);
                    cmd.Parameters.AddWithValue("@Estado", oBoleto.Estado ?? (object)DBNull.Value);
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

        // --- EDITAR BOLETO ---
        public bool MtdEditarBoleto(BoletosModel oBoleto)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BoletoModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oBoleto.IdBoleto);
                    cmd.Parameters.AddWithValue("@IdVuelo", oBoleto.IdVuelo);
                    cmd.Parameters.AddWithValue("@IdPasajero", oBoleto.IdPasajero);
                    cmd.Parameters.AddWithValue("@NumeroAsiento", oBoleto.NumeroAsiento ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Clase", oBoleto.Clase ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", oBoleto.Precio);
                    cmd.Parameters.AddWithValue("@Cantidad", (object?)oBoleto.Cantidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descuento", (object?)oBoleto.Descuento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", (object?)oBoleto.Impuesto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Total", oBoleto.Total);
                    cmd.Parameters.AddWithValue("@Reembolso", oBoleto.Reembolso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaCompra", oBoleto.FechaCompra);
                    cmd.Parameters.AddWithValue("@Estado", oBoleto.Estado ?? (object)DBNull.Value);
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

        // --- BUSCAR BOLETO POR ID ---
        public BoletosModel MtdBuscarBoleto(int IdBoleto)
        {
            var oBoleto = new BoletosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BoletoBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", IdBoleto);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oBoleto.IdBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            oBoleto.IdVuelo = Convert.ToInt32(dr["IdVuelo"]);
                            oBoleto.IdPasajero = Convert.ToInt32(dr["IdPasajero"]);
                            oBoleto.NumeroAsiento = dr["NumeroAsiento"].ToString();
                            oBoleto.Clase = dr["Clase"] == DBNull.Value ? null : dr["Clase"].ToString();
                            oBoleto.Precio = Convert.ToDecimal(dr["Precio"]);
                            oBoleto.Cantidad = dr["Cantidad"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["Cantidad"]);
                            oBoleto.Descuento = dr["Descuento"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Descuento"]);
                            oBoleto.Impuesto = dr["Impuesto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Impuesto"]);
                            oBoleto.Total = Convert.ToDecimal(dr["Total"]);
                            oBoleto.Reembolso = dr["Reembolso"] == DBNull.Value ? null : dr["Reembolso"].ToString();
                            oBoleto.FechaCompra = dr["FechaCompra"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["FechaCompra"]);
                            oBoleto.Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oBoleto;
        }

        // --- ELIMINAR BOLETO ---
        public bool MtdEliminarBoleto(int IdBoleto)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BoletoEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", IdBoleto);
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

        // --- LISTAR VUELOS ACTIVOS ---
        // Método que lista vuelos activos para llenar combo
        public List<SelectListItem> MtdListarVuelosActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();                SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        IdVuelo,
                        AeropuertoOrigen,
                        AeropuertoDestino
                    FROM Vuelos
                    WHERE Estado = 'Activo'
                    ORDER BY IdVuelo DESC", conexion);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = dr["IdVuelo"].ToString(),
                            Text = $"{dr["IdVuelo"]} - {dr["AeropuertoOrigen"]} - {dr["AeropuertoDestino"]}"
                        });
                    }
                }
            }

            return lista;
        }

        // --- LISTAR PASAJEROS ACTIVOS ---
        public List<SelectListItem> MtdListarPasajerosActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
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
                        var tipoPasajero = dr["TipoPasajero"] != DBNull.Value ? dr["TipoPasajero"].ToString() : "";
                        var nombreCompleto = $"{dr["Nombres"]} {dr["Apellidos"]}";
                        lista.Add(new SelectListItem
                        {
                            Value = dr["IdPasajero"].ToString(),
                            Text = $"{dr["IdPasajero"]} - {nombreCompleto} - {tipoPasajero}"
                        });
                    }
                }
            }

            return lista;
        }

        public VuelosModel? MtdBuscarVuelo(int idVuelo)
        {
            VuelosModel? vuelo = null;
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                var cmd = new SqlCommand("SELECT Precio, Clase FROM Vuelos WHERE IdVuelo = @IdVuelo", conexion);
                cmd.Parameters.AddWithValue("@IdVuelo", idVuelo);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        vuelo = new VuelosModel
                        {
                            IdVuelo = idVuelo,
                            Precio = dr["Precio"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Precio"]),
                            Clase = dr["Clase"] == DBNull.Value ? null : dr["Clase"].ToString()
                        };
                    }
                }
            }

            return vuelo;
        }

        // --- OBTENER TIPO DE PASAJERO POR ID ---
        public string? MtdObtenerTipoPasajero(int idPasajero)
        {
            string? tipoPasajero = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("SELECT TipoPasajero FROM Pasajeros WHERE IdPasajero = @IdPasajero", conexion);
                    cmd.Parameters.AddWithValue("@IdPasajero", idPasajero);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        tipoPasajero = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tipo de pasajero: {ex.Message}");
            }

            return tipoPasajero;
        }

        // --- OBTENER CAPACIDAD DEL AVIÓN POR IDVUELO ---
        public int MtdObtenerCapacidadAvionPorVuelo(int idVuelo)
        {
            int capacidad = 0;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand(@"
                        SELECT a.Capacidad 
                        FROM Aviones a
                        INNER JOIN Vuelos v ON a.IdAvion = v.IdAvion
                        WHERE v.IdVuelo = @IdVuelo", conexion);
                    cmd.Parameters.AddWithValue("@IdVuelo", idVuelo);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        capacidad = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener capacidad del avión: {ex.Message}");
            }

            return capacidad;
        }

        // --- OBTENER ASIENTOS OCUPADOS POR IDVUELO ---
        public List<string> MtdObtenerAsientosOcupados(int idVuelo, int? idBoletoExcluir = null)
        {
            var asientosOcupados = new List<string>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var sql = @"
                        SELECT NumeroAsiento
                        FROM Boletos
                        WHERE IdVuelo = @IdVuelo
                            AND NumeroAsiento IS NOT NULL
                            AND NumeroAsiento != ''
                            AND Estado != 'Anulado'
                            AND FechaEliminacion IS NULL";
                    
                    if (idBoletoExcluir.HasValue)
                    {
                        sql += " AND IdBoleto != @IdBoletoExcluir";
                    }

                    var cmd = new SqlCommand(sql, conexion);
                    cmd.Parameters.AddWithValue("@IdVuelo", idVuelo);
                    if (idBoletoExcluir.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@IdBoletoExcluir", idBoletoExcluir.Value);
                    }

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var numeroAsiento = dr["NumeroAsiento"]?.ToString();
                            if (!string.IsNullOrEmpty(numeroAsiento))
                            {
                                // Si contiene múltiples asientos separados por comas, dividirlos
                                var asientos = numeroAsiento.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                foreach (var asiento in asientos)
                                {
                                    var asientoLimpio = asiento.Trim();
                                    if (!string.IsNullOrEmpty(asientoLimpio) && !asientosOcupados.Contains(asientoLimpio))
                                    {
                                        asientosOcupados.Add(asientoLimpio);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener asientos ocupados: {ex.Message}");
            }

            return asientosOcupados;
        }

        // --- OBTENER INFORMACIÓN DEL VUELO (FECHAS) ---
        public VuelosModel? MtdObtenerInfoVuelo(int idVuelo)
        {
            VuelosModel? vuelo = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand(@"
                        SELECT 
                            FechaHoraSalida,
                            FechaHoraLlegada,
                            AeropuertoOrigen,
                            AeropuertoDestino
                        FROM Vuelos 
                        WHERE IdVuelo = @IdVuelo", conexion);
                    cmd.Parameters.AddWithValue("@IdVuelo", idVuelo);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vuelo = new VuelosModel
                            {
                                IdVuelo = idVuelo,
                                FechaHoraSalida = dr["FechaHoraSalida"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaHoraSalida"]),
                                FechaHoraLlegada = dr["FechaHoraLlegada"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaHoraLlegada"]),
                                AeropuertoOrigen = dr["AeropuertoOrigen"]?.ToString() ?? "",
                                AeropuertoDestino = dr["AeropuertoDestino"]?.ToString() ?? ""
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener información del vuelo: {ex.Message}");
            }

            return vuelo;
        }

    }
}
