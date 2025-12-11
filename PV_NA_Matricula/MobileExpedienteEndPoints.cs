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
                IEstudianteService estudianteService,
                IDireccionService direccionService) =>
            {
                int idUsuario = idEstudiante; 

                var est = await estudianteService.GetByIdAsync(idEstudiante, idUsuario);
                if (est is null)
                {
                    return Results.NotFound(new { message = "Estudiante no encontrado." });
                }


                var provincias = await direccionService.GetProvinciasAsync(idUsuario);
                var provincia = provincias.FirstOrDefault(p => p.ID_Provincia == input.IdProvincia);
                if (provincia is null)
                {
                    return Results.BadRequest(new { message = "Provincia inválida." });
                }

                var cantones = await direccionService.GetCantonesPorProvinciaAsync(input.IdProvincia, idUsuario);
                var canton = cantones.FirstOrDefault(c => c.ID_Canton == input.IdCanton);
                if (canton is null)
                {
                    return Results.BadRequest(new { message = "Cantón inválido para esa provincia." });
                }

                var distritos = await direccionService.GetDistritosAsync(input.IdProvincia, input.IdCanton, idUsuario);
                var distrito = distritos.FirstOrDefault(d => d.ID_Distrito == input.IdDistrito);
                if (distrito is null)
                {
                    return Results.BadRequest(new { message = "Distrito inválido para esa provincia y cantón." });
                }

                est.Telefono = input.Telefono;
                est.Direccion =
                    $"{provincia.Nombre}, {canton.Nombre}, {distrito.Nombre}. {input.DetalleDireccion}".Trim();

                await estudianteService.UpdateAsync(est, idUsuario);

                return Results.Ok(new { message = "Expediente actualizado correctamente." });
            })
            .WithSummary("Actualiza teléfono y dirección del expediente del estudiante desde la app móvil, validando provincia/cantón/distrito y registrando en bitácora.");
        }
    }
}
