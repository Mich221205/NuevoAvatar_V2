using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class EstudianteEndpoints
    {
        public static void MapEstudianteEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Obtener todos los expedientes de estudiantes
            // ======================================================
            app.MapGet("/expediente", async (int idUsuario, IEstudianteService service) =>
            {
                var estudiantes = await service.GetAllAsync(idUsuario);
                return Results.Ok(estudiantes);
            })
            .WithSummary("Obtiene la lista completa de expedientes de estudiantes y registra la acción en bitácora.");

            // ======================================================
            //  Obtener expediente por ID
            // ======================================================
            app.MapGet("/expediente/{id:int}", async (int id, int idUsuario, IEstudianteService service) =>
            {
                var estudiante = await service.GetByIdAsync(id, idUsuario);
                return estudiante is not null
                    ? Results.Ok(estudiante)
                    : Results.NotFound(new { message = "Expediente no encontrado." });
            })
            .WithSummary("Obtiene un expediente específico por su ID y registra la acción en bitácora.");

            // ======================================================
            //  Crear nuevo expediente
            // ======================================================
            app.MapPost("/expediente", async (Estudiante estudiante, int idUsuario, IEstudianteService service) =>
            {
                var id = await service.CreateAsync(estudiante, idUsuario);
                return Results.Created($"/expediente/{id}", estudiante);
            })
            .WithSummary("Crea un nuevo expediente de estudiante y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar expediente existente
            // ======================================================
            app.MapPut("/expediente/{id:int}", async (int id, Estudiante estudiante, int idUsuario, IEstudianteService service) =>
            {
                if (id != estudiante.ID_Estudiante)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el del expediente." });

                await service.UpdateAsync(estudiante, idUsuario);
                return Results.Ok(new { message = "Expediente actualizado correctamente." });
            })
            .WithSummary("Actualiza los datos de un expediente existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar expediente
            // ======================================================
            app.MapDelete("/expediente/{id:int}", async (int id, int idUsuario, IEstudianteService service) =>
            {
                await service.DeleteAsync(id, idUsuario);
                return Results.Ok(new { message = "Expediente eliminado correctamente." });
            })
            .WithSummary("Elimina un expediente de estudiante por su ID y registra la acción en bitácora.");
        }
    }
}
