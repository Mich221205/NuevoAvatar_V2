using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Services;
using Microsoft.AspNetCore.Mvc;

namespace PV_NA_UsuariosRoles.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/usuario")
                              .WithTags("Usuarios")
                              .RequireAuthorization();

            // GET - Listar todos
            group.MapGet("/", async ([FromServices] IUsuarioService service, [FromQuery] int idUsuarioAccion) =>
            {
                try
                {
                    var result = await service.GetAllAsync();
                    await service.GetByIdAsync(idUsuarioAccion); // registra consulta
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // GET - Buscar por ID
            group.MapGet("/{id:int}", async (int id, [FromQuery] int idUsuarioAccion, [FromServices] IUsuarioService service) =>
            {
                try
                {
                    var usuario = await service.GetByIdAsync(id);
                    if (usuario == null)
                        return Results.NotFound(new { error = $"Usuario {id} no encontrado." });

                    await service.GetByIdAsync(idUsuarioAccion);
                    return Results.Ok(usuario);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // GET - Filtrar
            group.MapGet("/filtrar", async (string? identificacion, string? nombre, string? tipo,
                [FromQuery] int idUsuarioAccion, [FromServices] IUsuarioService service) =>
            {
                try
                {
                    var result = await service.FilterAsync(identificacion, nombre, tipo);
                    await service.GetByIdAsync(idUsuarioAccion);
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // POST - Crear
            group.MapPost("/", async ([FromBody] Usuario usuario, [FromQuery] int idUsuarioAccion,
                [FromServices] IUsuarioService service) =>
            {
                try
                {
                    await service.CrearAsync(usuario, idUsuarioAccion);
                    var creado = await service.GetByIdAsync(usuario.ID_Usuario);
                    return Results.Created($"/usuario/{usuario.ID_Usuario}", creado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // PUT - Actualizar
            group.MapPut("/{id:int}", async (int id, [FromBody] Usuario usuario,
                [FromQuery] int idUsuarioAccion, [FromServices] IUsuarioService service) =>
            {
                try
                {
                    usuario.ID_Usuario = id;
                    await service.ActualizarAsync(usuario, idUsuarioAccion);
                    var actualizado = await service.GetByIdAsync(id);
                    return Results.Ok(actualizado);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // DELETE - Eliminar
            group.MapDelete("/{id:int}", async (int id, [FromQuery] int idUsuarioAccion,
                [FromServices] IUsuarioService service) =>
            {
                try
                {
                    var usuario = await service.GetByIdAsync(id);
                    if (usuario == null)
                        return Results.NotFound(new { error = $"Usuario {id} no encontrado." });

                    await service.EliminarAsync(id, idUsuarioAccion);
                    return Results.Ok(usuario);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}
