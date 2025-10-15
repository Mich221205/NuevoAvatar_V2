using Microsoft.AspNetCore.Mvc;
using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Services;

namespace PV_NA_UsuariosRoles.Endpoints
{
    public static class ParametroEndpoints
    {
        public static void MapParametroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/parametro")
                              .WithTags("Parámetros")
                              .RequireAuthorization();

            // GET - Todos
            group.MapGet("/", async ([FromServices] IParametroService service,
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
            group.MapGet("/{id}", async (string id,
                                         [FromQuery] int idUsuarioAccion,
                                         [FromServices] IParametroService service) =>
            {
                try
                {
                    var parametro = await service.GetByIdAsync(id, idUsuarioAccion);
                    return parametro is null
                        ? Results.NotFound(new { error = $"Parámetro {id} no encontrado." })
                        : Results.Ok(parametro);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // POST - Crear
            group.MapPost("/", async ([FromBody] Parametro parametro,
                                      [FromQuery] int idUsuarioAccion,
                                      [FromServices] IParametroService service) =>
            {
                try
                {
                    await service.CrearAsync(parametro, idUsuarioAccion);
                    var creado = await service.GetByIdAsync(parametro.ID_Parametro, idUsuarioAccion);
                    return Results.Created($"/parametro/{parametro.ID_Parametro}", creado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // PUT - Actualizar
            group.MapPut("/{id}", async (string id,
                                         [FromBody] Parametro parametro,
                                         [FromQuery] int idUsuarioAccion,
                                         [FromServices] IParametroService service) =>
            {
                try
                {
                    parametro.ID_Parametro = id;
                    await service.ActualizarAsync(parametro, idUsuarioAccion);
                    var actualizado = await service.GetByIdAsync(id, idUsuarioAccion);
                    return Results.Ok(actualizado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // DELETE - Eliminar
            group.MapDelete("/{id}", async (string id,
                                            [FromQuery] int idUsuarioAccion,
                                            [FromServices] IParametroService service) =>
            {
                try
                {
                    var parametro = await service.GetByIdAsync(id, idUsuarioAccion);
                    if (parametro is null)
                        return Results.NotFound(new { error = $"Parámetro {id} no encontrado." });

                    await service.EliminarAsync(id, idUsuarioAccion);
                    return Results.Ok(parametro);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}
