using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class MatriculaEndpoints
    {
        public static void MapMatriculaEndpoints(this WebApplication app)
        {
            app.MapPost("/matricula", async (Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                try
                {
                    var id = await service.CreateAsync(matricula, idUsuario);

                    var creada = await service.GetByIdAsync(id, idUsuario);

                    return creada is null
                        ? Results.Created($"/matricula/{id}", matricula) 
                        : Results.Created($"/matricula/{id}", creada);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new
                    {
                        message = ex.Message
                    });
                }
            })
            .WithSummary("Crea una nueva matrícula para un estudiante y registra la acción en bitácora.");

            
            app.MapPut("/matricula/{id:int}", async (int id, Matricula matricula, int idUsuario, IMatriculaService service) =>
            {
                if (id != matricula.ID_Matricula)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el de la matrícula." });

                try
                {
                    await service.UpdateAsync(matricula, idUsuario);

                    var actualizada = await service.GetByIdAsync(id, idUsuario);
                    return actualizada is not null
                        ? Results.Ok(actualizada)
                        : Results.Ok(new { message = "Matrícula actualizada correctamente." });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new
                    {
                        message = ex.Message
                    });
                }
            })
            .WithSummary("Actualiza los datos de una matrícula existente y registra la acción en bitácora.");

            app.MapDelete("/matricula/{id:int}", async (int id, int idUsuario, IMatriculaService service) =>
            {
                try
                {
                    var existente = await service.GetByIdAsync(id, idUsuario);
                    if (existente is null)
                        return Results.NotFound(new { message = "Matrícula no encontrada." });

                    await service.DeleteAsync(id, idUsuario, existente);

                    return Results.Ok(existente); 
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new
                    {
                        message = ex.Message
                    });
                }
            })
            .WithSummary("Elimina una matrícula por su ID y registra la acción en bitácora.");

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
