using Microsoft.AspNetCore.Mvc;
using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Services;

namespace PV_NA_UsuariosRoles.Endpoints
{
    public static class RolEndpoints
    {
        public static void MapRolEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/rol")
                              .WithTags("Roles")
                              .RequireAuthorization();

            // GET - Obtener todos
            group.MapGet("/", async ([FromServices] IRolService service,
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

            // GET - Obtener por ID
            group.MapGet("/{id:int}", async (int id,
                                             [FromQuery] int idUsuarioAccion,
                                             [FromServices] IRolService service) =>
            {
                try
                {
                    var rol = await service.GetByIdAsync(id, idUsuarioAccion);
                    return rol is null
                        ? Results.NotFound(new { error = $"Rol {id} no encontrado." })
                        : Results.Ok(rol);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // POST - Crear
            group.MapPost("/", async ([FromBody] Rol rol,
                                      [FromQuery] int idUsuarioAccion,
                                      [FromServices] IRolService service) =>
            {
                try
                {
                    await service.CrearAsync(rol, idUsuarioAccion);
                    return Results.Created($"/rol/{rol.ID_Rol}", rol);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // PUT - Actualizar
            group.MapPut("/{id:int}", async (int id,
                                             [FromBody] Rol rol,
                                             [FromQuery] int idUsuarioAccion,
                                             [FromServices] IRolService service) =>
            {
                try
                {
                    rol.ID_Rol = id;
                    await service.ActualizarAsync(rol, idUsuarioAccion);
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
                                                [FromServices] IRolService service) =>
            {
                try
                {
                    var rol = await service.GetByIdAsync(id, idUsuarioAccion);
                    if (rol is null)
                        return Results.NotFound(new { error = $"Rol {id} no encontrado." });

                    await service.EliminarAsync(id, idUsuarioAccion);
                    return Results.Ok(rol);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}

