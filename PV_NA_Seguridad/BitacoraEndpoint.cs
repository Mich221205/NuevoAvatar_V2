using PV_NA_Seguridad.Entities;
using PV_NA_Seguridad.Services;

namespace PV_NA_Seguridad
{
    public static class BitacoraEndpoints
    {
        public static void MapBitacoraEndpoints(this WebApplication app)
        {

            app.MapPost("/bitacora", async (Bitacora bitacora, BitacoraService service) =>
            {
                try
                {
                    await service.RegistrarAsync(bitacora);
                    return Results.Created($"/bitacora/{bitacora.ID_Usuario}", new
                    {
                        message = "Bitácora registrada exitosamente.",
                        usuario = bitacora.ID_Usuario,
                        accion = bitacora.Accion
                    });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            })
            .WithName("RegistrarBitacora")
            .WithSummary("Registra una nueva acción en la bitácora.")
            .WithDescription("Inserta un nuevo registro en la tabla Bitacora con la fecha actual.");

            // ✅ GET /bitacora → listar todas las bitácoras
            app.MapGet("/bitacora", async (BitacoraService service) =>
            {
                var result = await service.ListarAsync();
                return Results.Ok(result);
            })
            .WithName("ListarBitacoras")
            .WithSummary("Obtiene todas las bitácoras registradas.");

            // ✅ GET /bitacora/usuario/{id} → listar bitácoras de un usuario
            app.MapGet("/bitacora/usuario/{idUsuario:int}", async (int idUsuario, BitacoraService service) =>
            {
                var result = await service.ListarPorUsuarioAsync(idUsuario);
                if (!result.Any())
                    return Results.NotFound(new { message = "No se encontraron bitácoras para este usuario." });

                return Results.Ok(result);
            })
            .WithName("ListarBitacorasPorUsuario")
            .WithSummary("Obtiene las bitácoras de un usuario específico.");
        }
    }
}
