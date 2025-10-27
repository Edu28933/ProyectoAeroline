using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class EmpleadosData
    {
        // Método que consulta todos los empleados
        public List<EmpleadosModel> MtdConsultarEmpleados()
        {
            var listaEmpleados = new List<EmpleadosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarEmpleados", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaEmpleados.Add(new EmpleadosModel
                            {
                                IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]),
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                Nombre = dr["Nombre"].ToString(),
                                Cargo = dr["Cargo"].ToString(),
                                Licencia = dr["Licencia"].ToString(),
                                Telefono = Convert.ToInt32(dr["Telefono"]),
                                Correo = dr["Correo"].ToString(),
                                Salario = Convert.ToDouble(dr["Salario"]),
                                Direccion = dr["Direccion"].ToString(),
                                FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                                ContactoEmergencia = Convert.ToInt32(dr["ContactoEmergencia"]),
                                Estado = dr["Estado"].ToString(),
                                FotoRuta = dr["FotoRuta"] == DBNull.Value ? null : dr["FotoRuta"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaEmpleados;
        }

        // Método que agrega un empleado
        public bool MtdAgregarEmpleado(EmpleadosModel oEmpleado)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", oEmpleado.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", oEmpleado.Nombre);
                    cmd.Parameters.AddWithValue("@Cargo", oEmpleado.Cargo);
                    cmd.Parameters.AddWithValue("@Licencia", oEmpleado.Licencia);
                    cmd.Parameters.AddWithValue("@Telefono", oEmpleado.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", oEmpleado.Correo);
                    cmd.Parameters.AddWithValue("@Salario", oEmpleado.Salario);
                    cmd.Parameters.AddWithValue("@Direccion", oEmpleado.Direccion);
                    cmd.Parameters.AddWithValue("@FechaIngreso", oEmpleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@ContactoEmergencia", oEmpleado.ContactoEmergencia);

                    cmd.Parameters.AddWithValue("@Estado", oEmpleado.Estado);
                    // 🆕 Nuevo parámetro para guardar la ruta de la foto
                    cmd.Parameters.AddWithValue("@FotoRuta", (object?)oEmpleado.FotoRuta ?? DBNull.Value);

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

        // Método que actualiza un empleado
        public bool MtdEditarEmpleado(EmpleadosModel oEmpleado)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", oEmpleado.IdEmpleado);
                    cmd.Parameters.AddWithValue("@IdUsuario", oEmpleado.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", oEmpleado.Nombre);
                    cmd.Parameters.AddWithValue("@Cargo", oEmpleado.Cargo);
                    cmd.Parameters.AddWithValue("@Licencia", oEmpleado.Licencia);
                    cmd.Parameters.AddWithValue("@Telefono", oEmpleado.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", oEmpleado.Correo);
                    cmd.Parameters.AddWithValue("@Salario", oEmpleado.Salario);
                    cmd.Parameters.AddWithValue("@Direccion", oEmpleado.Direccion);
                    cmd.Parameters.AddWithValue("@FechaIngreso", oEmpleado.FechaIngreso);
                    cmd.Parameters.AddWithValue("@ContactoEmergencia", oEmpleado.ContactoEmergencia);
                    cmd.Parameters.AddWithValue("@Estado", oEmpleado.Estado);
                    cmd.Parameters.AddWithValue("@FotoRuta", (object?)oEmpleado.FotoRuta ?? DBNull.Value);

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

        // Método que busca un empleado por su Id
        public EmpleadosModel MtdBuscarEmpleado(int IdEmpleado)
        {
            var oEmpleado = new EmpleadosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oEmpleado.IdEmpleado = Convert.ToInt32(dr["IdEmpleado"]);
                            oEmpleado.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                            oEmpleado.Nombre = dr["Nombre"].ToString();
                            oEmpleado.Cargo = dr["Cargo"].ToString();
                            oEmpleado.Licencia = dr["Licencia"].ToString();
                            oEmpleado.Telefono = Convert.ToInt32(dr["Telefono"]);
                            oEmpleado.Correo = dr["Correo"].ToString();
                            oEmpleado.Salario = Convert.ToDouble(dr["Salario"]);
                            oEmpleado.Direccion = dr["Direccion"].ToString();
                            oEmpleado.FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]);
                            oEmpleado.ContactoEmergencia = Convert.ToInt32(dr["ContactoEmergencia"]);
                            oEmpleado.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oEmpleado;
        }

        // Método que elimina un empleado
        public bool MtdEliminarEmpleado(int IdEmpleado)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_EliminarEmpleado", conexion);
                    cmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
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
    }
}