using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class AvionesData
    {
        // Método que consulta todos los aviones
        public List<AvionesModel> MtdConsultarAviones()
        {
            var listaAviones = new List<AvionesModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AvionSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaAviones.Add(new AvionesModel
                            {
                                IdAvion = Convert.ToInt32(dr["IdAvion"]),
                                IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]),
                                Placa = dr["Placa"].ToString(),
                                Modelo = dr["Modelo"].ToString(),
                                Tipo = dr["Tipo"].ToString(),
                                Capacidad = Convert.ToInt32(dr["Capacidad"]),
                                FechaUltimoMantenimiento = Convert.ToDateTime(dr["FechaUltimoMantenimiento"]),
                                RangoKm = Convert.ToInt32(dr["RangoKm"]),
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

            return listaAviones;
        }

        // Método que agrega un avión
        public bool MtdAgregarAvion(AvionesModel oAvion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AvionAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdAerolinea", oAvion.IdAerolinea);
                    cmd.Parameters.AddWithValue("@Placa", oAvion.Placa);
                    cmd.Parameters.AddWithValue("@Modelo", oAvion.Modelo);
                    cmd.Parameters.AddWithValue("@Tipo", oAvion.Tipo);
                    cmd.Parameters.AddWithValue("@Capacidad", oAvion.Capacidad);
                    cmd.Parameters.AddWithValue("@FechaUltimoMantenimiento", oAvion.FechaUltimoMantenimiento);
                    cmd.Parameters.AddWithValue("@RangoKm", oAvion.RangoKm);
                    cmd.Parameters.AddWithValue("@Estado", oAvion.Estado);
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

        // Método que actualiza un avión
        public bool MtdEditarAvion(AvionesModel oAvion)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AvionModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdAvion", oAvion.IdAvion);
                    cmd.Parameters.AddWithValue("@IdAerolinea", oAvion.IdAerolinea);
                    cmd.Parameters.AddWithValue("@Placa", oAvion.Placa);
                    cmd.Parameters.AddWithValue("@Modelo", oAvion.Modelo);
                    cmd.Parameters.AddWithValue("@Tipo", oAvion.Tipo);
                    cmd.Parameters.AddWithValue("@Capacidad", oAvion.Capacidad);
                    cmd.Parameters.AddWithValue("@FechaUltimoMantenimiento", oAvion.FechaUltimoMantenimiento);
                    cmd.Parameters.AddWithValue("@RangoKm", oAvion.RangoKm);
                    cmd.Parameters.AddWithValue("@Estado", oAvion.Estado);
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

        // Método que busca un avión por su Id
        public AvionesModel MtdBuscarAvion(int IdAvion)
        {
            var oAvion = new AvionesModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AvionBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdAvion", IdAvion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oAvion.IdAvion = Convert.ToInt32(dr["IdAvion"]);
                            oAvion.IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]);
                            oAvion.Placa = dr["Placa"].ToString();
                            oAvion.Modelo = dr["Modelo"].ToString();
                            oAvion.Tipo = dr["Tipo"].ToString();
                            oAvion.Capacidad = Convert.ToInt32(dr["Capacidad"]);
                            oAvion.FechaUltimoMantenimiento = Convert.ToDateTime(dr["FechaUltimoMantenimiento"]);
                            oAvion.RangoKm = Convert.ToInt32(dr["RangoKm"]);
                            oAvion.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oAvion;
        }

        // Método que elimina un avión
        public bool MtdEliminarAvion(int IdAvion)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AvionEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdAvion", IdAvion);
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

        //MÉTODO QUE BUSCA Y LISTA LOS ID PARA AGREGAR
        public List<AerolineasModel> MtdObtenerAerolineas()
        {
            var lista = new List<AerolineasModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                conexion.Open();
                using (var cmd = new SqlCommand("usp_AerolineasListar", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new AerolineasModel
                            {
                                IdAerolinea = Convert.ToInt32(dr["IdAerolinea"]),
                                Nombre = dr["Nombre"].ToString()
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}
