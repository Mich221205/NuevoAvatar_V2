using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Services;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica
{
    public static class InstitucionEndpoints
    {
        public static void MapInstitucionEndpoints(this IEndpointRouteBuilder app)
        {
            // Obtener todas las instituciones
            app.MapGet("/institucion", async (IInstitucionService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var instituciones = await service.GetAll(idUsuario);
                return Results.Ok(instituciones);
            });

            // Obtener institución por ID
            app.MapGet("/institucion/{id:int}", async (int id, IInstitucionService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var institucion = await service.GetById(id, idUsuario);

                return institucion is not null
                    ? Results.Ok(institucion)
                    : Results.NotFound("Institución no encontrada.");
            });

            // Crear institución
            app.MapPost("/institucion", async (Institucion inst, IInstitucionService service, HttpContext ctx) =>
            {
                if (string.IsNullOrWhiteSpace(inst.Nombre))
                    return Results.BadRequest("El nombre de la institución es obligatorio.");
                if (!inst.Nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return Results.BadRequest("El nombre solo puede contener letras y espacios.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    var creado = await service.Create(inst, idUsuario);
                    return creado
                        ? Results.Created($"/institucion/{inst.ID_Institucion}", inst)
                        : Results.Problem("No se pudo crear la institución.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });

            // Actualizar institución
            app.MapPut("/institucion/{id:int}", async (int id, Institucion inst, IInstitucionService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");
                if (string.IsNullOrWhiteSpace(inst.Nombre))
                    return Results.BadRequest("El nombre de la institución es obligatorio.");
                if (!inst.Nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return Results.BadRequest("El nombre solo puede contener letras y espacios.");

                inst.ID_Institucion = id;
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    var actualizado = await service.Update(inst, idUsuario);
                    return actualizado
                        ? Results.Ok("Institución actualizada correctamente.")
                        : Results.NotFound("Institución no encontrada.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });

            // Eliminar institución
            app.MapDelete("/institucion/{id:int}", async (int id, IInstitucionService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                try
                {
                    var eliminado = await service.Delete(id, idUsuario);
                    return eliminado
                        ? Results.Ok($"Institución {id} eliminada correctamente.")
                        : Results.NotFound("Institución no encontrada.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            });
        }
    }
}
