using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_Pagos;
using PV_NA_Pagos.Repository;
using PV_NA_Pagos.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("BitacoraClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5210");
});

builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5233");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Factura - IPN1",
        Version = "v1",
        Description = "Servicio para facturación y pagos de los estudiantes (HU IPN1 e IPN2)."
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

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<FacturaRepository>();
builder.Services.AddScoped<FacturaService>();
builder.Services.AddScoped<PagoRepository>();
builder.Services.AddScoped<PagoService>();

builder.Services.AddHttpClient();

var app = builder.Build();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Factura - IPN1 v1");
    });
}

app.MapFacturaEndpoints();
app.MapPagoEndpoints();

app.Run();
