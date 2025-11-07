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
            //  -> Devuelve 201 Created con el objeto creado como body
            // ======================================================
            app.MapPost("/expediente", async (Estudiante estudiante, int idUsuario, IEstudianteService service) =>
            {
                var id = await service.CreateAsync(estudiante, idUsuario);

                // Volvemos a leer para responder con el body creado
                var creado = await service.GetByIdAsync(id, idUsuario);

                return creado is null
                    ? Results.Created($"/expediente/{id}", estudiante) // fallback si no se pudo leer
                    : Results.Created($"/expediente/{id}", creado);
            })
            .WithSummary("Crea un nuevo expediente de estudiante y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar expediente existente
            //  -> Devuelve 200 OK con el objeto actualizado como body
            // ======================================================
            app.MapPut("/expediente/{id:int}", async (int id, Estudiante estudiante, int idUsuario, IEstudianteService service) =>
            {
                if (id != estudiante.ID_Estudiante)
                    return Results.BadRequest(new { message = "El ID de la URL no coincide con el del expediente." });

                await service.UpdateAsync(estudiante, idUsuario);

                // Leemos el registro actualizado para retornarlo en el body
                var actualizado = await service.GetByIdAsync(id, idUsuario);
                return actualizado is not null
                    ? Results.Ok(actualizado)
                    : Results.Ok(new { message = "Expediente actualizado correctamente." });
            })
            .WithSummary("Actualiza los datos de un expediente existente y registra la acción en bitácora.");

            // ======================================================
            //  Eliminar expediente
            //  -> Devuelve 200 OK con el objeto eliminado como body
            // ======================================================
            app.MapDelete("/expediente/{id:int}", async (int id, int idUsuario, IEstudianteService service) =>
            {
                // Leemos ANTES de eliminar para poder devolver el body del eliminado
                var existente = await service.GetByIdAsync(id, idUsuario);
                if (existente is null)
                    return Results.NotFound(new { message = "Expediente no encontrado." });

                // ⬇️ CAMBIO: pasar el body al service para que Bitácora lo guarde
                await service.DeleteAsync(id, idUsuario, existente);

                return Results.Ok(existente); // cuerpo con el expediente eliminado
            })
            .WithSummary("Elimina un expediente de estudiante por su ID y registra la acción en bitácora.");
        }
    }
}
