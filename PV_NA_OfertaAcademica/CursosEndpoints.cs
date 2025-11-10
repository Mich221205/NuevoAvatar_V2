using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Services;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica
{
    public static class CursoEndpoints
    {
        public static void MapCursoEndpoints(this IEndpointRouteBuilder app)
        {
            // Obtener todos los cursos
            app.MapGet("/curso", async (ICursoService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var cursos = await service.Listar(idUsuario);
                return Results.Ok(cursos);
            });

            // Obtener curso por ID
            app.MapGet("/curso/{id:int}", async (int id, ICursoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var curso = await service.Obtener(id, idUsuario);

                return curso is not null ? Results.Ok(curso) : Results.NotFound("Curso no encontrado.");
            });

            // Obtener cursos por carrera
            app.MapGet("/curso/carrera/{idCarrera:int}", async (int idCarrera, ICursoService service, HttpContext ctx) =>
            {
                if (idCarrera <= 0)
                    return Results.BadRequest("El ID de la carrera debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    var cursos = await service.ListarPorCarrera(idCarrera, idUsuario);
                    return Results.Ok(cursos);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });

            // Crear curso
            app.MapPost("/curso", async (CursoCreateDto dto, ICursoService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    var id = await service.Crear(dto, idUsuario);
                    return Results.Created($"/curso/{id}", new { ID_Curso = id });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });

            // Actualizar curso
            app.MapPut("/curso/{id:int}", async (int id, CursoUpdateDto dto, ICursoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    await service.Actualizar(id, dto, idUsuario);
                    return Results.Ok("Curso actualizado correctamente.");
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound("No existe un curso con ese ID.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });

            // Eliminar curso
            app.MapDelete("/curso/{id:int}", async (int id, ICursoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    await service.Eliminar(id, idUsuario);
                    return Results.Ok("Curso eliminado correctamente.");
                }
                catch (KeyNotFoundException)
                {
                    return Results.NotFound("No existe un curso con ese ID.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });
        }
    }
}
