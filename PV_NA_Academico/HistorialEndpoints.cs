using PV_NA_Academico.Services;

namespace PV_NA_Academico
{
    public static class HistorialEndpoints
    {
        public static void MapHistorialEndpoints(this WebApplication app)
        {
            
            app.MapGet("/historialacademico", async (
                string tipoIdentificacion,
                string identificacion,
                int idUsuario,
                HistorialService service) =>
            {
                var result = await service.ObtenerHistorialAsync(tipoIdentificacion, identificacion, idUsuario);

                if (!result.Any())
                    return Results.NotFound(new { message = "No se encontraron registros de historial académico para este estudiante." });

                return Results.Ok(result);
            })
            .WithName("ObtenerHistorialAcademico")
            .WithSummary("Consulta el historial académico de un estudiante.")
            .WithDescription("Devuelve los cursos con su promedio y periodo a partir de la tabla HistorialAcademico.");
        }
    }
}
