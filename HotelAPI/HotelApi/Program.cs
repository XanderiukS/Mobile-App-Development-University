using Coree.Interfaz;
using Coree.Servicio;
using MongoDB.Driver;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

///---------
// conexión
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection."
    );

// PostgreSQL
builder.Services.AddSingleton(
    NpgsqlDataSource.Create(connectionString)
);

// MongoDB
string mongoConnectionString =
    builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión MongoDB."
    );

builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(mongoConnectionString)
);

// Servicios
builder.Services.AddScoped<IHabitacion, HabitacionServicio>();
builder.Services.AddScoped<IReserva, ReservaServicio>();

///


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interfaz Swagger: /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Hotel API v1"
        );

        options.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
