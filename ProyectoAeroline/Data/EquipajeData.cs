using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class EquipajeData
    {
        // Método que consulta todos los equipajes
        public List<EquipajeModel> MtdConsultarEquipajes()
        {
            var listaEquipajes = new List<EquipajeModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EquipajeSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaEquipajes.Add(new EquipajeModel
                            {
                                IdEquipaje = Convert.ToInt32(dr["IdEquipaje"]),
                                IdBoleto = Convert.ToInt32(dr["IdBoleto"]),
                                Peso = Convert.ToDecimal(dr["Peso"]),
                                Dimensiones = dr["Dimensiones"] == DBNull.Value ? null : dr["Dimensiones"].ToString(),
                                CaracteristicasEspeciales = dr["CaracteristicasEspeciales"] == DBNull.Value ? null : dr["CaracteristicasEspeciales"].ToString(),
                                Monto = dr["Monto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Monto"]),
                                CostoExtra = dr["CostoExtra"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CostoExtra"]),
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

            return listaEquipajes;
        }

        // Método que agrega un equipaje
        public bool MtdAgregarEquipaje(EquipajeModel oEquipaje)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EquipajeAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdBoleto", oEquipaje.IdBoleto);
                    cmd.Parameters.AddWithValue("@Peso", oEquipaje.Peso);
                    cmd.Parameters.AddWithValue("@Dimensiones", (object?)oEquipaje.Dimensiones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Monto", (object?)oEquipaje.Monto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CaracteristicasEspeciales", (object?)oEquipaje.CaracteristicasEspeciales ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostoExtra", (object?)oEquipaje.CostoExtra ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oEquipaje.Estado ?? DBNull.Value);
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

        // Método que actualiza un equipaje
        public bool MtdEditarEquipaje(EquipajeModel oEquipaje)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EquipajeModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdEquipaje", oEquipaje.IdEquipaje);
                    cmd.Parameters.AddWithValue("@IdBoleto", oEquipaje.IdBoleto);
                    cmd.Parameters.AddWithValue("@Peso", oEquipaje.Peso);
                    cmd.Parameters.AddWithValue("@Dimensiones", (object?)oEquipaje.Dimensiones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Monto", (object?)oEquipaje.Monto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CaracteristicasEspeciales", (object?)oEquipaje.CaracteristicasEspeciales ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostoExtra", (object?)oEquipaje.CostoExtra ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object?)oEquipaje.Estado ?? DBNull.Value);
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

        // Método que busca un equipaje por su Id
        public EquipajeModel MtdBuscarEquipaje(int IdEquipaje)
        {
            var oEquipaje = new EquipajeModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EquipajeBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdEquipaje", IdEquipaje);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oEquipaje.IdEquipaje = Convert.ToInt32(dr["IdEquipaje"]);
                            oEquipaje.IdBoleto = Convert.ToInt32(dr["IdBoleto"]);
                            oEquipaje.Peso = Convert.ToDecimal(dr["Peso"]);
                            oEquipaje.Dimensiones = dr["Dimensiones"] == DBNull.Value ? null : dr["Dimensiones"].ToString();
                            oEquipaje.CaracteristicasEspeciales = dr["CaracteristicasEspeciales"] == DBNull.Value ? null : dr["CaracteristicasEspeciales"].ToString();
                            oEquipaje.Monto = dr["Monto"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["Monto"]);
                            oEquipaje.CostoExtra = dr["CostoExtra"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CostoExtra"]);
                            oEquipaje.Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oEquipaje;
        }

        // Método que elimina un equipaje
        public bool MtdEliminarEquipaje(int IdEquipaje)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EquipajeEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdEquipaje", IdEquipaje);
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

