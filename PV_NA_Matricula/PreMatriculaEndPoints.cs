using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class PreMatriculaEndpoints
    {
        public static void MapPreMatriculaEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Obtener todas las prematrículas
            // ======================================================
            app.MapGet("/prematricula", async (int idUsuario, IPreMatriculaService service) =>
            {
                var prematriculas = await service.GetAllAsync(idUsuario);
                return Results.Ok(prematriculas);
            })
            .WithSummary("Obtiene todas las prematrículas registradas y registra la acción en bitácora.");

            // ======================================================
            //  Obtener una prematrícula por ID
            // ======================================================
            app.MapGet("/prematricula/{id:int}", async (int id, int idUsuario, IPreMatriculaService service) =>
            {
                var prematricula = await service.GetByIdAsync(id, idUsuario);
                return prematricula is not null
                    ? Results.Ok(prematricula)
                    : Results.NotFound(new { message = "Prematrícula no encontrada." });
            })
            .WithSummary("Obtiene una prematrícula específica por su ID y registra la acción en bitácora.");

            // ======================================================
            //  Crear una nueva prematrícula
            // ======================================================
            app.MapPost("/prematricula", async (PreMatricula prematricula, int idUsuario, IPreMatriculaService service) =>
            {
                var id = await service.CreateAsync(prematricula, idUsuario);
                return Results.Created($"/prematricula/{id}", prematricula);
            })
            .WithSummary("Crea una nueva prematrícula y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar una prematrícula existente
            // ======================================================
            app.MapPut("/prematricula/{id:int}", async (int id, PreMatricula prematricula, int idUsuario, IPreMatriculaService service) =>
            {
                if (id != prematricula.ID_Prematricula)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el de la prematrícula." });

                await service.UpdateAsync(prematricula, idUsuario);
                return Results.Ok(new { message = "Prematrícula actualizada correctamente." });
            })
            .WithSummary("Actualiza los datos de una prematrícula existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar una prematrícula
            // ======================================================
            app.MapDelete("/prematricula/{id:int}", async (int id, int idUsuario, IPreMatriculaService service) =>
            {
                await service.DeleteAsync(id, idUsuario);
                return Results.Ok(new { message = "Prematrícula eliminada correctamente." });
            })
            .WithSummary("Elimina una prematrícula por su ID y registra la acción en bitácora.");
        }
    }
}
