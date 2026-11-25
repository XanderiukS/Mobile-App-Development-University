using Coree.Interfaz;
using Coree.Servicio;
using MongoDB.Driver;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS para el frontend Angular (FBanco / ng serve)
const string FrontendCors = "Frontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

///---------
// conexi�n
string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontr� la cadena de conexi�n DefaultConnection."
    );

// PostgreSQL
builder.Services.AddSingleton(
    NpgsqlDataSource.Create(connectionString)
);

// MongoDB
string mongoConnectionString =
    builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException(
        "No se encontr� la cadena de conexi�n MongoDB."
    );

builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(mongoConnectionString)
);

// Servicios
builder.Services.AddScoped<IUsuario, UsuarioServicio>();
builder.Services.AddScoped<ICliente, ClienteServicio>();
builder.Services.AddScoped<IMovimiento, MovimientoServicio>();

///


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interfaz Swagger:
    // /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Backend Banco API v1"
        );

        options.RoutePrefix = "swagger";
    });
}

// En desarrollo no redirigimos a HTTPS: el frontend llama por HTTP y la
// redireccion 307 rompe las peticiones fetch/CORS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(FrontendCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
