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
    public class ClienteServicio : ICliente
    {
        private readonly string _connectionSting;

        public ClienteServicio( IConfiguration configuration) {
            _connectionSting = configuration.GetConnectionString("DefaultConnection") ??
                  throw new InvalidOperationException("No se encontro la cadena de conexión");
        }

        private NpgsqlConnection CrearConexion() { 
        
            return new NpgsqlConnection(_connectionSting);
        }

        public async Task<List<MCliente>> ObtenerTodos()
        {
            try
            {
                const string sql = @"
                    SELECT
                        id_cliente AS IdCliente,
                        dpi AS Dpi,
                        nombres AS Nombres,
                        apellidos AS Apellidos,
                        telefono AS Telefono,
                        correo AS Correo,
                        direccion AS Direccion,
                        fecha_registro AS FechaRegistro,
                        estado AS Estado
                    FROM cliente
                    ORDER BY id_cliente;";
                await using var connection = CrearConexion();

                var resultado = await connection.QueryAsync<MCliente>(sql);
                return resultado.ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine( "Error al obtener clientes: " + ex.Message);
                return new List<MCliente>();

            }
        
        }

        public async Task<MCliente?> ObtenerPorId(int id)
        {
            try
            {
                const string sql = @"
                    SELECT
                        id_cliente AS IdCliente,
                        dpi AS Dpi,
                        nombres AS Nombres,
                        apellidos AS Apellidos,
                        telefono AS Telefono,
                        correo AS Correo,
                        direccion AS Direccion,
                        fecha_registro AS FechaRegistro,
                        estado AS Estado
                    FROM cliente
                    WHERE id_cliente = @IdCliente;";

                await using var connection = CrearConexion();

                var resultado = await connection.QueryFirstOrDefaultAsync<MCliente>(sql, 
                                new { IdCliente = id } );

                return resultado;

            }
            catch (Exception ex) {
                Console.WriteLine("Error al obtener el cliente por ID" + ex.Message);
               return null; 
            }

        }


        public async Task<bool> Insertar(MCliente cliente)
        {
            try {
                const string sql = @"
                    INSERT INTO cliente
                    (     dpi,  nombres,  apellidos, telefono,  correo, direccion,   estado )
                    VALUES
                    (   @Dpi, @Nombres, @Apellidos,  @Telefono, @Correo, @Direccion,@Estado  );";

                await using var connection = CrearConexion();
                int resultado =
                   await connection.ExecuteAsync(sql, cliente);
                return resultado > 0;
            }
            catch (Exception ex) { 
                Console.WriteLine("Error al insertar cliente" + ex.Message); 
                return false;
            }
        
        }

        public async Task<bool> Actualizar(MCliente cliente)
        {
            try
            {
                const string sql = @"
                                UPDATE cliente
                                SET
                                    dpi = @Dpi,
                                    nombres = @Nombres,
                                    apellidos = @Apellidos,
                                    telefono = @Telefono,
                                    correo = @Correo,
                                    direccion = @Direccion,
                                    estado = @Estado
                                WHERE id_cliente = @IdCliente;";

                await using var connection = CrearConexion();
                int resultado =
                   await connection.ExecuteAsync(sql, cliente);
                return resultado > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar cliente" + ex.Message);
                return false;
            }

        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                const string sql = @"
                    DELETE FROM cliente
                    WHERE id_cliente = @IdCliente;";


                await using var connection = CrearConexion();
                int resultado =
                   await connection.ExecuteAsync(sql, new { IdCliente  = id });
                return resultado > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al Eliminar cliente" + ex.Message);
                return false;
            }

        }


    }


    
}
