using Coree.Interfaz;
using Dapper;
using Microsoft.Extensions.Configuration;
using Modelo.Modelos;
using MongoDB.Driver;
using Npgsql;
using System.Data;

namespace Coree.Servicio
{
    public class ReservaServicio : IReserva
    {
        private const string Disponible = "DISPONIBLE";
        private const string Ocupada = "OCUPADA";

        private const string SqlSelectReserva = @"
            SELECT
                id_reserva AS IdReserva,
                nombre_cliente AS NombreCliente,
                numero_habitacion AS NumeroHabitacion,
                fecha_entrada AS FechaEntrada,
                fecha_salida AS FechaSalida,
                cantidad_personas AS CantidadPersonas,
                monto_total AS MontoTotal,
                estado AS Estado,
                fecha_registro AS FechaRegistro,
                usuario AS Usuario
            FROM reserva";
        private class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
        {
            public override void SetValue(IDbDataParameter parameter, DateOnly value)
            {
                parameter.DbType = DbType.Date;
                parameter.Value = value.ToDateTime(TimeOnly.MinValue);
            }

            public override DateOnly Parse(object value) => value switch
            {
                DateOnly d => d,
                DateTime dt => DateOnly.FromDateTime(dt),
                _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
            };
        }

        static ReservaServicio()
        {
            SqlMapper.AddTypeHandler(new DateOnlyHandler());
        }

        private readonly string _connectionString;
        private readonly IMongoCollection<MReservaHistorialMongo> _historialMongo;

        public ReservaServicio(IConfiguration configuration, IMongoClient mongoClient)
        {
            // Postgres
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                  throw new InvalidOperationException("No se encontro la cadena de conexión");

            // Mongo
            var mongoDatabase = mongoClient.GetDatabase("HOTEL");
            _historialMongo = mongoDatabase.GetCollection<MReservaHistorialMongo>("HISTORIAL_RESERVA");
        }

        private NpgsqlConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<List<MReserva>> ObtenerTodas()
        {
            await using var connection = CrearConexion();
            var resultado = await connection.QueryAsync<MReserva>(SqlSelectReserva + " ORDER BY id_reserva;");
            return resultado.ToList();
        }

        public async Task<MReserva> Insertar(MReservaRequest request)
        {
            Validar(request);

            await using var connection = CrearConexion();
            await connection.OpenAsync();
            await using var tx = await connection.BeginTransactionAsync();

            MReserva reserva;
            try
            {
                var habitacion = await ObtenerHabitacionBloqueada(connection, tx, request.NumeroHabitacion)
                    ?? throw new NoEncontradoException($"La habitación {request.NumeroHabitacion} no existe.");

                if (habitacion.Estado != Disponible)
                    throw new HabitacionNoDisponibleException($"La habitación {habitacion.NumeroHabitacion} no está disponible.");

                decimal monto = CalcularMonto(request, habitacion.PrecioNoche);

                const string sqlInsert = @"
                    INSERT INTO reserva
                        (nombre_cliente, numero_habitacion, fecha_entrada, fecha_salida, cantidad_personas, monto_total, estado, usuario)
                    VALUES
                        (@NombreCliente, @NumeroHabitacion, @FechaEntrada, @FechaSalida, @CantidadPersonas, @MontoTotal, 'ACTIVA', @Usuario)
                    RETURNING id_reserva;";

                int id = await connection.ExecuteScalarAsync<int>(sqlInsert, new
                {
                    request.NombreCliente,
                    request.NumeroHabitacion,
                    request.FechaEntrada,
                    request.FechaSalida,
                    request.CantidadPersonas,
                    MontoTotal = monto,
                    request.Usuario
                }, tx);

                await CambiarEstadoHabitacion(connection, tx, request.NumeroHabitacion, Ocupada);

                reserva = await ObtenerPorId(connection, tx, id)
                    ?? throw new InvalidOperationException("No se pudo leer la reserva creada.");

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            await RegistrarHistorial(reserva, "REGISTRO",
                $"Reserva registrada para {reserva.NombreCliente}, habitación {reserva.NumeroHabitacion}, monto {reserva.MontoTotal}.",
                request.Usuario);

            return reserva;
        }

        public async Task<MReserva> Actualizar(int idReserva, MReservaRequest request)
        {
            Validar(request);

            await using var connection = CrearConexion();
            await connection.OpenAsync();
            await using var tx = await connection.BeginTransactionAsync();

            MReserva reserva;
            string descripcion;
            try
            {
                var actual = await ObtenerPorId(connection, tx, idReserva, bloquear: true)
                    ?? throw new NoEncontradoException($"La reserva {idReserva} no existe.");

                bool cambioHabitacion = actual.NumeroHabitacion != request.NumeroHabitacion;

                MHabitacion? nueva;
                if (cambioHabitacion)
                {
                    // Bloqueo de habitaciones en orden ascendente para evitar deadlocks
                    int primero = Math.Min(actual.NumeroHabitacion, request.NumeroHabitacion);
                    int segundo = Math.Max(actual.NumeroHabitacion, request.NumeroHabitacion);
                    var h1 = await ObtenerHabitacionBloqueada(connection, tx, primero);
                    var h2 = await ObtenerHabitacionBloqueada(connection, tx, segundo);
                    nueva = primero == request.NumeroHabitacion ? h1 : h2;

                    if (nueva == null)
                        throw new NoEncontradoException($"La habitación {request.NumeroHabitacion} no existe.");
                    if (nueva.Estado != Disponible)
                        throw new HabitacionNoDisponibleException($"La habitación {nueva.NumeroHabitacion} no está disponible.");
                }
                else
                {
                    nueva = await ObtenerHabitacionBloqueada(connection, tx, request.NumeroHabitacion)
                        ?? throw new NoEncontradoException($"La habitación {request.NumeroHabitacion} no existe.");
                }

                decimal monto = CalcularMonto(request, nueva.PrecioNoche);

                const string sqlUpdate = @"
                    UPDATE reserva SET
                        nombre_cliente = @NombreCliente,
                        numero_habitacion = @NumeroHabitacion,
                        fecha_entrada = @FechaEntrada,
                        fecha_salida = @FechaSalida,
                        cantidad_personas = @CantidadPersonas,
                        monto_total = @MontoTotal,
                        usuario = @Usuario
                    WHERE id_reserva = @IdReserva;";

                await connection.ExecuteAsync(sqlUpdate, new
                {
                    IdReserva = idReserva,
                    request.NombreCliente,
                    request.NumeroHabitacion,
                    request.FechaEntrada,
                    request.FechaSalida,
                    request.CantidadPersonas,
                    MontoTotal = monto,
                    request.Usuario
                }, tx);

                if (cambioHabitacion)
                {
                    await CambiarEstadoHabitacion(connection, tx, actual.NumeroHabitacion, Disponible);
                    await CambiarEstadoHabitacion(connection, tx, request.NumeroHabitacion, Ocupada);
                }

                reserva = await ObtenerPorId(connection, tx, idReserva)
                    ?? throw new InvalidOperationException("No se pudo leer la reserva actualizada.");

                descripcion = cambioHabitacion
                    ? $"Reserva modificada; cambio de habitación {actual.NumeroHabitacion} -> {reserva.NumeroHabitacion}, monto {actual.MontoTotal} -> {reserva.MontoTotal}."
                    : $"Reserva modificada; habitación {reserva.NumeroHabitacion}, monto {actual.MontoTotal} -> {reserva.MontoTotal}.";

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            await RegistrarHistorial(reserva, "MODIFICACION", descripcion, request.Usuario);
            return reserva;
        }

        public async Task Eliminar(int idReserva, string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                throw new ValidacionException("El usuario es obligatorio.");

            await using var connection = CrearConexion();
            await connection.OpenAsync();
            await using var tx = await connection.BeginTransactionAsync();

            MReserva reserva;
            try
            {
                reserva = await ObtenerPorId(connection, tx, idReserva, bloquear: true)
                    ?? throw new NoEncontradoException($"La reserva {idReserva} no existe.");

                await ObtenerHabitacionBloqueada(connection, tx, reserva.NumeroHabitacion);

                await connection.ExecuteAsync(
                    "DELETE FROM reserva WHERE id_reserva = @IdReserva;",
                    new { IdReserva = idReserva }, tx);

                await CambiarEstadoHabitacion(connection, tx, reserva.NumeroHabitacion, Disponible);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            await RegistrarHistorial(reserva, "ELIMINACION",
                $"Reserva eliminada; habitación {reserva.NumeroHabitacion} liberada.",
                usuario);
        }

        // ---------- helpers ----------

        private static void Validar(MReservaRequest r)
        {
            if (string.IsNullOrWhiteSpace(r.NombreCliente))
                throw new ValidacionException("El nombre del cliente es obligatorio.");
            if (string.IsNullOrWhiteSpace(r.Usuario))
                throw new ValidacionException("El usuario es obligatorio.");
            if (r.FechaSalida <= r.FechaEntrada)
                throw new ValidacionException("La fecha de salida debe ser posterior a la fecha de entrada.");
            if (r.CantidadPersonas <= 0)
                throw new ValidacionException("La cantidad de personas debe ser mayor a 0.");
        }

        private static decimal CalcularMonto(MReservaRequest r, decimal precioNoche)
        {
            int noches = r.FechaSalida.DayNumber - r.FechaEntrada.DayNumber;
            return noches * precioNoche;
        }

        private static Task<MHabitacion?> ObtenerHabitacionBloqueada(NpgsqlConnection cn, NpgsqlTransaction tx, int numero)
        {
            const string sql = @"
                SELECT
                    numero_habitacion AS NumeroHabitacion,
                    tipo AS Tipo,
                    precio_noche AS PrecioNoche,
                    estado AS Estado
                FROM habitacion
                WHERE numero_habitacion = @Numero
                FOR UPDATE;";
            return cn.QueryFirstOrDefaultAsync<MHabitacion>(sql, new { Numero = numero }, tx);
        }

        private static Task<int> CambiarEstadoHabitacion(NpgsqlConnection cn, NpgsqlTransaction tx, int numero, string estado)
        {
            return cn.ExecuteAsync(
                "UPDATE habitacion SET estado = @Estado WHERE numero_habitacion = @Numero;",
                new { Estado = estado, Numero = numero }, tx);
        }

        private static Task<MReserva?> ObtenerPorId(NpgsqlConnection cn, NpgsqlTransaction tx, int id, bool bloquear = false)
        {
            string sql = SqlSelectReserva + " WHERE id_reserva = @Id" + (bloquear ? " FOR UPDATE;" : ";");
            return cn.QueryFirstOrDefaultAsync<MReserva>(sql, new { Id = id }, tx);
        }

        // El historial se guarda después del commit; un fallo en Mongo no revierte la operación.
        private async Task RegistrarHistorial(MReserva reserva, string tipo, string descripcion, string usuario)
        {
            try
            {
                await _historialMongo.InsertOneAsync(new MReservaHistorialMongo
                {
                    IdReserva = reserva.IdReserva,
                    NumeroHabitacion = reserva.NumeroHabitacion,
                    Estado = tipo == "ELIMINACION" ? "ELIMINADA" : reserva.Estado,
                    TipoOperacion = tipo,
                    Descripcion = descripcion,
                    Usuario = usuario,
                    FechaOperacion = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar historial en MongoDB: " + ex.Message);
            }
        }
    }
}
