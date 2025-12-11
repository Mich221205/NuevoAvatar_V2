using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;
using System.Text.Json;

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

                var creado = await service.GetByIdAsync(id, idUsuario);

                return creado is null
                    ? Results.Created($"/expediente/{id}", estudiante)
                    : Results.Created($"/expediente/{id}", creado);
            })
            .WithSummary("Crea un nuevo expediente de estudiante y registra la acción en bitácora.");

            // ======================================================
            //  Actualizar expediente existente (NUEVO PUT SIMPLE)
            // ======================================================
            app.MapPut("/expediente/{id:int}", async (
                int id,
                int idUsuario,
                HttpRequest request,
                IEstudianteService service
            ) =>
            {
                // Leer json body dinámico (para recibir SOLO: Telefono + Direccion)
                var body = await request.ReadFromJsonAsync<Dictionary<string, object>>();
                if (body == null)
                    return Results.BadRequest(new { message = "Body inválido." });

                string telefono = body.ContainsKey("Telefono")
                    ? body["Telefono"]?.ToString() ?? ""
                    : "";

                string direccion = body.ContainsKey("Direccion")
                    ? body["Direccion"]?.ToString() ?? ""
                    : "";

                // Llamar servicio especializado
                await service.UpdateTelefonoDireccionAsync(
                    id,
                    telefono,
                    direccion,
                    idUsuario
                );

                // Devolver el estudiante actualizado
                var actualizado = await service.GetByIdAsync(id, idUsuario);
                return Results.Ok(actualizado);
            })
            .WithSummary("Actualiza teléfono y dirección del expediente (versión simplificada para app móvil).");

            // ======================================================
            //  Eliminar expediente
            // ======================================================
            app.MapDelete("/expediente/{id:int}", async (int id, int idUsuario, IEstudianteService service) =>
            {
                var existente = await service.GetByIdAsync(id, idUsuario);
                if (existente is null)
                    return Results.NotFound(new { message = "Expediente no encontrado." });

                await service.DeleteAsync(id, idUsuario, existente);

                return Results.Ok(existente);
            })
            .WithSummary("Elimina un expediente de estudiante por su ID y registra la acción en bitácora.");
        }
    }
}
