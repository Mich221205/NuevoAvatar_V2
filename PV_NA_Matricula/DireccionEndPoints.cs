using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class DireccionEndpoints
    {
        public static void MapDireccionEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Obtener todas las provincias
            // ======================================================
            app.MapGet("/direccion/provincias", async (int idUsuario, IDireccionService service) =>
            {
                var provincias = await service.GetProvinciasAsync(idUsuario);
                return Results.Ok(provincias);
            })
            .WithSummary("Obtiene todas las provincias registradas y registra la acción en bitácora.");

            // ======================================================
            //  Obtener cantones por provincia
            // ======================================================
            app.MapGet("/direccion/cantones", async (int idProvincia, int idUsuario, IDireccionService service) =>
            {
                var cantones = await service.GetCantonesPorProvinciaAsync(idProvincia, idUsuario);
                return cantones.Any()
                    ? Results.Ok(cantones)
                    : Results.NotFound(new { message = "No se encontraron cantones para esa provincia." });
            })
            .WithSummary("Obtiene los cantones de una provincia específica y registra la acción en bitácora.");

            // ======================================================
            //  Obtener distritos por provincia y cantón
            // ======================================================
            app.MapGet("/direccion/distritos", async (int idProvincia, int idCanton, int idUsuario, IDireccionService service) =>
            {
                var distritos = await service.GetDistritosAsync(idProvincia, idCanton, idUsuario);
                return distritos.Any()
                    ? Results.Ok(distritos)
                    : Results.NotFound(new { message = "No se encontraron distritos para los parámetros indicados." });
            })
            .WithSummary("Obtiene los distritos según provincia y cantón y registra la acción en bitácora.");
        }
    }
}
