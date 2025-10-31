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
            // ======================================================
            app.MapPost("/notas/cargardesglose", async (int idGrupo, List<NotaRubro> rubros, int idUsuario, INotasService service) =>
            {
                var result = await service.CargarDesgloseAsync(idGrupo, rubros, idUsuario);
                return Results.Ok(new { message = $"Se cargaron {result} rubros correctamente (suma 100%)." });
            })
            .WithSummary("Carga el desglose de rubros de evaluación para un grupo específico y registra la acción en bitácora.");

            // ======================================================
            //  Asignar o actualizar nota de un rubro
            // ======================================================
            app.MapPost("/notas/asignarnotarubro", async (Nota nota, int idUsuario, INotasService service) =>
            {
                var result = await service.AsignarNotaRubroAsync(nota, idUsuario);
                return Results.Ok(new { message = "Nota registrada o actualizada correctamente." });
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