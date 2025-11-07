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
            //  201 Created con el objeto creado como body
            // ======================================================
            app.MapPost("/matricula", async (Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                var id = await service.CreateAsync(matricula, idUsuario);

                // Intentamos leer lo recién creado para devolverlo como body
                var creada = await service.GetByIdAsync(id, idUsuario);

                return creada is null
                    ? Results.Created($"/matricula/{id}", matricula) // fallback si no se pudo leer
                    : Results.Created($"/matricula/{id}", creada);
            })
            .WithSummary("Crea una nueva matrícula para un estudiante y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar una matrícula existente
            //  -> 200 OK con el objeto actualizado como body
            // ======================================================
            app.MapPut("/matricula/{id:int}", async (int id, Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                if (id != matricula.ID_Matricula)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el de la matrícula." });

                // Actualizamos
                await service.UpdateAsync(matricula, idUsuario);

                // Leemos la matrícula actualizada para retornarla en el body
                var actualizada = await service.GetByIdAsync(id, idUsuario);
                return actualizada is not null
                    ? Results.Ok(actualizada)
                    : Results.Ok(new { message = "Matrícula actualizada correctamente." });
            })
            .WithSummary("Actualiza los datos de una matrícula existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar una matrícula
            //  -> 200 OK con el objeto eliminado como body
            // ======================================================
            app.MapDelete("/matricula/{id:int}", async (int id, int idUsuario, IMatriculaService service) =>
            {
                // Leemos ANTES de eliminar para poder devolver el body del eliminado
                var existente = await service.GetByIdAsync(id, idUsuario);
                if (existente is null)
                    return Results.NotFound(new { message = "Matrícula no encontrada." });

                // ⬇️ ÚNICO CAMBIO: pasar el body al service para bitácora
                await service.DeleteAsync(id, idUsuario, existente);

                return Results.Ok(existente); // devolvemos la matrícula eliminada
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
