using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class MantenimientosData
    {
        // Método que consulta todos los mantenimientos
        public List<MantenimientosModel> MtdConsultarMantenimientos()
        {
            var listaMantenimientos = new List<MantenimientosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaMantenimientos.Add(new MantenimientosModel
                            {
                                IdMantenimiento = Convert.ToInt32(dr["IdMantenimiento"]),
                                IdAvion = Convert.ToInt32(dr["IdAvion"]),
                                IdEmpleado = dr["IdEmpleado"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdEmpleado"]),
                                FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                                FechaSalida = dr["FechaSalida"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaSalida"]),
                                Tipo = dr["Tipo"].ToString(),
                                Costo = Convert.ToDecimal(dr["Costo"]),
                                CostoExtra = Convert.ToDecimal(dr["CostoExtra"]),
                                Descripcion = dr["Descripcion"].ToString(),
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

            return listaMantenimientos;
        }

        // Método que agrega un mantenimiento
        public bool MtdAgregarMantenimiento(MantenimientosModel oMantenimiento)
        {
            bool respuesta = false;

            if (oMantenimiento == null)
            {
                throw new ArgumentNullException(nameof(oMantenimiento), "El modelo de mantenimiento no puede ser nulo.");
            }

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdAvion", oMantenimiento.IdAvion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", (object?)oMantenimiento.IdEmpleado ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaIngreso", oMantenimiento.FechaIngreso);
                    cmd.Parameters.AddWithValue("@FechaSalida", (object?)oMantenimiento.FechaSalida ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (object?)oMantenimiento.Tipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Costo", oMantenimiento.Costo);
                    cmd.Parameters.AddWithValue("@CostoExtra", oMantenimiento.CostoExtra);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oMantenimiento.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oMantenimiento.Estado ?? DBNull.Value);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al agregar mantenimiento: {sqlEx.Message}");
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar mantenimiento: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Método que actualiza un mantenimiento
        public bool MtdEditarMantenimiento(MantenimientosModel oMantenimiento)
        {
            bool respuesta = false;

            if (oMantenimiento == null)
            {
                throw new ArgumentNullException(nameof(oMantenimiento), "El modelo de mantenimiento no puede ser nulo.");
            }

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdMantenimiento", oMantenimiento.IdMantenimiento);
                    cmd.Parameters.AddWithValue("@IdAvion", oMantenimiento.IdAvion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", (object?)oMantenimiento.IdEmpleado ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaIngreso", oMantenimiento.FechaIngreso);
                    cmd.Parameters.AddWithValue("@FechaSalida", (object?)oMantenimiento.FechaSalida ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tipo", (object?)oMantenimiento.Tipo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Costo", oMantenimiento.Costo);
                    cmd.Parameters.AddWithValue("@CostoExtra", oMantenimiento.CostoExtra);
                    cmd.Parameters.AddWithValue("@Descripcion", (object?)oMantenimiento.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oMantenimiento.Estado ?? DBNull.Value);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al modificar mantenimiento: {sqlEx.Message}");
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar mantenimiento: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Método que busca un mantenimiento por su Id
        public MantenimientosModel MtdBuscarMantenimiento(int IdMantenimiento)
        {
            var oMantenimiento = new MantenimientosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdMantenimiento", IdMantenimiento);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oMantenimiento.IdMantenimiento = Convert.ToInt32(dr["IdMantenimiento"]);
                            oMantenimiento.IdAvion = Convert.ToInt32(dr["IdAvion"]);
                            oMantenimiento.IdEmpleado = dr["IdEmpleado"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdEmpleado"]);
                            oMantenimiento.FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]);
                            oMantenimiento.FechaSalida = dr["FechaSalida"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["FechaSalida"]);
                            oMantenimiento.Tipo = dr["Tipo"].ToString();
                            oMantenimiento.Costo = Convert.ToDecimal(dr["Costo"]);
                            oMantenimiento.CostoExtra = Convert.ToDecimal(dr["CostoExtra"]);
                            oMantenimiento.Descripcion = dr["Descripcion"].ToString();
                            oMantenimiento.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oMantenimiento;
        }

        // Método que elimina un mantenimiento
        public bool MtdEliminarMantenimiento(int IdMantenimiento)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (IdMantenimiento <= 0)
            {
                throw new ArgumentException("El IdMantenimiento debe ser mayor que cero.", nameof(IdMantenimiento));
            }

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdMantenimiento", IdMantenimiento);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al eliminar mantenimiento: {sqlEx.Message}");
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar mantenimiento: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // --- LISTAR AVIONES ACTIVOS ---
        public List<SelectListItem> MtdListarAvionesActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("usp_ListarAvionesActivos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

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

        // --- LISTAR EMPLEADOS ACTIVOS ---
        public List<SelectListItem> MtdListarEmpleadosActivos()
        {
            var lista = new List<SelectListItem>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("usp_ListarEmpleadosActivos", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new SelectListItem
                        {
                            Value = dr["IdEmpleado"].ToString(),
                            Text = $"{dr["IdEmpleado"]} - {dr["Nombre"]}"
                        });
                    }
                }
            }
            return lista;
        }

        // --- OBTENER INFORMACIÓN DE UN AVION POR ID ---
        public string? MtdObtenerInfoAvion(int IdAvion)
        {
            string? placa = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Placa FROM Aviones WHERE IdAvion = @IdAvion", conexion);
                    cmd.Parameters.AddWithValue("@IdAvion", IdAvion);
                    
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        placa = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener información del avión: {ex.Message}");
            }

            return placa;
        }

    }
}