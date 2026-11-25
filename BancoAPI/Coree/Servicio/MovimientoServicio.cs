using Coree.Interfaz;
using Dapper;
using Microsoft.Extensions.Configuration;
using Modelo.Modelos;
using MongoDB.Driver;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coree.Servicio
{
    public class MovimientoServicio: IMovimiento
    {
        private readonly string _connectionString;
        private readonly IMongoCollection<MMovimientoMongo> _movimientosMongo;

        public MovimientoServicio(IConfiguration configuration) {

            //Pos
            _connectionString =
                  configuration.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException(
                      "No se encontró la cadena de conexión DefaultConnection.");

            //Mongo
            string mongoConnection =configuration.GetConnectionString("MongoDB")
                ?? throw new InvalidOperationException("No se encontró MongoDB.");

            var mongoClient = new MongoClient(mongoConnection); // Conexión con mongo

            var mongoDatabase = mongoClient.GetDatabase("BANCO"); // La BD que vamos a utiliza
            _movimientosMongo =  mongoDatabase.GetCollection<MMovimientoMongo>("MOVIMIENTO_PROCESO"); 
            // Coleccion que va utilizar
        }

        private NpgsqlConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<int> Insertar(MMovimiento movimiento) {

            using var conexion = CrearConexion();
            int idMovimiento = movimiento.IdMovimiento;
            string sql = "INSERT INTO movimientos (id_cuenta_origen, id_cuenta_destino, id_tipo_movimiento, monto, saldo_anterior, saldo_nuevo, descripcion, id_usuario) VALUES (@IdCuentaOrigen, @IdCuentaDestino, @IdTipoMovimiento, @Monto, @SaldoAnterior, @SaldoNuevo, @Descripcion, @IdUsuario) RETURNING id_movimiento;";
            // INSERT EN POSTGRESQL
             //idMovimiento = await conexion.ExecuteScalarAsync<int>(sql, movimiento);
            // INSERT PARA MONGO
            var movimientoMongo = new MMovimientoMongo
            {
                IdMovimiento = movimiento.IdMovimiento,
                EstadoProceso = "PENDIENTE"
            };
            await _movimientosMongo.InsertOneAsync(movimientoMongo);

            return idMovimiento;
        }
    }
}
