using Coree.Interfaz;
using Dapper;
using Microsoft.Extensions.Configuration;
using Modelo.Modelos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coree.Servicio
{
    public class UsuarioServicio : IUsuario
    {
        private readonly string _connectionString;

        public UsuarioServicio(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión DefaultConnection.");
        }

        private NpgsqlConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }


        public async Task<List<MUsuario>> ObtenerTodos()
        {
            try
            {
                const string sql = @"
                SELECT
                    id_usuario AS IdUsuario,
                    nombre AS Nombre,
                    usuario AS UsuarioLogin,
                    password AS Password,
                    id_rol AS IdRol,
                    estado AS Estado,
                    fecha_creacion AS FechaCreacion
                FROM usuario
                ORDER BY id_usuario;";
                await using var connection = CrearConexion();

                var resultado = await connection.QueryAsync<MUsuario>(sql);

                return resultado.ToList();

            }
            catch (Exception ex) {
                Console.WriteLine("Error al obtener usuarios: " + ex.Message);

                return new List<MUsuario>();
            }
        }
    }
}
