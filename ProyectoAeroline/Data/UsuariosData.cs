using ProyectoAeroline.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class UsuariosData
    {
        // Método que consulta todos los usuarios
        public List<UsuariosModel> MtdConsultarUsuarios()
        {
            var listaUsuarios = new List<UsuariosModel>();
            var conn = new Conexion();

            using (var conexion = new SqlConnection(conn.GetConnectionString()))
            {
                try
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuariosSeleccionar", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            listaUsuarios.Add(new UsuariosModel
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                IdRol = Convert.ToInt32(dr["IdRol"]),
                                Nombre = dr["Nombre"].ToString(),
                                Contraseña = dr["Contraseña"].ToString(), // en DB es 'Contraseña'
                                Estado = dr["Estado"].ToString(),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return listaUsuarios;
        }

        // Método que agrega un usuario
        public bool MtdAgregarUsuario(UsuariosModel oUsuario)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioAgregar", conexion);
                    cmd.Parameters.AddWithValue("@IdRol", oUsuario.IdRol);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@Contrasena", oUsuario.Contraseña);
                    cmd.Parameters.AddWithValue("@Estado", oUsuario.Estado);
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

        // Método que actualiza un usuario
        public bool MtdEditarUsuario(UsuariosModel oUsuario)
        {
            bool respuesta = false;

            try
            {
                var conn = new Conexion();

                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioModificar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdRol", oUsuario.IdRol);
                    cmd.Parameters.AddWithValue("@Nombre", oUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@Contrasena", oUsuario.Contraseña);
                    cmd.Parameters.AddWithValue("@Estado", oUsuario.Estado);
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

        // Método que busca un usuario por su Id
        public UsuariosModel MtdBuscarUsuario(int IdUsuario)
        {
            var oUsuario = new UsuariosModel();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioBuscar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oUsuario.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                            oUsuario.IdRol = Convert.ToInt32(dr["IdRol"]);
                            oUsuario.Nombre = dr["Nombre"].ToString();
                            oUsuario.Contraseña = dr["Contraseña"].ToString(); // en DB es 'Contraseña'
                            oUsuario.Estado = dr["Estado"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return oUsuario;
        }

        // Método que elimina un usuario
        public bool MtdEliminarUsuario(int IdUsuario)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_UsuarioEliminar", conexion);
                    cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar usuario: " + ex.Message);
                respuesta = false;
            }

            return respuesta;
        }

    }
}
