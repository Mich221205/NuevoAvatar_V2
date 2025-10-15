using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using PV_NA_Academico;
using PV_NA_Academico.Repository;
using PV_NA_Academico.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("BitacoraClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5210");

});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Bitácora - ACA1",
        Version = "v1",
        Description = "Servicio para consultar el historial académico de los estudiantes (HU ACA1)."
    });
});


builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<HistorialRepository>();
builder.Services.AddScoped<HistorialService>();
builder.Services.AddScoped<ListadoEstudiantesRepository>();
builder.Services.AddScoped<ListadoEstudiantesService>();

builder.Services.AddHttpClient();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHistorialEndpoints();
app.MapListadoEstudiantesEndpoints();

app.Run();
