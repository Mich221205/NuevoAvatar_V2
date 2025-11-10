using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Services;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica
{
    public static class GrupoEndpoints
    {
        public static void MapGrupoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/grupo")
                           .WithOpenApi()
                           .WithTags("Grupo");
                          // .RequireAuthorization(); // exige token válido (USR5)

            //  Obtener todos los grupos
            group.MapGet("/", async (IGrupoService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var grupos = await service.ObtenerTodosAsync(idUsuario);
                return Results.Ok(grupos);
            })
            .WithSummary("Obtiene todos los grupos")
            .WithDescription("Retorna el listado completo de grupos registrados.");

            //  Obtener grupo por ID
            group.MapGet("/{id:int}", async (int id, IGrupoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var grupo = await service.ObtenerPorIdAsync(id, idUsuario);

                return grupo is not null
                    ? Results.Ok(grupo)
                    : Results.NotFound("Grupo no encontrado.");
            })
            .WithSummary("Obtiene un grupo por su ID.");

            //  Crear nuevo grupo
            group.MapPost("/", async (GrupoCreateDto dto, IGrupoService service, HttpContext ctx) =>
            {
                var errores = ValidarGrupo(dto);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var creado = await service.CrearAsync(dto, idUsuario);

                return creado
                    ? Results.Created($"/grupo/{dto.Numero_Grupo}", dto)
                    : Results.Problem("Error al crear el grupo.");
            })
            .WithSummary("Crea un nuevo grupo.");

            //  Modificar grupo
            group.MapPut("/{id:int}", async (int id, GrupoUpdateDto dto, IGrupoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                dto.ID_Grupo = id;

                var errores = ValidarGrupo(dto);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                var existente = await service.ObtenerPorIdAsync(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe un grupo con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var actualizado = await service.ModificarAsync(dto, idUsuario);

                return actualizado
                    ? Results.Ok("Grupo actualizado correctamente.")
                    : Results.Problem("Error al actualizar el grupo.");
            })
            .WithSummary("Modifica un grupo existente.");

            //  Eliminar grupo
            group.MapDelete("/{id:int}", async (int id, IGrupoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var existente = await service.ObtenerPorIdAsync(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe un grupo con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var eliminado = await service.EliminarAsync(id, idUsuario);

                return eliminado
                    ? Results.Ok($"Grupo con ID {id} eliminado correctamente.")
                    : Results.Problem("Error al eliminar el grupo.");
            })
            .WithSummary("Elimina un grupo existente.");
        }

        //  Validaciones del DTO
        private static List<string> ValidarGrupo(GrupoCreateDto dto)
        {
            var errores = new List<string>();

            if (dto.Numero_Grupo <= 0)
                errores.Add("El número de grupo debe ser mayor a 0.");

            if (dto.ID_Curso <= 0)
                errores.Add("Debe indicar un curso válido.");

            if (dto.ID_Profesor <= 0)
                errores.Add("Debe indicar un profesor válido.");

            if (dto.ID_Periodo <= 0)
                errores.Add("Debe indicar un periodo válido.");

            if (string.IsNullOrWhiteSpace(dto.Horario))
                errores.Add("El horario es obligatorio.");

            return errores;
        }
    }
}
