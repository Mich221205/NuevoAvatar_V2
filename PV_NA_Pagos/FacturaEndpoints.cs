using PV_NA_Pagos.Dtos;
using PV_NA_Pagos.Services;

namespace PV_NA_Pagos
{
    public static class FacturaEndpoints
    {
        public static void MapFacturaEndpoints(this WebApplication app)
        {
            app.MapPost("/factura", async (
                int idEstudiante,
                int idMatricula,
                int idPeriodo,
                int idUsuario,
                FacturaService service) =>
            {
                var factura = await service.CrearFacturaAsync(
                    idEstudiante,
                    idMatricula,
                    idPeriodo,
                    idUsuario);

                return Results.Created($"/factura/{factura?.ID_Factura}", factura);
            })
            .WithSummary("Crea una factura nueva para un estudiante / matrícula / período.");

            app.MapGet("/factura/{id:int}", async (int id, FacturaService service) =>
            {
                var factura = await service.ObtenerPorIdAsync(id);
                return factura is not null ? Results.Ok(factura) : Results.NotFound();
            })
            .WithSummary("Obtiene una factura por su ID.");

            app.MapGet("/factura", async (
                int? idPeriodo,
                int? idEstudiante,
                string? estado,
                FacturaService service) =>
            {
                var facturas = await service.ListarAsync(idPeriodo, idEstudiante, estado);
                return Results.Ok(facturas);
            })
            .WithSummary("Obtiene facturas filtradas por período, estudiante y estado.");

            app.MapPost("/factura/{id:int}/reversar", async (
                int id,
                int idUsuario,
                FacturaReversarDto dto,
                FacturaService service) =>
            {
                bool ok = await service.ReversarAsync(id, dto, idUsuario);

                return ok
                    ? Results.Ok(new { message = "Factura anulada correctamente." })
                    : Results.NotFound(new { message = "Factura no encontrada o no se pudo anular." });
            })
            .WithSummary("Reversa (anula) una factura existente.");
        }
    }
}

