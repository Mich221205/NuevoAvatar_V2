using PV_NA_Academico.Services;

namespace PV_NA_Academico
{
    public static class ListadoEstudiantesEndpoints
    {
        public static void MapListadoEstudiantesEndpoints(this WebApplication app)
        {
            app.MapGet("/listadoestudiantes", async (
                string periodo,
                int idUsuario,
                ListadoEstudiantesService service) =>
            {
                var result = await service.ObtenerPorPeriodoAsync(periodo, idUsuario);
                if (!result.Any())
                    return Results.NotFound(new { message = "No se encontraron estudiantes para el período indicado." });

                return Results.Ok(result);
            })
            .WithName("ObtenerListadoEstudiantesPeriodo")
            .WithSummary("Lista estudiantes matriculados en un período.")
            .WithDescription("Devuelve Periodo, TipoIdentificacion, Identificacion, NombreCompleto, Carrera, Curso, Grupo desde ListadoEstudiantesPeriodo.");
        }
    }
}
