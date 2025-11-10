using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_OfertaAcademica;
using PV_NA_OfertaAcademica.Controllers;
using PV_NA_OfertaAcademica.Repository;
using PV_NA_OfertaAcademica.Services;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();


// Cliente para Bitácora (GEN1)
builder.Services.AddHttpClient("BitacoraClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5210"); // Puerto del microservicio Bitácora
    client.BaseAddress = new Uri("http://localhost:5062"); // Puerto del microservicio Bitácora
});
builder.Services.AddScoped<BitacoraService>();


// Cliente para Autenticación (USR5)
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5233"); // Puerto del microservicio UsuariosRoles
    client.BaseAddress = new Uri("http://localhost:5189"); // Puerto del microservicio UsuariosRoles
});

// Dependencias
builder.Services.AddScoped<GrupoRepository>();
builder.Services.AddScoped<IGrupoService, GrupoService>();


builder.Services.AddScoped<IInstitucionRepository, InstitucionRepository>();
builder.Services.AddScoped<IInstitucionService, InstitucionService>();

builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ICursoService, CursoService>();

builder.Services.AddScoped<PeriodoRepository>();
builder.Services.AddScoped<PeriodoService>();

builder.Services.AddScoped<IProfesorRepository, ProfesorRepository>();
builder.Services.AddScoped<IProfesorService, ProfesorService>();

builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<ICarreraService, CarreraService>();


// 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Oferta Académica",
        Version = "v1",
        Description = "Microservicio para administrar la oferta académica (Instituciones, Carreras, Cursos, Grupos, Periodos, Profesores)"
    });

    // Soporte para Autorización Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}",
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

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Permitir Swagger y el endpoint /validate sin token
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

    try
    {
        // Validar con microservicio Auth (opcional) 
        var authClient = context.RequestServices
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient("AuthClient");

        var response = await authClient.GetAsync($"/login/validate?token={token}");
        if (!response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token inválido o expirado.");
            return;
        }

        //  Decodificar localmente el token JWT para extraer el usuario
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var claims = jwt.Claims.ToList();
        // var identity = new ClaimsIdentity(claims, "jwt","email", "role");
        var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;

        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Error al validar token: {ex.Message}");
    }
});



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ENDPOINTS

app.MapInstitucionEndpoints();
app.MapCursoEndpoints();
app.MapGrupoEndpoints();
app.MapPeriodoEndpoints();
app.MapProfesorEndpoints();
app.MapCarreraEndpoints();

app.Run();
