using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class MatriculaEndpoints
    {
        public static void MapMatriculaEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Crear una nueva matrícula
            // ======================================================
            app.MapPost("/matricula", async (Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                var id = await service.CreateAsync(matricula, idUsuario);
                return Results.Created($"/matricula/{id}", matricula);
            })
            .WithSummary("Crea una nueva matrícula para un estudiante y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar una matrícula existente
            // ======================================================
            app.MapPut("/matricula/{id:int}", async (int id, Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                if (id != matricula.ID_Matricula)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el de la matrícula." });

                await service.UpdateAsync(matricula, idUsuario);
                return Results.Ok(new { message = "Matrícula actualizada correctamente." });
            })
            .WithSummary("Actualiza los datos de una matrícula existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar una matrícula
            // ======================================================
            app.MapDelete("/matricula/{id:int}", async (int id, int idUsuario, IMatriculaService service) =>
            {
                await service.DeleteAsync(id, idUsuario);
                return Results.Ok(new { message = "Matrícula eliminada correctamente." });
            })
            .WithSummary("Elimina una matrícula por su ID y registra la acción en bitácora.");

            // ======================================================
            //  Listar estudiantes matriculados por curso y grupo
            // ======================================================
            app.MapGet("/matricula/curso/{idCurso:int}/grupo/{idGrupo:int}", async (int idCurso, int idGrupo, int idUsuario, IMatriculaService service) =>
            {
                var estudiantes = await service.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo, idUsuario);
                return estudiantes.Any()
                    ? Results.Ok(estudiantes)
                    : Results.NotFound(new { message = "No hay estudiantes matriculados en ese curso y grupo." });
            })
            .WithSummary("Obtiene los estudiantes matriculados según curso y grupo y registra la acción en bitácora.");
        }
    }
}
