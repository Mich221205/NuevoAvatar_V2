using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class MobileDireccionEndpoints
    {
        public static void MapMobileDireccionEndpoints(this WebApplication app)
        {
            // ===========================================
            // PROVINCIAS
            // ===========================================
            app.MapGet("/mobile/direccion/provincias", async (
                int idUsuario,
                IDireccionService service) =>
            {
                var data = await service.GetProvinciasAsync(idUsuario);
                return Results.Ok(data);
            })
            .WithSummary("Obtiene todas las provincias para la app móvil.");

            // ===========================================
            // CANTONES - TODOS
            // ===========================================
            app.MapGet("/mobile/direccion/cantones/todos", async (
                int idUsuario,
                IDireccionService service) =>
            {
                var data = await service.GetCantonesTodosAsync(idUsuario);
                return Results.Ok(data);
            })
            .WithSummary("Obtiene todos los cantones para la app móvil.");

            // ===========================================
            // DISTRITOS - TODOS
            // ===========================================
            app.MapGet("/mobile/direccion/distritos/todos", async (
                int idUsuario,
                IDireccionService service) =>
            {
                var data = await service.GetDistritosTodosAsync(idUsuario);
                return Results.Ok(data);
            })
            .WithSummary("Obtiene todos los distritos para la app móvil.");
        }
    }
}
