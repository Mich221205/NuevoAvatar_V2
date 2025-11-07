using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class NotasEndpoints
    {
        public static void MapNotasEndpoints(this WebApplication app)
        {
            // ======================================================
            //  Cargar desglose de rubros (por grupo)
            //  -> Devuelve OK con el desglose guardado como body
            // ======================================================
            app.MapPost("/notas/cargardesglose", async (int idGrupo, List<NotaRubro> rubros, int idUsuario, INotasService service) =>
            {
                var result = await service.CargarDesgloseAsync(idGrupo, rubros, idUsuario);

                // Leemos el desglose actualmente persistido para retornarlo en el body
                var desglose = await service.ObtenerDesgloseAsync(idGrupo, idUsuario);

                return Results.Ok(new
                {
                    message = $"Se cargaron {result} rubros correctamente (suma 100%).",
                    data = desglose
                });
            })
            .WithSummary("Carga el desglose de rubros de evaluación para un grupo específico y registra la acción en bitácora.");

            // ======================================================
            //  Asignar o actualizar nota de un rubro
            //  -> Devuelve OK con las notas actuales del estudiante en ese grupo como body
            // ======================================================
            app.MapPost("/notas/asignarnotarubro", async (Nota nota, int idUsuario, INotasService service) =>
            {
                var result = await service.AsignarNotaRubroAsync(nota, idUsuario);

                // Leemos el estado actual de las notas del estudiante en el grupo
                // (Esto evita depender de un GetById de nota y sigue tu contrato actual)
                var notasActuales = await service.ObtenerNotasAsync(nota.ID_Estudiante, nota.ID_Grupo, idUsuario);

                return Results.Ok(new
                {
                    message = "Nota registrada o actualizada correctamente.",
                    affected = result,
                    data = notasActuales
                });
            })
            .WithSummary("Registra o actualiza la nota de un estudiante para un rubro específico y registra la acción en bitácora.");

            // ======================================================
            //  Obtener desglose de rubros de un grupo
            // ======================================================
            app.MapGet("/notas/obtenerdesglose/{idGrupo:int}", async (int idGrupo, int idUsuario, INotasService service) =>
            {
                var data = await service.ObtenerDesgloseAsync(idGrupo, idUsuario);
                return data.Any()
                    ? Results.Ok(data)
                    : Results.NotFound(new { message = "No se encontraron rubros para este grupo." });
            })
            .WithSummary("Obtiene el desglose de rubros de evaluación para un grupo y registra la acción en bitácora.");

            // ======================================================
            //  Obtener notas de un estudiante en un grupo
            // ======================================================
            app.MapGet("/notas/obtenernotas", async (int idEstudiante, int idGrupo, int idUsuario, INotasService service) =>
            {
                var data = await service.ObtenerNotasAsync(idEstudiante, idGrupo, idUsuario);
                return data.Any()
                    ? Results.Ok(data)
                    : Results.NotFound(new { message = "No se encontraron notas para este estudiante en el grupo indicado." });
            })
            .WithSummary("Obtiene todas las notas de un estudiante en un grupo específico y registra la acción en bitácora.");
        }
    }
}