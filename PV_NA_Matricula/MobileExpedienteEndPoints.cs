using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class MobileExpedienteEndpoints
    {
        public static void MapMobileExpedienteEndpoints(this WebApplication app)
        {
            app.MapGet("/mobile/expediente/{idEstudiante:int}", async (
                int idEstudiante,
                IEstudianteService estudianteService) =>
            {
                int idUsuario = idEstudiante;

                var est = await estudianteService.GetByIdAsync(idEstudiante, idUsuario);
                if (est is null)
                {
                    return Results.NotFound(new { message = "Estudiante no encontrado." });
                }

                var dto = new ExpedienteMovilDto
                {
                    IdEstudiante = est.ID_Estudiante,
                    Identificacion = est.Identificacion,
                    TipoIdentificacion = est.Tipo_Identificacion,
                    Email = est.Email,
                    Nombre = est.Nombre,
                    Telefono = est.Telefono ?? string.Empty,
                    DireccionActual = est.Direccion ?? string.Empty
                };

                return Results.Ok(dto);
            })
            .WithSummary("Consulta el expediente del estudiante para la app móvil y registra en bitácora.");

            app.MapPut("/mobile/expediente/{idEstudiante:int}", async (
                int idEstudiante,
                ExpedienteMovilUpdate input,
                IEstudianteService estudianteService) =>
            {
                int idUsuario = idEstudiante;

                var est = await estudianteService.GetByIdAsync(idEstudiante, idUsuario);
                if (est is null)
                {
                    return Results.NotFound(new { message = "Estudiante no encontrado." });
                }

                est.Telefono = input.Telefono;

                est.Direccion = input.DetalleDireccion?.Trim() ?? string.Empty;

                await estudianteService.UpdateAsync(est, idUsuario);

                return Results.Ok(new { message = "Expediente actualizado correctamente." });
            })
            .WithSummary("Actualiza teléfono y dirección del expediente del estudiante desde la app móvil (solo texto libre, sin validaciones).");
        }
    }
}
