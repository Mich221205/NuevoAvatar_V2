using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;
using Microsoft.Extensions.Logging;

namespace PV_NA_Matricula
{
    public static class PreMatriculaEndpoints
    {
        public static void MapPreMatriculaEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Obtener todas las prematrículas
            // ======================================================
            app.MapGet("/prematricula", async (
                    int idUsuario,
                    IPreMatriculaService service,
                    ILogger<Program> logger) =>
            {
                try
                {
                    var prematriculas = await service.GetAllAsync(idUsuario);
                    return Results.Ok(prematriculas);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener las prematrículas para el usuario {IdUsuario}", idUsuario);
                    return Results.Problem(
                        detail: "Ocurrió un error al obtener las prematrículas.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithSummary("Obtiene todas las prematrículas registradas y registra la acción en bitácora.");

            // ======================================================
            //  Obtener una prematrícula por ID
            // ======================================================
            app.MapGet("/prematricula/{id:int}", async (
                    int id,
                    int idUsuario,
                    IPreMatriculaService service,
                    ILogger<Program> logger) =>
            {
                try
                {
                    var prematricula = await service.GetByIdAsync(id, idUsuario);
                    return prematricula is not null
                        ? Results.Ok(prematricula)
                        : Results.NotFound(new { message = "Prematrícula no encontrada." });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al obtener la prematrícula {Id} para el usuario {IdUsuario}", id, idUsuario);
                    return Results.Problem(
                        detail: "Ocurrió un error al obtener la prematrícula.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithSummary("Obtiene una prematrícula específica por su ID y registra la acción en bitácora.");

            // ======================================================
            //  Crear una nueva prematrícula
            // ======================================================
            app.MapPost("/prematricula", async (
                    PreMatricula prematricula,
                    int idUsuario,
                    IPreMatriculaService service,
                    ILogger<Program> logger) =>
            {
                try
                {
                    var id = await service.CreateAsync(prematricula, idUsuario);

                    // Leemos la recién creada para devolverla como body
                    var creada = await service.GetByIdAsync(id, idUsuario);

                    return creada is null
                        ? Results.Created($"/prematricula/{id}", prematricula) // fallback
                        : Results.Created($"/prematricula/{id}", creada);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al crear la prematrícula para el usuario {IdUsuario}", idUsuario);

                    string mensaje;
                    if (ex.Message.Contains("Ya existe una prematrícula", StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje = "Ya existe una prematrícula para este estudiante, curso y período.";
                    }
                    else if (ex.Message.Contains("periodos FUTUROS", StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje = "Solo se pueden prematricular periodos futuros (fecha de inicio posterior a la actual).";
                    }
                    else
                    {
                        mensaje = "Ocurrió un error al crear la prematrícula. Intente nuevamente.";
                    }

                    return Results.BadRequest(new { message = mensaje });
                }
            })
            .WithSummary("Crea una nueva prematrícula y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar una prematrícula existente
            // ======================================================
            app.MapPut("/prematricula/{id:int}", async (
                    int id,
                    PreMatricula prematricula,
                    int idUsuario,
                    IPreMatriculaService service,
                    ILogger<Program> logger) =>
            {
                if (id != prematricula.ID_Prematricula)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el de la prematrícula." });

                try
                {
                    await service.UpdateAsync(prematricula, idUsuario);

                    // Leemos la prematrícula actualizada para retornarla
                    var actualizada = await service.GetByIdAsync(id, idUsuario);
                    return actualizada is not null
                        ? Results.Ok(actualizada)
                        : Results.Ok(new { message = "Prematrícula actualizada correctamente." });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al actualizar la prematrícula {Id} para el usuario {IdUsuario}", id, idUsuario);

                    string mensaje;
                    if (ex.Message.Contains("Ya existe una prematrícula", StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje = "Ya existe una prematrícula para este estudiante, curso y período.";
                    }
                    else if (ex.Message.Contains("periodos FUTUROS", StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje = "Solo se pueden prematricular periodos futuros (fecha de inicio posterior a la actual).";
                    }
                    else
                    {
                        mensaje = "Ocurrió un error al actualizar la prematrícula. Intente nuevamente.";
                    }

                    return Results.BadRequest(new { message = mensaje });
                }
            })
            .WithSummary("Actualiza los datos de una prematrícula existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar una prematrícula
            // ======================================================
            app.MapDelete("/prematricula/{id:int}", async (
                    int id,
                    int idUsuario,
                    IPreMatriculaService service,
                    ILogger<Program> logger) =>
            {
                try
                {
                    // Leemos antes de eliminar para poder devolver el body del eliminado
                    var existente = await service.GetByIdAsync(id, idUsuario);
                    if (existente is null)
                        return Results.NotFound(new { message = "Prematrícula no encontrada." });

                    // pasar el body al service para que Bitácora lo guarde
                    await service.DeleteAsync(id, idUsuario, existente);

                    return Results.Ok(existente);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al eliminar la prematrícula {Id} para el usuario {IdUsuario}", id, idUsuario);

                    return Results.BadRequest(new
                    {
                        message = "Ocurrió un error al eliminar la prematrícula. Es posible que tenga relaciones asociadas o que la operación no pueda completarse."
                    });
                }
            })
            .WithSummary("Elimina una prematrícula por su ID y registra la acción en bitácora.");
        }
    }
}
