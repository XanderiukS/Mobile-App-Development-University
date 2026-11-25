using Core.Interfaz;
using Core.Servicio;
using MongoDB.Driver;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------
// Servicios
// -------------------------------------------------------------------
builder.Services.AddControllers();

// OpenAPI / Swagger
builder.Services.AddOpenApi();

// ---- Conexión PostgreSQL ----
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));

// ---- Conexión MongoDB ----
string mongoConnectionString =
    builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión MongoDB.");

// MongoClient como singleton (maneja su propio pool de conexiones).
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));

// ---- Servicios de negocio ----
// Cuando alguien pida IOrden, .NET entrega un OrdenServicio (uno por request).
builder.Services.AddScoped<IOrden, OrdenServicio>();

// -------------------------------------------------------------------
// Pipeline HTTP
// -------------------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI en /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Ordenes API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
