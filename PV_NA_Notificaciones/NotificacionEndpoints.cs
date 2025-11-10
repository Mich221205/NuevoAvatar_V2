using PV_NA_Notificaciones.Services;

namespace PV_NA_Notificaciones
{
    public static class NotificacionEndpoints
    {
        public static void MapNotificacionEndpoints(this WebApplication app)
        {
           
            app.MapPost("/notificar", async (string email, string asunto, string cuerpo, int idUsuario, NotificacionService service) =>
            {
                var result = await service.EnviarAsync(email, asunto, cuerpo, idUsuario);
                return Results.Created($"/notificar/{result.ID_Notificacion}", result);
            })
            .WithSummary("Envía una notificación por correo (simulada).");

          
            app.MapGet("/notificar", async (NotificacionService service) =>
            {
                var lista = await service.ListarAsync();
                return Results.Ok(lista);
            })
            .WithSummary("Lista todas las notificaciones enviadas.");

          
            app.MapGet("/notificar/{id:int}", async (int id, NotificacionService service) =>
            {
                var noti = await service.ObtenerPorIdAsync(id);
                return noti is not null ? Results.Ok(noti) : Results.NotFound();
            })
            .WithSummary("Obtiene una notificación por ID.");
        }
    }
}
