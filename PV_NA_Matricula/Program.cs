using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_Matricula;
using PV_NA_Matricula.Repository;
using PV_NA_Matricula.Services;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 🔹 HTTP Clients
// ======================================================
builder.Services.AddHttpClient("BitacoraClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5062");
});

builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5189");
});

builder.Services.AddHttpClient("OfertaClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5048");
});

builder.Services.AddHttpClient("PagosClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5034");
});

// ======================================================
// 🔹 Repositorios y Servicios
// ======================================================
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Interfaces
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IPreMatriculaRepository, PreMatriculaRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IDireccionRepository, DireccionRepository>();
builder.Services.AddScoped<INotasRepository, NotasRepository>();

// Clases concretas (NECESARIAS PARA MOBILE)
builder.Services.AddScoped<MatriculaRepository>();
builder.Services.AddScoped<PreMatriculaRepository>();
builder.Services.AddScoped<EstudianteRepository>();
builder.Services.AddScoped<DireccionRepository>();
builder.Services.AddScoped<NotasRepository>();

// Servicios
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IPreMatriculaService, PreMatriculaService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<INotasService, NotasService>();

builder.Services.AddScoped<PagosService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// ======================================================
// 🔹 Swagger con soporte JWT
// ======================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Matrícula - MAT1 a MAT5",
        Version = "v1",
        Description = "Servicio para la gestión de matrícula, prematrícula y notas de los estudiantes."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el formato: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// ======================================================
// 🔹 Construcción de la Aplicación
// ======================================================
var app = builder.Build();

// ======================================================
// 🔹 Middleware de Validación de Token
// ======================================================
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger") ||
        context.Request.Path.StartsWithSegments("/validate") ||
        context.Request.Path.StartsWithSegments("/mobile"))
    {
        await next();
        return;
    }

    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

    if (string.IsNullOrWhiteSpace(token))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Token requerido.");
        return;
    }

    var authClient = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient");
    var response = await authClient.GetAsync($"/login/validate?token={token}");

    if (!response.IsSuccessStatusCode)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Token inválido o expirado.");
        return;
    }

    await next();
});

// ======================================================
// 🔹 Swagger
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Matrícula - MAT1 a MAT5 v1");
    });
}

// ======================================================
// 🔹 Endpoints
// ======================================================
app.MapPreMatriculaEndpoints();
app.MapMatriculaEndpoints();
app.MapEstudianteEndpoints();
app.MapDireccionEndpoints();
app.MapNotasEndpoints();
app.MapMobileEndPoints();
app.MapMobileExpedienteEndpoints();

// ======================================================
// 🔹 Run
// ======================================================
app.Run();

