using Microsoft.AspNetCore.Mvc;
using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
    public static class MobileEndPoints
    {
        public static void MapMobileEndPoints(this WebApplication app)
        {
            app.MapGet("/mobile/resumen/{idEstudiante}", async (
                HttpContext ctx,
                int idEstudiante,
                [FromServices] PreMatriculaRepository preRepo,
                [FromServices] MatriculaRepository matRepo,
                [FromServices] PagosService pagosService) =>
            {
                var rawToken = ctx.Request.Headers["Authorization"].ToString();
                var token = rawToken.Replace("Bearer ", "").Trim();

                Console.WriteLine($"TOKEN RECIBIDO EN MOBILE: {token}");

                var resumen = new ResumenEstudiante
                {
                    PrematriculasActivas = await preRepo.CountActivasAsync(idEstudiante),
                    MatriculasPeriodo = await matRepo.CountPeriodoActualAsync(idEstudiante),
                    CursosActuales = await matRepo.CountCursosActualesAsync(idEstudiante),
                    FacturasPendientes = await pagosService.CountFacturasPendientesAsync(idEstudiante, token)
                };

                return Results.Ok(resumen);
            });
        }
    }
}
