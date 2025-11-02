using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoAeroline.Models;
using System.Data;

namespace ProyectoAeroline.Data
{
    public class PasajerosData
    {
        // Consultar todos los pasajeros
        public List<PasajerosModel> MtdConsultarPasajeros()
        {
            var lista = new List<PasajerosModel>();
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_SeleccionarPasajeros", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new PasajerosModel
                            {
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                Nombres = dr["Nombres"]?.ToString(),
                                Apellidos = dr["Apellidos"]?.ToString(),
                                Pasaporte = dr["Pasaporte"]?.ToString(),
                                Correo = dr["Correo"]?.ToString(),
                                Direccion = dr["Direccion"]?.ToString(),
                                Telefono = dr["Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Telefono"]) : null,
                                Pais = dr["Pais"]?.ToString(),
                                TipoPasajero = dr["TipoPasajero"]?.ToString(),
                                Nacionalidad = dr["Nacionalidad"]?.ToString(),
                                ContactoEmergencia = dr["ContactoEmergencia"] != DBNull.Value ? Convert.ToInt32(dr["ContactoEmergencia"]) : null,
                                Estado = dr["Estado"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar pasajeros: {ex.Message}");
            }

            return lista;
        }

        // Buscar pasajero por ID
        public PasajerosModel? MtdBuscarPasajero(int IdPasajero)
        {
            PasajerosModel? oPasajero = null;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_BuscarPasajero", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdPasajero", IdPasajero);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oPasajero = new PasajerosModel
                            {
                                IdPasajero = Convert.ToInt32(dr["IdPasajero"]),
                                Nombres = dr["Nombres"]?.ToString(),
                                Apellidos = dr["Apellidos"]?.ToString(),
                                Pasaporte = dr["Pasaporte"]?.ToString(),
                                Correo = dr["Correo"]?.ToString(),
                                Direccion = dr["Direccion"]?.ToString(),
                                Telefono = dr["Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Telefono"]) : null,
                                Pais = dr["Pais"]?.ToString(),
                                TipoPasajero = dr["TipoPasajero"]?.ToString(),
                                Nacionalidad = dr["Nacionalidad"]?.ToString(),
                                ContactoEmergencia = dr["ContactoEmergencia"] != DBNull.Value ? Convert.ToInt32(dr["ContactoEmergencia"]) : null,
                                Estado = dr["Estado"]?.ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar pasajero: {ex.Message}");
            }

            return oPasajero;
        }

        // Agregar pasajero
        public bool MtdAgregarPasajero(PasajerosModel oPasajero)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (oPasajero == null)
                throw new ArgumentNullException(nameof(oPasajero), "El modelo de pasajero no puede ser nulo.");

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_AgregarPasajero", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Nombres", oPasajero.Nombres ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellidos", oPasajero.Apellidos ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pasaporte", oPasajero.Pasaporte?.ToUpper() ?? (object)DBNull.Value); // Convertir a mayúsculas
                    cmd.Parameters.AddWithValue("@Correo", oPasajero.Correo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Direccion", oPasajero.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", oPasajero.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pais", oPasajero.Pais ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoPasajero", oPasajero.TipoPasajero ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nacionalidad", oPasajero.Nacionalidad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactoEmergencia", oPasajero.ContactoEmergencia ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oPasajero.Estado ?? "Activo");

                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar pasajero: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Editar pasajero
        public bool MtdEditarPasajero(PasajerosModel oPasajero)
        {
            bool respuesta = false;
            var conn = new Conexion();

            if (oPasajero == null)
                throw new ArgumentNullException(nameof(oPasajero), "El modelo de pasajero no puede ser nulo.");

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    SqlCommand cmd = new SqlCommand("sp_ModificarPasajero", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@IdPasajero", oPasajero.IdPasajero);
                    cmd.Parameters.AddWithValue("@Nombres", oPasajero.Nombres ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellidos", oPasajero.Apellidos ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pasaporte", oPasajero.Pasaporte?.ToUpper() ?? (object)DBNull.Value); // Convertir a mayúsculas
                    cmd.Parameters.AddWithValue("@Correo", oPasajero.Correo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Direccion", oPasajero.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telefono", oPasajero.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Pais", oPasajero.Pais ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoPasajero", oPasajero.TipoPasajero ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nacionalidad", oPasajero.Nacionalidad ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactoEmergencia", oPasajero.ContactoEmergencia ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", oPasajero.Estado ?? "Activo");

                    cmd.ExecuteNonQuery();
                }

                respuesta = true;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar pasajero: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Eliminar pasajero
        public bool MtdEliminarPasajero(int IdPasajero)
        {
            bool respuesta = false;
            var conn = new Conexion();

            try
            {
                using (var conexion = new SqlConnection(conn.GetConnectionString()))
                {
                    conexion.Open();
                    
                    // Validación previa: Verificar el estado del pasajero antes de intentar eliminar
                    var pasajero = MtdBuscarPasajero(IdPasajero);
                    if (pasajero == null)
                    {
                        throw new Exception("El pasajero no existe o ya fue eliminado.");
                    }
                    
                    if (pasajero.Estado == "Activo")
                    {
                        throw new Exception("No se puede eliminar un pasajero con estado \"Activo\". Por favor, cambie el estado a \"Inactivo\" antes de eliminar.");
                    }
                    
                    // Si pasa la validación, proceder con la eliminación
                    SqlCommand cmd = new SqlCommand("sp_EliminarPasajero", conexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@IdPasajero", IdPasajero);
                    
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    // Verificar que se eliminó al menos una fila
                    if (rowsAffected > 0)
                    {
                        respuesta = true;
                    }
                    else
                    {
                        throw new Exception("No se pudo eliminar el pasajero. Verifique que el estado no sea \"Activo\".");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en base de datos: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar pasajero: {ex.Message}");
                throw;
            }

            return respuesta;
        }

        // Obtener lista completa de países
        public List<string> ObtenerPaises()
        {
            return new List<string>
            {
                "Afganistán", "Albania", "Alemania", "Andorra", "Angola", "Antigua y Barbuda",
                "Arabia Saudí", "Argelia", "Argentina", "Armenia", "Australia", "Austria", "Azerbaiyán",
                "Bahamas", "Bangladés", "Barbados", "Baréin", "Bélgica", "Belice", "Benín", "Bielorrusia",
                "Birmania", "Bolivia", "Bosnia y Herzegovina", "Botsuana", "Brasil", "Brunéi",
                "Bulgaria", "Burkina Faso", "Burundi", "Bután", "Cabo Verde", "Camboya", "Camerún",
                "Canadá", "Catar", "Chad", "Chile", "China", "Chipre", "Colombia", "Comoras",
                "Corea del Norte", "Corea del Sur", "Costa Rica", "Costa de Marfil", "Croacia", "Cuba",
                "Dinamarca", "Dominica", "Ecuador", "Egipto", "El Salvador", "Emiratos Árabes Unidos",
                "Eritrea", "Eslovaquia", "Eslovenia", "España", "Estados Unidos", "Estonia", "Etiopía",
                "Filipinas", "Finlandia", "Fiyi", "Francia", "Gabón", "Gambia", "Georgia", "Ghana",
                "Granada", "Grecia", "Guatemala", "Guinea", "Guinea-Bisáu", "Guinea Ecuatorial",
                "Guyana", "Haití", "Honduras", "Hungría", "India", "Indonesia", "Irak", "Irán",
                "Irlanda", "Islandia", "Islas Marshall", "Islas Salomón", "Israel", "Italia",
                "Jamaica", "Japón", "Jordania", "Kazajistán", "Kenia", "Kirguistán", "Kiribati",
                "Kuwait", "Laos", "Lesoto", "Letonia", "Líbano", "Liberia", "Libia", "Liechtenstein",
                "Lituania", "Luxemburgo", "Macedonia del Norte", "Madagascar", "Malasia", "Malaui",
                "Maldivas", "Malí", "Malta", "Marruecos", "Mauricio", "Mauritania", "México",
                "Micronesia", "Moldavia", "Mónaco", "Mongolia", "Montenegro", "Mozambique",
                "Namibia", "Nauru", "Nepal", "Nicaragua", "Níger", "Nigeria", "Noruega", "Nueva Zelanda",
                "Omán", "Países Bajos", "Pakistán", "Palaos", "Panamá", "Papúa Nueva Guinea",
                "Paraguay", "Perú", "Polonia", "Portugal", "Reino Unido", "República Centroafricana",
                "República Checa", "República del Congo", "República Democrática del Congo",
                "República Dominicana", "Ruanda", "Rumania", "Rusia", "Samoa", "San Cristóbal y Nieves",
                "San Marino", "San Vicente y las Granadinas", "Santa Lucía", "Santo Tomé y Príncipe",
                "Senegal", "Serbia", "Seychelles", "Sierra Leona", "Singapur", "Siria", "Somalia",
                "Sri Lanka", "Suazilandia", "Sudáfrica", "Sudán", "Sudán del Sur", "Suecia", "Suiza",
                "Surinam", "Tailandia", "Tanzania", "Tayikistán", "Timor Oriental", "Togo", "Tonga",
                "Trinidad y Tobago", "Túnez", "Turkmenistán", "Turquía", "Tuvalu", "Ucrania",
                "Uganda", "Uruguay", "Uzbekistán", "Vanuatu", "Vaticano", "Venezuela", "Vietnam",
                "Yemen", "Yibuti", "Zambia", "Zimbabue"
            };
        }
    }
}
