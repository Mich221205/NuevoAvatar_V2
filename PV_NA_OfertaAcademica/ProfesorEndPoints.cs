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
            

            
            group.MapGet("/", async (IProfesorService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var data = await service.GetAllAsync(idUsuario);
                return Results.Ok(data);
            })
            .WithSummary("Obtiene todos los profesores")
            .WithDescription("Devuelve la lista completa de profesores registrados.");

         
            group.MapGet("/{id:int}", async (int id, IProfesorService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var data = await service.GetByIdAsync(id, idUsuario);

                return data is not null ? Results.Ok(data) : Results.NotFound("Profesor no encontrado.");
            })
            .WithSummary("Obtiene un profesor por ID.");

            
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

          
            group.MapDelete("/{id:int}", async (int id, IProfesorService service, HttpContext ctx) =>
            {
                try
                {
                    if (id <= 0)
                        return Results.BadRequest(new { Error = "El ID debe ser mayor a 0." });

                    var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                    var result = await service.DeleteAsync(id, idUsuario);

                    return Results.Ok(new { Mensaje = $"Profesor con ID {id} eliminado correctamente." });
                }
                catch (Exception ex)
                {
                    
                    var msg = ex.Message ?? string.Empty;

                    if (msg.Contains("No se encontró el profesor indicado", StringComparison.OrdinalIgnoreCase))
                    {
                        return Results.NotFound(new { Error = "No se encontró el profesor indicado." });
                    }

                    
                    if (msg.Contains("grupos en períodos activos", StringComparison.OrdinalIgnoreCase))
                    {
                        
                        return Results.Conflict(new
                        {
                            Error = "No se puede eliminar el profesor porque dicta grupos en períodos activos."
                        });
                    }

                    return Results.BadRequest(new { Error = msg });
                }
            })
            .WithSummary("Elimina un profesor existente.");
        }
    }
}
