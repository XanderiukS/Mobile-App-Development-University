using Coree.Interfaz;
using Dapper;
using Microsoft.Extensions.Configuration;
using Modelo.Modelos;
using Npgsql;

namespace Coree.Servicio
{
    public class HabitacionServicio : IHabitacion
    {
        private readonly string _connectionString;

        public HabitacionServicio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                  throw new InvalidOperationException("No se encontro la cadena de conexión");
        }

        private NpgsqlConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<List<MHabitacion>> ObtenerTodas()
        {
            const string sql = @"
                SELECT
                    numero_habitacion AS NumeroHabitacion,
                    tipo AS Tipo,
                    precio_noche AS PrecioNoche,
                    estado AS Estado
                FROM habitacion
                ORDER BY numero_habitacion;";

            await using var connection = CrearConexion();
            var resultado = await connection.QueryAsync<MHabitacion>(sql);
            return resultado.ToList();
        }
    }
}
