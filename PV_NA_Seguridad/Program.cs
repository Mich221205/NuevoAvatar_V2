using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_Seguridad;
using PV_NA_Seguridad.Repository;
using PV_NA_Seguridad.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Bitácora - GEN1",
        Version = "v1",
        Description = "Servicio para registrar y consultar bitácoras del sistema (GEN1)."
    });
});


builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<BitacoraRepository>();
builder.Services.AddScoped<BitacoraService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBitacoraEndpoints();

app.Run();
