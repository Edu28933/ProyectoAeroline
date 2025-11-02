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

        // Método que agrega una factura
        public bool MtdAgregarFacturacion(FacturacionModel oFacturacion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_FacturacionAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oFacturacion.IdBoleto);
                    cmd.Parameters.AddWithValue("@FechaEmision", oFacturacion.FechaEmision);
                    cmd.Parameters.AddWithValue("@HoraEmision", (object?)oFacturacion.HoraEmision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oFacturacion.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoPago", (object?)oFacturacion.TipoPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Moneda", (object?)oFacturacion.Moneda ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Monto", (object?)oFacturacion.Monto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", (object?)oFacturacion.Impuesto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MontoFactura", oFacturacion.MontoFactura);
                    cmd.Parameters.AddWithValue("@MontoTotal", (object?)oFacturacion.MontoTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroAutorizacion", (object?)oFacturacion.NumeroAutorizacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oFacturacion.Estado ?? DBNull.Value);
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
                    cmd.Parameters.AddWithValue("@HoraEmision", (object?)oFacturacion.HoraEmision ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oFacturacion.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoPago", (object?)oFacturacion.TipoPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Moneda", (object?)oFacturacion.Moneda ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Monto", (object?)oFacturacion.Monto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Impuesto", (object?)oFacturacion.Impuesto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MontoFactura", oFacturacion.MontoFactura);
                    cmd.Parameters.AddWithValue("@MontoTotal", (object?)oFacturacion.MontoTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumeroAutorizacion", (object?)oFacturacion.NumeroAutorizacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oFacturacion.Estado ?? DBNull.Value);
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

