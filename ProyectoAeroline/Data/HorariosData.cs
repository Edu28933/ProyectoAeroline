using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class HorariosData
    {
        // Listar todos los horarios
        public List<HorariosModel> MtdConsultarHorarios()
        {
            var lista = new List<HorariosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarHorarios", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new HorariosModel
                            {
                                IdHorario = Convert.ToInt32(dr["IdHorario"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                TiempoEspera = dr["TiempoEspera"] != DBNull.Value ? TimeSpan.Parse(dr["TiempoEspera"].ToString()!) : null,
                                Estado = dr["Estado"]?.ToString(),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : null,
                                HoraCreacion = dr["HoraCreacion"] != DBNull.Value ? TimeSpan.Parse(dr["HoraCreacion"].ToString()!) : null,
                                UsuarioActualizacion = dr["UsuarioActualizacion"]?.ToString(),
                                FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaActualizacion"]) : null,
                                HoraActualizacion = dr["HoraActualizacion"] != DBNull.Value ? TimeSpan.Parse(dr["HoraActualizacion"].ToString()!) : null,
                                DescripcionVuelo = dr["DescripcionVuelo"]?.ToString(),
                                NumeroVuelo = dr["NumeroVuelo"]?.ToString(),
                                AeropuertoOrigen = dr["AeropuertoOrigen"]?.ToString(),
                                AeropuertoDestino = dr["AeropuertoDestino"]?.ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al consultar horarios: {ex.Message}");
                }
            }

            return lista;
        }

        // Buscar horario por ID
        public HorariosModel? MtdBuscarHorario(int IdHorario)
        {
            HorariosModel? oHorario = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarHorario", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdHorario", IdHorario);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oHorario = new HorariosModel
                            {
                                IdHorario = Convert.ToInt32(dr["IdHorario"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                TiempoEspera = dr["TiempoEspera"] != DBNull.Value ? TimeSpan.Parse(dr["TiempoEspera"].ToString()!) : null,
                                Estado = dr["Estado"]?.ToString(),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : null,
                                DescripcionVuelo = dr["DescripcionVuelo"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar horario: {ex.Message}");
            }

            return oHorario;
        }

        // Agregar horario
        public bool MtdAgregarHorario(HorariosModel oHorario)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (oHorario == null)
                throw new ArgumentNullException(nameof(oHorario), "El modelo de horario no puede ser nulo.");

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarHorario", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdVuelo", oHorario.IdVuelo);
                    cmd.Parameters.AddWithValue("@HoraSalida", oHorario.HoraSalida);
                    cmd.Parameters.AddWithValue("@HoraLlegada", oHorario.HoraLlegada);
                    cmd.Parameters.AddWithValue("@TiempoEspera", oHorario.TiempoEspera ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oHorario.Estado ?? "Activo");

                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                // Re-lanzar excepciones SQL para que el controller las maneje
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar horario: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Editar horario
        public bool MtdEditarHorario(HorariosModel oHorario)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarHorario", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdHorario", oHorario.IdHorario);
                    cmd.Parameters.AddWithValue("@IdVuelo", oHorario.IdVuelo);
                    cmd.Parameters.AddWithValue("@HoraSalida", oHorario.HoraSalida);
                    cmd.Parameters.AddWithValue("@HoraLlegada", oHorario.HoraLlegada);
                    cmd.Parameters.AddWithValue("@TiempoEspera", oHorario.TiempoEspera ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oHorario.Estado ?? "Activo");

                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                // Re-lanzar excepciones SQL para que el controller las maneje
                throw new Exception(sqlEx.Message, sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar horario: {ex.Message}");
            }

            return respuesta;
        }

        // Eliminar horario
        public bool MtdEliminarHorario(int IdHorario)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarHorario", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdHorario", IdHorario);
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar horario: {ex.Message}");
            }

            return respuesta;
        }

        // Obtener vuelos para combo (Código + Origen + Destino)
        public List<SelectListItem> ObtenerVuelosParaCombo()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("usp_ObtenerVuelosParaCombo", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new SelectListItem
                            {
                                Value = dr["IdVuelo"].ToString(),
                                Text = dr["DescripcionVuelo"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener vuelos para combo: {ex.Message}");
                }
            }

            return lista;
        }
    }
}

