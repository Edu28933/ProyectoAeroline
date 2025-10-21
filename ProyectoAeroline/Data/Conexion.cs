using Microsoft.Data.SqlClient;

namespace ProyectoAeroline.Data
{
    public class Conexion
    {
        //Declara un campo privado de solo lectura que no se puede cambiar despues de la asignacion
        private readonly string _connectionString;

        public Conexion()
        {
            // Accediendo al archivo de configuración JSON
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Obteniendo la cadena de conexión
            _connectionString = builder.GetValue<string>("ConnectionStrings:CadenaSQL")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        // Retorna la cadena de conexión
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}