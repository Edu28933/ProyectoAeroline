using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class FacturacionData
    {
        // Método que consulta todas las facturas
        public List<FacturacionModel> MtdConsultarFacturacion()
        {
            var listaFacturacion = new List<FacturacionModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaFacturacion.Add(new FacturacionModel
                            {
                                IdFactura = Convert.ToInt32(dr["IdFactura"]),
                                IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                FechaEmision = Convert.ToDateTime(dr["FechaEmision"]),
                                HoraEmision = dr["HoraEmision"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(dr["HoraEmision"].ToString() ?? "00:00:00"),
                                Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"].ToString(),
                                TipoPago = dr["TipoPago"] == DBNull.Value ? null : dr["TipoPago"].ToString(),
                                Moneda = dr["Moneda"] == DBNull.Value ? null : dr["Moneda"].ToString(),
                                Monto = dr["Monto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Monto"]),
                                Impuesto = dr["Impuesto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Impuesto"]),
                                MontoFactura = Convert.ToDecimal(dr["MontoFactura"]),
                                MontoTotal = dr["MontoTotal"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["MontoTotal"]),
                                NumeroAutorizacion = dr["NumeroAutorizacion"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["NumeroAutorizacion"]),
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

            return listaFacturacion;
        }

        // Método que agrega una factura (con datos automáticos del boleto)
        public bool MtdAgregarFacturacion(FacturacionModel oFacturacion)
        {
            bool respuesta = false;

            try
            {
                // Obtener datos del boleto
                var datosBoleto = MtdObtenerDatosBoleto(oFacturacion.IdBoleto);
                
                // Asignar datos automáticos
                oFacturacion.FechaEmision = datosBoleto.FechaCompra.Date;
                oFacturacion.HoraEmision = datosBoleto.HoraCompra;
                oFacturacion.Monto = datosBoleto.Precio;
                oFacturacion.Impuesto = datosBoleto.Impuesto ?? 0;
                oFacturacion.Moneda = datosBoleto.Moneda;

                // Calcular MontoFactura (Monto + Impuesto)
                oFacturacion.MontoFactura = oFacturacion.Monto.Value + oFacturacion.Impuesto.Value;

                // Calcular MontoTotal (MontoFactura) - según el usuario, son iguales
                oFacturacion.MontoTotal = oFacturacion.MontoFactura;

                // Generar número de autorización correlativo
                oFacturacion.NumeroAutorizacion = MtdGenerarNumeroAutorizacion();

                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oFacturacion.IdBoleto);
                    cmd.Parameters.AddWithValue("@FechaEmision", oFacturacion.FechaEmision);
                    cmd.Parameters.AddWithValue("@HoraEmision", oFacturacion.HoraEmision.HasValue ? (object)oFacturacion.HoraEmision.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(oFacturacion.Descripcion) ? DBNull.Value : oFacturacion.Descripcion);
                    cmd.Parameters.AddWithValue("@TipoPago", string.IsNullOrEmpty(oFacturacion.TipoPago) ? DBNull.Value : oFacturacion.TipoPago);
                    cmd.Parameters.AddWithValue("@Moneda", string.IsNullOrEmpty(oFacturacion.Moneda) ? DBNull.Value : oFacturacion.Moneda);
                    cmd.Parameters.AddWithValue("@Monto", oFacturacion.Monto.HasValue ? (object)oFacturacion.Monto.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", oFacturacion.Impuesto.HasValue ? (object)oFacturacion.Impuesto.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@MontoFactura", oFacturacion.MontoFactura);
                    cmd.Parameters.AddWithValue("@MontoTotal", oFacturacion.MontoTotal.HasValue ? (object)oFacturacion.MontoTotal.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroAutorizacion", oFacturacion.NumeroAutorizacion.HasValue ? (object)oFacturacion.NumeroAutorizacion.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", string.IsNullOrEmpty(oFacturacion.Estado) ? DBNull.Value : oFacturacion.Estado);
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

        // Método que actualiza una factura
        public bool MtdEditarFacturacion(FacturacionModel oFacturacion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdFactura", oFacturacion.IdFactura);
                    cmd.Parameters.AddWithValue("@IdBoleto", oFacturacion.IdBoleto);
                    cmd.Parameters.AddWithValue("@FechaEmision", oFacturacion.FechaEmision);
                    cmd.Parameters.AddWithValue("@HoraEmision", oFacturacion.HoraEmision.HasValue ? (object)oFacturacion.HoraEmision.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", string.IsNullOrEmpty(oFacturacion.Descripcion) ? DBNull.Value : oFacturacion.Descripcion);
                    cmd.Parameters.AddWithValue("@TipoPago", string.IsNullOrEmpty(oFacturacion.TipoPago) ? DBNull.Value : oFacturacion.TipoPago);
                    cmd.Parameters.AddWithValue("@Moneda", string.IsNullOrEmpty(oFacturacion.Moneda) ? DBNull.Value : oFacturacion.Moneda);
                    cmd.Parameters.AddWithValue("@Monto", oFacturacion.Monto.HasValue ? (object)oFacturacion.Monto.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", oFacturacion.Impuesto.HasValue ? (object)oFacturacion.Impuesto.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@MontoFactura", oFacturacion.MontoFactura);
                    cmd.Parameters.AddWithValue("@MontoTotal", oFacturacion.MontoTotal.HasValue ? (object)oFacturacion.MontoTotal.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroAutorizacion", oFacturacion.NumeroAutorizacion.HasValue ? (object)oFacturacion.NumeroAutorizacion.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", string.IsNullOrEmpty(oFacturacion.Estado) ? DBNull.Value : oFacturacion.Estado);
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

        // Método que busca una factura por su Id
        public FacturacionModel MtdBuscarFacturacion(int IdFactura)
        {
            var oFacturacion = new FacturacionModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdFactura", IdFactura);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oFacturacion.IdFactura = Convert.ToInt32(dr["IdFactura"]);
                            oFacturacion.IdBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            oFacturacion.FechaEmision = Convert.ToDateTime(dr["FechaEmision"]);
                            oFacturacion.HoraEmision = dr["HoraEmision"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(dr["HoraEmision"].ToString() ?? "00:00:00");
                            oFacturacion.Descripcion = dr["Descripcion"] == DBNull.Value ? null : dr["Descripcion"].ToString();
                            oFacturacion.TipoPago = dr["TipoPago"] == DBNull.Value ? null : dr["TipoPago"].ToString();
                            oFacturacion.Moneda = dr["Moneda"] == DBNull.Value ? null : dr["Moneda"].ToString();
                            oFacturacion.Monto = dr["Monto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Monto"]);
                            oFacturacion.Impuesto = dr["Impuesto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Impuesto"]);
                            oFacturacion.MontoFactura = Convert.ToDecimal(dr["MontoFactura"]);
                            oFacturacion.MontoTotal = dr["MontoTotal"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["MontoTotal"]);
                            oFacturacion.NumeroAutorizacion = dr["NumeroAutorizacion"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["NumeroAutorizacion"]);
                            oFacturacion.Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oFacturacion;
        }

        // Método que elimina una factura
        public bool MtdEliminarFacturacion(int IdFactura)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdFactura", IdFactura);
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

        // Método para listar boletos con nombre de pasajero (para el dropdown)
        public List<SelectListItem> MtdListarBoletosConPasajero()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT 
                            b.IdBoleto,
                            b.Estado,
                            p.Nombres + ' ' + p.Apellidos AS NombrePasajero
                        FROM Boletos b
                        INNER JOIN Pasajeros p ON b.IdPasajero = p.IdPasajero
                        WHERE b.FechaEliminacion IS NULL
                        ORDER BY b.IdBoleto DESC", conexion);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var estado = dr["Estado"]?.ToString();
                            var idBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            var nombrePasajero = dr["NombrePasajero"]?.ToString() ?? "Sin nombre";
                            
                            lista.Add(new SelectListItem
                            {
                                Value = idBoleto.ToString(),
                                Text = $"Boleto {idBoleto} - {nombrePasajero}"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al listar boletos: {ex.Message}");
                }
            }

            return lista;
        }

        // Método para obtener datos del boleto (fecha, hora, monto, impuesto)
        public (DateTime FechaCompra, TimeSpan HoraCompra, decimal Precio, decimal? Impuesto, string Moneda) MtdObtenerDatosBoleto(int idBoleto)
        {
            var conn = new Conexion();
            DateTime fechaCompra = DateTime.Now;
            TimeSpan horaCompra = DateTime.Now.TimeOfDay;
            decimal precio = 0;
            decimal? impuesto = null;
            string moneda = "GTQ";

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT 
                            b.FechaCompra,
                            CAST(b.FechaCompra AS TIME) AS HoraCompra,
                            b.Precio,
                            b.Impuesto,
                            v.Moneda
                        FROM Boletos b
                        LEFT JOIN Vuelos v ON b.IdVuelo = v.IdVuelo
                        WHERE b.IdBoleto = @IdBoleto
                          AND b.FechaEliminacion IS NULL", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", idBoleto);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            fechaCompra = dr["FechaCompra"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCompra"]) : DateTime.Now;
                            horaCompra = dr["HoraCompra"] != DBNull.Value ? TimeSpan.Parse(dr["HoraCompra"].ToString() ?? "00:00:00") : DateTime.Now.TimeOfDay;
                            precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : 0;
                            impuesto = dr["Impuesto"] != DBNull.Value ? Convert.ToDecimal(dr["Impuesto"]) : null;
                            moneda = dr["Moneda"]?.ToString() ?? "GTQ";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener datos del boleto: {ex.Message}");
                }
            }

            return (fechaCompra, horaCompra, precio, impuesto, moneda);
        }

        // Método para generar número de autorización correlativo
        public int MtdGenerarNumeroAutorizacion()
        {
            var conn = new Conexion();
            int siguienteNumero = 1000; // Número inicial

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(MAX(NumeroAutorizacion), 999) + 1 AS SiguienteNumero
                        FROM Facturacion
                        WHERE NumeroAutorizacion IS NOT NULL", conexion);

                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        siguienteNumero = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al generar número de autorización: {ex.Message}");
                }
            }

            return siguienteNumero;
        }

        // Método para validar si se puede eliminar (solo canceladas)
        public bool MtdPuedeEliminar(int idFactura)
        {
            var conn = new Conexion();
            bool puedeEliminar = false;

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT Estado
                        FROM Facturacion
                        WHERE IdFactura = @IdFactura", conexion);
                    cmd.Parameters.AddWithValue("@IdFactura", idFactura);

                    var estado = cmd.ExecuteScalar()?.ToString();
                    puedeEliminar = estado == "Cancelada";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al validar eliminación: {ex.Message}");
                }
            }

            return puedeEliminar;
        }

        // Método que elimina una factura (con validación de estado)
        public (bool Success, string ErrorMessage) MtdEliminarFacturacionValidado(int IdFactura)
        {
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();

                    // Validar estado
                    if (!MtdPuedeEliminar(IdFactura))
                    {
                        return (false, "Solo se pueden eliminar facturas con estado 'Cancelada'.");
                    }

                    SqlCommand cmd = new SqlCommand("sp_FacturacionEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdFactura", IdFactura);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    return (true, string.Empty);
                }
            }
            catch (SqlException sqlEx)
            {
                return (false, sqlEx.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}

