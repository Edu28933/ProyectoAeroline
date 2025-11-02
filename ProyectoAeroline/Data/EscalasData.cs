using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class EscalasData
    {
        // Consultar todas las escalas
        public List<EscalasModel> MtdConsultarEscalas()
        {
            var lista = new List<EscalasModel>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarEscalas", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new EscalasModel
                            {
                                IdEscala = Convert.ToInt32(dr["IdEscala"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                IdAeropuerto = Convert.ToInt32(dr["IdAeropuerto"]),
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                TiempoEspera = dr["TiempoEspera"] != DBNull.Value ? TimeSpan.Parse(dr["TiempoEspera"].ToString()!) : null,
                                Estado = dr["Estado"]?.ToString(),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : null,
                                HoraCreacion = dr["HoraCreacion"] != DBNull.Value ? TimeSpan.Parse(dr["HoraCreacion"].ToString()!) : null,
                                UsuarioActualizacion = dr["UsuarioActualizacion"]?.ToString(),
                                FechaActualizacion = dr["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaActualizacion"]) : null,
                                HoraActualizacion = dr["HoraActualizacion"] != DBNull.Value ? TimeSpan.Parse(dr["HoraActualizacion"].ToString()!) : null,
                                DescripcionVuelo = dr["DescripcionVuelo"]?.ToString(),
                                DescripcionAeropuerto = dr["DescripcionAeropuerto"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar escalas: {ex.Message}");
            }

            return lista;
        }

        // Buscar escala por ID
        public EscalasModel? MtdBuscarEscala(int IdEscala)
        {
            EscalasModel? oEscala = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarEscala", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdEscala", IdEscala);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oEscala = new EscalasModel
                            {
                                IdEscala = Convert.ToInt32(dr["IdEscala"]),
                                IdVuelo = Convert.ToInt32(dr["IdVuelo"]),
                                IdAeropuerto = Convert.ToInt32(dr["IdAeropuerto"]),
                                HoraLlegada = dr["HoraLlegada"] != DBNull.Value ? TimeSpan.Parse(dr["HoraLlegada"].ToString()!) : TimeSpan.Zero,
                                HoraSalida = dr["HoraSalida"] != DBNull.Value ? TimeSpan.Parse(dr["HoraSalida"].ToString()!) : TimeSpan.Zero,
                                TiempoEspera = dr["TiempoEspera"] != DBNull.Value ? TimeSpan.Parse(dr["TiempoEspera"].ToString()!) : null,
                                Estado = dr["Estado"]?.ToString(),
                                UsuarioCreacion = dr["UsuarioCreacion"]?.ToString(),
                                FechaCreacion = dr["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(dr["FechaCreacion"]) : null,
                                HoraCreacion = dr["HoraCreacion"] != DBNull.Value ? TimeSpan.Parse(dr["HoraCreacion"].ToString()!) : null,
                                DescripcionVuelo = dr["DescripcionVuelo"]?.ToString(),
                                DescripcionAeropuerto = dr["DescripcionAeropuerto"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar escala: {ex.Message}");
            }

            return oEscala;
        }

        // Agregar escala
        public bool MtdAgregarEscala(EscalasModel oEscala)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (oEscala == null)
                throw new ArgumentNullException(nameof(oEscala), "El modelo de escala no puede ser nulo.");

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarEscala", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdVuelo", oEscala.IdVuelo);
                    cmd.Parameters.AddWithValue("@IdAeropuerto", oEscala.IdAeropuerto);
                    cmd.Parameters.AddWithValue("@HoraLlegada", oEscala.HoraLlegada);
                    cmd.Parameters.AddWithValue("@HoraSalida", oEscala.HoraSalida);
                    cmd.Parameters.AddWithValue("@TiempoEspera", oEscala.TiempoEspera ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oEscala.Estado ?? "Activo");

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
                Console.WriteLine($"Error al agregar escala: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Editar escala
        public bool MtdEditarEscala(EscalasModel oEscala)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (oEscala == null)
                throw new ArgumentNullException(nameof(oEscala), "El modelo de escala no puede ser nulo.");

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarEscala", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdEscala", oEscala.IdEscala);
                    cmd.Parameters.AddWithValue("@IdVuelo", oEscala.IdVuelo);
                    cmd.Parameters.AddWithValue("@IdAeropuerto", oEscala.IdAeropuerto);
                    cmd.Parameters.AddWithValue("@HoraLlegada", oEscala.HoraLlegada);
                    cmd.Parameters.AddWithValue("@HoraSalida", oEscala.HoraSalida);
                    cmd.Parameters.AddWithValue("@TiempoEspera", oEscala.TiempoEspera ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oEscala.Estado ?? "Activo");

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
                Console.WriteLine($"Error al editar escala: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Eliminar escala
        public bool MtdEliminarEscala(int IdEscala)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Validación previa: Verificar el estado de la escala antes de intentar eliminar
                    var escala = MtdBuscarEscala(IdEscala);
                    if (escala == null)
                    {
                        throw new Exception("La escala no existe o ya fue eliminada.");
                    }
                    
                    if (escala.Estado == "Activo")
                    {
                        throw new Exception("No se puede eliminar una escala con estado \"Activo\". Por favor, cambie el estado a \"Inactivo\" antes de eliminar.");
                    }
                    
                    // Si pasa la validación, proceder con la eliminación
                    SqlCommand cmd = new SqlCommand("sp_EliminarEscala", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdEscala", IdEscala);
                    
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    // Verificar que se eliminó al menos una fila
                    if (rowsAffected > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        throw new Exception("No se pudo eliminar la escala. Verifique que el estado no sea \"Activo\".");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Re-lanzar excepciones SQL para que el controller las maneje
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar escala: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Obtener vuelos para combo (IdVuelo + AeropuertoOrigen + AeropuertoDestino)
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

        // Obtener aeropuertos para combo (IdAeropuerto + Nombre)
        public List<SelectListItem> ObtenerAeropuertosParaCombo()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Intentar usar el stored procedure primero
                    try
                    {
                        SqlCommand cmd = new SqlCommand("usp_ObtenerAeropuertosParaCombo", conexion)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var idAeropuerto = dr["IdAeropuerto"]?.ToString() ?? "";
                                var descripcion = dr["DescripcionAeropuerto"]?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(idAeropuerto) && !string.IsNullOrEmpty(descripcion))
                                {
                                    lista.Add(new SelectListItem
                                    {
                                        Value = idAeropuerto,
                                        Text = descripcion
                                    });
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Si el SP no existe, usar consulta directa
                        string query = @"SELECT IdAeropuerto, 
                                        CAST(IdAeropuerto AS VARCHAR(10)) + ' - ' + ISNULL(Nombre, '') AS DescripcionAeropuerto
                                        FROM Aeropuertos 
                                        WHERE Estado = 'Activo' 
                                        ORDER BY Nombre";
                        
                        SqlCommand cmdDirect = new SqlCommand(query, conexion);
                        using (var dr = cmdDirect.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var idAeropuerto = dr["IdAeropuerto"]?.ToString() ?? "";
                                var descripcion = dr["DescripcionAeropuerto"]?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(idAeropuerto) && !string.IsNullOrEmpty(descripcion))
                                {
                                    lista.Add(new SelectListItem
                                    {
                                        Value = idAeropuerto,
                                        Text = descripcion
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener aeropuertos para combo: {ex.Message}");
            }

            return lista;
        }
    }
}

