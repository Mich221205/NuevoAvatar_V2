using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_Matricula;
using PV_NA_Matricula.Repository;
using PV_NA_Matricula.Services;
using System.Data;

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

// ======================================================
// 🔹 Repositorios y Servicios
// ======================================================
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IPreMatriculaRepository, PreMatriculaRepository>();
builder.Services.AddScoped<IPreMatriculaService, PreMatriculaService>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IDireccionRepository, DireccionRepository>();
builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<INotasRepository, NotasRepository>();
builder.Services.AddScoped<INotasService, NotasService>();

builder.Services.AddHttpClient();

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
        context.Request.Path.StartsWithSegments("/validate"))
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

// ======================================================
// 🔹 Run
// ======================================================
app.Run();
