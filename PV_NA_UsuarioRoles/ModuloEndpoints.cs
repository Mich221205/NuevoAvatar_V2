using Microsoft.AspNetCore.Mvc;
using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Services;

namespace PV_NA_UsuariosRoles.Endpoints
{
    public static class ModuloEndpoints
    {
        public static void MapModuloEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/modulo")
                              .WithTags("Módulos")
                              .RequireAuthorization();

            // GET - Todos
            group.MapGet("/", async ([FromServices] IModuloService service,
                                     [FromQuery] int idUsuarioAccion) =>
            {
                try
                {
                    var result = await service.GetAllAsync(idUsuarioAccion);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // GET - Por ID
            group.MapGet("/{id:int}", async (int id,
                                             [FromQuery] int idUsuarioAccion,
                                             [FromServices] IModuloService service) =>
            {
                try
                {
                    var modulo = await service.GetByIdAsync(id, idUsuarioAccion);
                    return modulo is null
                        ? Results.NotFound(new { error = $"Módulo {id} no encontrado." })
                        : Results.Ok(modulo);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // POST - Crear
            group.MapPost("/", async ([FromBody] Modulo modulo,
                                      [FromQuery] int idUsuarioAccion,
                                      [FromServices] IModuloService service) =>
            {
                try
                {
                    await service.CrearAsync(modulo, idUsuarioAccion);
                    var creado = await service.GetByIdAsync(modulo.ID_Modulo, idUsuarioAccion);
                    return Results.Created($"/modulo/{modulo.ID_Modulo}", creado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // PUT - Actualizar
            group.MapPut("/{id:int}", async (int id,
                                             [FromBody] Modulo modulo,
                                             [FromQuery] int idUsuarioAccion,
                                             [FromServices] IModuloService service) =>
            {
                try
                {
                    modulo.ID_Modulo = id;
                    await service.ActualizarAsync(modulo, idUsuarioAccion);
                    var actualizado = await service.GetByIdAsync(id, idUsuarioAccion);
                    return Results.Ok(actualizado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // DELETE - Eliminar
            group.MapDelete("/{id:int}", async (int id,
                                                [FromQuery] int idUsuarioAccion,
                                                [FromServices] IModuloService service) =>
            {
                try
                {
                    var modulo = await service.GetByIdAsync(id, idUsuarioAccion);
                    if (modulo is null)
                        return Results.NotFound(new { error = $"Módulo {id} no encontrado." });

                    await service.EliminarAsync(id, idUsuarioAccion);
                    return Results.Ok(modulo);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}

