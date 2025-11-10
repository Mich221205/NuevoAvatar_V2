using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Services;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica
{
    public static class ProfesorEndpoints
    {
        public static void MapProfesorEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/profesor")
                           .WithOpenApi()
                           .WithTags("Profesor");
                          // .RequireAuthorization(); 

            //  Obtener todos los profesores
            group.MapGet("/", async (IProfesorService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var data = await service.GetAllAsync(idUsuario);
                return Results.Ok(data);
            })
            .WithSummary("Obtiene todos los profesores")
            .WithDescription("Devuelve la lista completa de profesores registrados.");

            // Obtener profesor por ID
            group.MapGet("/{id:int}", async (int id, IProfesorService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var data = await service.GetByIdAsync(id, idUsuario);

                return data is not null ? Results.Ok(data) : Results.NotFound("Profesor no encontrado.");
            })
            .WithSummary("Obtiene un profesor por ID.");

            //  Crear profesor
            group.MapPost("/", async (Profesor profesor, IProfesorService service, HttpContext ctx) =>
            {
                try
                {
                    var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                    var result = await service.CreateAsync(profesor, idUsuario);
                    return Results.Created($"/profesor/{result}", new { Mensaje = "Profesor registrado correctamente." });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
            })
            .WithSummary("Crea un nuevo profesor.")
            .WithDescription("Registra un profesor nuevo y genera una bitácora.");

            //  Actualizar profesor
            group.MapPut("/", async (Profesor profesor, IProfesorService service, HttpContext ctx) =>
            {
                try
                {
                    var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                    var result = await service.UpdateAsync(profesor, idUsuario);
                    return Results.Ok(new { Mensaje = "Profesor actualizado correctamente." });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
            })
            .WithSummary("Modifica un profesor existente.");

            //  Eliminar profesor
            group.MapDelete("/{id:int}", async (int id, IProfesorService service, HttpContext ctx) =>
            {
                try
                {
                    if (id <= 0)
                        return Results.BadRequest("El ID debe ser mayor a 0.");

                    var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                    var result = await service.DeleteAsync(id, idUsuario);
                    return Results.Ok(new { Mensaje = $"Profesor con ID {id} eliminado correctamente." });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
            })
            .WithSummary("Elimina un profesor existente.");
        }
    }
}
