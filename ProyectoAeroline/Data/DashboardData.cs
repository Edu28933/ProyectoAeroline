using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class DashboardData
    {
        // Obtener estadísticas del dashboard con rango de fechas opcional
        public DashboardStats MtdObtenerEstadisticas(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var stats = new DashboardStats();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();

                    // Construir consultas con parámetros SQL para seguridad
                    // Vuelos reservados (Boletos con estado Pendiente o Confirmado)
                    var cmdBoletos = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM Boletos b
                        WHERE b.Estado IN ('Pendiente', 'Confirmado')", conexion);
                    
                    if (fechaInicio.HasValue && fechaFin.HasValue)
                    {
                        cmdBoletos.CommandText += " AND b.FechaCompra BETWEEN @FechaInicio AND @FechaFin";
                        cmdBoletos.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                        cmdBoletos.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdBoletos.CommandText += " AND b.FechaCompra >= @FechaInicio";
                        cmdBoletos.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                    }
                    else if (fechaFin.HasValue)
                    {
                        cmdBoletos.CommandText += " AND b.FechaCompra <= @FechaFin";
                        cmdBoletos.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    
                    stats.VuelosReservados = Convert.ToInt32(cmdBoletos.ExecuteScalar() ?? 0);

                    // Vuelos completados (Boletos con estado Confirmado que ya pasaron)
                    var cmdCompletados = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM Boletos b
                        INNER JOIN Vuelos v ON b.IdVuelo = v.IdVuelo
                        WHERE b.Estado = 'Confirmado'
                          AND CAST(v.FechaHoraSalida AS DATE) < CAST(GETDATE() AS DATE)", conexion);
                    
                    if (fechaInicio.HasValue && fechaFin.HasValue)
                    {
                        cmdCompletados.CommandText += " AND b.FechaCompra BETWEEN @FechaInicio AND @FechaFin";
                        cmdCompletados.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                        cmdCompletados.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdCompletados.CommandText += " AND b.FechaCompra >= @FechaInicio";
                        cmdCompletados.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                    }
                    else if (fechaFin.HasValue)
                    {
                        cmdCompletados.CommandText += " AND b.FechaCompra <= @FechaFin";
                        cmdCompletados.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    
                    stats.VuelosCompletados = Convert.ToInt32(cmdCompletados.ExecuteScalar() ?? 0);

                    // Vuelos cancelados (Boletos con estado Cancelado)
                    var cmdCancelados = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM Boletos b
                        WHERE b.Estado = 'Cancelado'", conexion);
                    
                    if (fechaInicio.HasValue && fechaFin.HasValue)
                    {
                        cmdCancelados.CommandText += " AND b.FechaCompra BETWEEN @FechaInicio AND @FechaFin";
                        cmdCancelados.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                        cmdCancelados.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdCancelados.CommandText += " AND b.FechaCompra >= @FechaInicio";
                        cmdCancelados.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                    }
                    else if (fechaFin.HasValue)
                    {
                        cmdCancelados.CommandText += " AND b.FechaCompra <= @FechaFin";
                        cmdCancelados.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    
                    stats.VuelosCancelados = Convert.ToInt32(cmdCancelados.ExecuteScalar() ?? 0);

                    // Total de ingresos (suma de precios de boletos confirmados)
                    var cmdIngresos = new SqlCommand(@"
                        SELECT ISNULL(SUM(b.Total), 0) 
                        FROM Boletos b
                        WHERE b.Estado = 'Confirmado'", conexion);
                    
                    if (fechaInicio.HasValue && fechaFin.HasValue)
                    {
                        cmdIngresos.CommandText += " AND b.FechaCompra BETWEEN @FechaInicio AND @FechaFin";
                        cmdIngresos.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                        cmdIngresos.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    else if (fechaInicio.HasValue)
                    {
                        cmdIngresos.CommandText += " AND b.FechaCompra >= @FechaInicio";
                        cmdIngresos.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
                    }
                    else if (fechaFin.HasValue)
                    {
                        cmdIngresos.CommandText += " AND b.FechaCompra <= @FechaFin";
                        cmdIngresos.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1));
                    }
                    
                    var ingresos = cmdIngresos.ExecuteScalar();
                    stats.IngresosTotales = ingresos != DBNull.Value ? Convert.ToDecimal(ingresos) : 0;

                    // Vuelos recientes (últimos 4 vuelos programados)
                    // Usando directamente los campos de la tabla Vuelos sin JOINs
                    var cmdVuelosRecientes = new SqlCommand(@"
                        SELECT TOP 4
                            v.IdVuelo,
                            v.Aerolinea AS NombreAerolinea,
                            v.AeropuertoOrigen,
                            v.AeropuertoDestino,
                            CAST(v.FechaHoraSalida AS DATE) AS FechaVuelo,
                            CAST(v.FechaHoraSalida AS TIME) AS HoraSalida,
                            CAST(v.FechaHoraLlegada AS TIME) AS HoraLlegada,
                            ISNULL(v.Precio, 0) AS PrecioBase,
                            ISNULL((SELECT MIN(Precio) FROM Boletos WHERE IdVuelo = v.IdVuelo), ISNULL(v.Precio, 0)) AS PrecioMinimo
                        FROM Vuelos v
                        WHERE CAST(v.FechaHoraSalida AS DATE) >= CAST(GETDATE() AS DATE)
                          AND v.Estado = 'Activo'
                        ORDER BY CAST(v.FechaHoraSalida AS DATE), CAST(v.FechaHoraSalida AS TIME)", conexion);

                    stats.VuelosRecientes = new List<VueloResumen>();
                    using (var dr = cmdVuelosRecientes.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            stats.VuelosRecientes.Add(new VueloResumen
                            {
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                NombreAerolinea = dr["NombreAerolinea"].ToString() ?? "",
                                AeropuertoOrigen = dr["AeropuertoOrigen"].ToString() ?? "",
                                AeropuertoDestino = dr["AeropuertoDestino"].ToString() ?? "",
                                FechaVuelo = dr["FechaVuelo"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVuelo"]) : DateTime.MinValue,
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                PrecioMinimo = dr["PrecioMinimo"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioMinimo"]) : Convert.ToDecimal(dr["PrecioBase"] ?? 0)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadísticas: {ex.Message}");
            }

            return stats;
        }

        // Buscar vuelos por criterios (origen, destino, fecha)
        public List<VueloResumen> MtdBuscarVuelos(string? origen = null, string? destino = null, DateTime? fecha = null)
        {
            var vuelos = new List<VueloResumen>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    var query = @"
                        SELECT TOP 20
                            v.IdVuelo,
                            a.Nombre AS NombreAerolinea,
                            a1.Nombre AS AeropuertoOrigen,
                            a2.Nombre AS AeropuertoDestino,
                            v.FechaVuelo,
                            ISNULL(h.HoraSalida, CAST('08:00' AS TIME)) AS HoraSalida,
                            ISNULL(h.HoraLlegada, CAST('10:00' AS TIME)) AS HoraLlegada,
                            v.PrecioBase,
                            ISNULL((SELECT MIN(Precio) FROM Boletos WHERE IdVuelo = v.IdVuelo AND FechaEliminacion IS NULL), v.PrecioBase) AS PrecioMinimo
                        FROM Vuelos v
                        INNER JOIN Aerolineas a ON v.IdAerolinea = a.IdAerolinea
                        INNER JOIN Aeropuertos a1 ON v.IdAeropuertoOrigen = a1.IdAeropuerto
                        INNER JOIN Aeropuertos a2 ON v.IdAeropuertoDestino = a2.IdAeropuerto
                        LEFT JOIN Horarios h ON v.IdHorario = h.IdHorario
                        WHERE v.FechaEliminacion IS NULL
                          AND v.FechaVuelo >= CAST(GETDATE() AS DATE)
                          AND a.FechaEliminacion IS NULL
                          AND a1.FechaEliminacion IS NULL
                          AND a2.FechaEliminacion IS NULL";

                    var parameters = new List<SqlParameter>();

                    if (!string.IsNullOrWhiteSpace(origen))
                    {
                        query += " AND (a1.Nombre LIKE @Origen OR a1.IATA LIKE @Origen)";
                        parameters.Add(new SqlParameter("@Origen", $"%{origen}%"));
                    }

                    if (!string.IsNullOrWhiteSpace(destino))
                    {
                        query += " AND (a2.Nombre LIKE @Destino OR a2.IATA LIKE @Destino)";
                        parameters.Add(new SqlParameter("@Destino", $"%{destino}%"));
                    }

                    if (fecha.HasValue)
                    {
                        query += " AND CAST(v.FechaVuelo AS DATE) = @Fecha";
                        parameters.Add(new SqlParameter("@Fecha", fecha.Value.Date));
                    }

                    query += " ORDER BY v.FechaVuelo, ISNULL(h.HoraSalida, CAST('08:00' AS TIME))";

                    var cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            vuelos.Add(new VueloResumen
                            {
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                NombreAerolinea = dr["NombreAerolinea"].ToString() ?? "",
                                AeropuertoOrigen = dr["AeropuertoOrigen"].ToString() ?? "",
                                AeropuertoDestino = dr["AeropuertoDestino"].ToString() ?? "",
                                FechaVuelo = dr["FechaVuelo"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVuelo"]) : DateTime.MinValue,
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                PrecioMinimo = dr["PrecioMinimo"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioMinimo"]) : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar vuelos: {ex.Message}");
            }

            return vuelos;
        }

        // Buscar vuelo por número de vuelo
        public VueloResumen? MtdBuscarVueloPorNumero(string numeroVuelo)
        {
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand(@"
                        SELECT TOP 1
                            v.IdVuelo,
                            a.Nombre AS NombreAerolinea,
                            a1.Nombre AS AeropuertoOrigen,
                            a2.Nombre AS AeropuertoDestino,
                            v.FechaVuelo,
                            ISNULL(h.HoraSalida, CAST('08:00' AS TIME)) AS HoraSalida,
                            ISNULL(h.HoraLlegada, CAST('10:00' AS TIME)) AS HoraLlegada,
                            v.PrecioBase,
                            ISNULL((SELECT MIN(Precio) FROM Boletos WHERE IdVuelo = v.IdVuelo AND FechaEliminacion IS NULL), v.PrecioBase) AS PrecioMinimo
                        FROM Vuelos v
                        INNER JOIN Aerolineas a ON v.IdAerolinea = a.IdAerolinea
                        INNER JOIN Aeropuertos a1 ON v.IdAeropuertoOrigen = a1.IdAeropuerto
                        INNER JOIN Aeropuertos a2 ON v.IdAeropuertoDestino = a2.IdAeropuerto
                        LEFT JOIN Horarios h ON v.IdHorario = h.IdHorario
                        WHERE v.FechaEliminacion IS NULL
                          AND v.NumeroVuelo LIKE @NumeroVuelo
                          AND a.FechaEliminacion IS NULL
                          AND a1.FechaEliminacion IS NULL
                          AND a2.FechaEliminacion IS NULL
                        ORDER BY v.FechaVuelo DESC", conexion);
                    cmd.Parameters.AddWithValue("@NumeroVuelo", $"%{numeroVuelo}%");

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new VueloResumen
                            {
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                NombreAerolinea = dr["NombreAerolinea"].ToString() ?? "",
                                AeropuertoOrigen = dr["AeropuertoOrigen"].ToString() ?? "",
                                AeropuertoDestino = dr["AeropuertoDestino"].ToString() ?? "",
                                FechaVuelo = dr["FechaVuelo"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVuelo"]) : DateTime.MinValue,
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                PrecioMinimo = dr["PrecioMinimo"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioMinimo"]) : 0
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar vuelo por número: {ex.Message}");
            }

            return null;
        }

        // Obtener lista de aeropuertos activos para combos
        public List<SelectListItem> MtdObtenerAeropuertosActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    var cmd = new SqlCommand(@"
                        SELECT IdAeropuerto, Nombre, IATA, Ciudad
                        FROM Aeropuertos
                        WHERE FechaEliminacion IS NULL
                          AND Estado = 'Activo'
                        ORDER BY Nombre", conexion);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = dr["Nombre"].ToString() ?? "",
                                Text = $"{dr["Nombre"]} ({dr["IATA"]}) - {dr["Ciudad"]}"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener aeropuertos: {ex.Message}");
            }

            return lista;
        }
    }

    public class DashboardStats
    {
        public int VuelosReservados { get; set; }
        public int VuelosCompletados { get; set; }
        public int VuelosCancelados { get; set; }
        public decimal IngresosTotales { get; set; }
        public List<VueloResumen> VuelosRecientes { get; set; } = new();
    }

    public class VueloResumen
    {
        public int IdVuelo { get; set; }
        public string NombreAerolinea { get; set; } = "";
        public string AeropuertoOrigen { get; set; } = "";
        public string AeropuertoDestino { get; set; } = "";
        public DateTime FechaVuelo { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public decimal PrecioMinimo { get; set; }
    }
}

