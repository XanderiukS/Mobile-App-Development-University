using Core.Interfaz;
using Dapper;
using Microsoft.Extensions.Configuration;
using Modelo.Modelos;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Servicio
{
    public class OrdenServicio : IOrden
    {
        // ---- PostgreSQL ----
        private readonly string _connectionString;

        // ---- MongoDB ----
        private readonly IMongoCollection<MOrdenMongo> _ordenesBitacora;

        public OrdenServicio(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión DefaultConnection.");

            string mongoConnection =
                configuration.GetConnectionString("MongoDB")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión MongoDB.");

            var mongoClient = new MongoClient(mongoConnection);
            var mongoDatabase = mongoClient.GetDatabase("SISTEMA_ORDENES");
            _ordenesBitacora =
                mongoDatabase.GetCollection<MOrdenMongo>("ORDEN_BITACORA");
        }
        private NpgsqlConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        // ------------------------------------------------------------------
        // GET: todas las órdenes
        // ------------------------------------------------------------------
        public async Task<List<MOrden>> ObtenerTodos()
        {
            try
            {
                const string sql = @"
                    SELECT
                        id_orden          AS IdOrden,
                        numero_orden      AS NumeroOrden,
                        nombre_cliente    AS NombreCliente,
                        direccion_entrega AS DireccionEntrega,
                        monto             AS Monto,
                        estado            AS Estado,
                        fecha_registro    AS FechaRegistro,
                        usuario_operacion AS UsuarioOperacion
                    FROM orden
                    ORDER BY id_orden;";

                await using var connection = CrearConexion();
                var resultado = await connection.QueryAsync<MOrden>(sql);
                return resultado.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener órdenes: " + ex.Message);
                return new List<MOrden>();
            }
        }

        // ------------------------------------------------------------------
        // GET: una orden por Id
        // ------------------------------------------------------------------
        public async Task<MOrden?> ObtenerPorId(int id)
        {
            try
            {
                const string sql = @"
                    SELECT
                        id_orden          AS IdOrden,
                        numero_orden      AS NumeroOrden,
                        nombre_cliente    AS NombreCliente,
                        direccion_entrega AS DireccionEntrega,
                        monto             AS Monto,
                        estado            AS Estado,
                        fecha_registro    AS FechaRegistro,
                        usuario_operacion AS UsuarioOperacion
                    FROM orden
                    WHERE id_orden = @IdOrden;";

                await using var connection = CrearConexion();
                return await connection.QueryFirstOrDefaultAsync<MOrden>(
                    sql, new { IdOrden = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la orden por Id: " + ex.Message);
                return null;
            }
        }

        // ------------------------------------------------------------------
        // POST: registrar una orden
        // ------------------------------------------------------------------
        public async Task<int> Insertar(MOrden orden)
        {
            orden.FechaRegistro = DateTime.Now;
            if (string.IsNullOrWhiteSpace(orden.Estado))
            {
                orden.Estado = "REGISTRADA";
            }

            const string sql = @"
                INSERT INTO orden
                    (numero_orden, nombre_cliente, direccion_entrega,
                     monto, estado, fecha_registro, usuario_operacion)
                VALUES
                    (@NumeroOrden, @NombreCliente, @DireccionEntrega,
                     @Monto, @Estado, @FechaRegistro, @UsuarioOperacion)
                RETURNING id_orden;";

            await using var connection = CrearConexion();

            int idGenerado = await connection.ExecuteScalarAsync<int>(sql, orden);

            var bitacora = new MOrdenMongo
            {
                NumeroOrden = orden.NumeroOrden,
                IdOrdenPostgres = idGenerado,
                Estado = orden.Estado,
                Fecha = orden.FechaRegistro,
                Descripcion =
                    $"Orden registrada por el usuario '{orden.UsuarioOperacion}'."
            };
            await _ordenesBitacora.InsertOneAsync(bitacora);

            return idGenerado;
        }

        // ------------------------------------------------------------------
        // PUT: modificar una orden
        // ------------------------------------------------------------------
        public async Task<bool> Actualizar(MOrden orden)
        {
            MOrden? anterior = await ObtenerPorId(orden.IdOrden);
            if (anterior == null)
            {
                return false;
            }

            const string sql = @"
                UPDATE orden
                SET
                    numero_orden      = @NumeroOrden,
                    nombre_cliente    = @NombreCliente,
                    direccion_entrega = @DireccionEntrega,
                    monto             = @Monto,
                    estado            = @Estado,
                    usuario_operacion = @UsuarioOperacion
                WHERE id_orden = @IdOrden;";

            await using var connection = CrearConexion();
            int filas = await connection.ExecuteAsync(sql, orden);
            if (filas == 0)
            {
                return false;
            }
            DateTime fechaCambio = DateTime.Now;
            var cambios = new List<string>();

            if (anterior.Estado != orden.Estado)
                cambios.Add($"estado: '{anterior.Estado}' -> '{orden.Estado}'");
            if (anterior.Monto != orden.Monto)
                cambios.Add($"monto: {anterior.Monto} -> {orden.Monto}");
            if (anterior.NombreCliente != orden.NombreCliente)
                cambios.Add($"cliente: '{anterior.NombreCliente}' -> '{orden.NombreCliente}'");
            if (anterior.DireccionEntrega != orden.DireccionEntrega)
                cambios.Add($"dirección: '{anterior.DireccionEntrega}' -> '{orden.DireccionEntrega}'");
            if (anterior.NumeroOrden != orden.NumeroOrden)
                cambios.Add($"número de orden: '{anterior.NumeroOrden}' -> '{orden.NumeroOrden}'");

            string detalle = cambios.Count > 0
                ? string.Join("; ", cambios)
                : "sin cambios en los campos";

            var bitacora = new MOrdenMongo
            {
                NumeroOrden = orden.NumeroOrden,
                IdOrdenPostgres = orden.IdOrden,
                Estado = orden.Estado,
                Fecha = fechaCambio,
                Descripcion =
                    $"Orden modificada por '{orden.UsuarioOperacion}' el " +
                    $"{fechaCambio:yyyy-MM-dd HH:mm:ss}. Cambios -> {detalle}."
            };
            await _ordenesBitacora.InsertOneAsync(bitacora);

            return true;
        }

        // ------------------------------------------------------------------
        // DELETE: eliminar una orden de PostgreSQL
        // ------------------------------------------------------------------
        public async Task<bool> Eliminar(int id)
        {
            try
            {
                const string sql = @"DELETE FROM orden WHERE id_orden = @IdOrden;";

                await using var connection = CrearConexion();
                int filas = await connection.ExecuteAsync(sql, new { IdOrden = id });
                return filas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar la orden: " + ex.Message);
                return false;
            }
        }
    }
}
