using PV_NA_Pagos.Services;

namespace PV_NA_Pagos
{
    public static class PagoEndpoints
    {
        public static void MapPagoEndpoints(this WebApplication app)
        {
          
            app.MapPost("/pago", async (int idFactura, decimal monto, int idUsuario, PagoService service) =>
            {
                var pago = await service.CrearAsync(idFactura, monto, idUsuario);
                return Results.Created($"/pago/{pago?.ID_Pago}", pago);
            })
            .WithSummary("Crea un nuevo pago asociado a una factura.");

            app.MapGet("/pago/{id:int}", async (int id, PagoService service) =>
            {
                var pago = await service.ObtenerPorIdAsync(id);
                return pago is not null ? Results.Ok(pago) : Results.NotFound();
            })
            .WithSummary("Obtiene un pago por su ID.");

     
            app.MapGet("/pago", async (PagoService service) =>
            {
                var pagos = await service.ListarAsync();
                return Results.Ok(pagos);
            })
            .WithSummary("Obtiene todos los pagos registrados.");


            app.MapPost("/pago/{id:int}/reversar", async (int id, int idUsuario, PagoService service) =>
            {
                bool ok = await service.ReversarAsync(id, idUsuario);
                return ok
                    ? Results.Ok(new { message = "Pago anulado correctamente." })
                    : Results.NotFound(new { message = "Pago no encontrado o no se pudo anular." });
            })
            .WithSummary("Reversa (anula) un pago existente.");
        }
    }
}
