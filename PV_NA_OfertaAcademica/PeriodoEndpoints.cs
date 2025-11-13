using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Services;
using PV_NA_OfertaAcademica.Entities;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica.Controllers
{
    public static class PeriodoEndpoints
    {
        public static void MapPeriodoEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/periodo")
                           .WithOpenApi()
                           .WithTags("Periodo");
            //.RequireAuthorization(); // 🔒 Token validado con USR5

            
            group.MapGet("/", async (PeriodoService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var periodos = await service.ObtenerTodosAsync(idUsuario);
                return Results.Ok(periodos);
            })
            .WithSummary("Obtiene todos los periodos")
            .WithDescription("Retorna la lista completa de periodos registrados.");

            
            group.MapGet("/{id:int}", async (int id, PeriodoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var periodo = await service.ObtenerPorIdAsync(id, idUsuario);

                return periodo is not null
                    ? Results.Ok(periodo)
                    : Results.NotFound("Periodo no encontrado.");
            })
            .WithSummary("Obtiene un periodo por su ID.");

            
            group.MapPost("/", async (PeriodoCreateDto dto, PeriodoService service, HttpContext ctx) =>
            {
                var errores = ValidarPeriodo(dto.Anio, dto.Numero_Periodo, dto.Fecha_Inicio, dto.Fecha_Fin);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                await service.CrearAsync(dto, idUsuario);

                return Results.Created($"/periodo/{dto.Anio}-{dto.Numero_Periodo}", dto);
            })
            .WithSummary("Crea un nuevo periodo")
            .WithDescription("Registra un nuevo periodo en la base de datos y genera una bitácora.");

            
            group.MapPut("/{id:int}", async (int id, PeriodoUpdateDto dto, PeriodoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                dto.ID_Periodo = id;

                var errores = ValidarPeriodo(dto.Anio, dto.Numero_Periodo, dto.Fecha_Inicio, dto.Fecha_Fin);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                var existente = await service.ObtenerPorIdAsync(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe un periodo con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                await service.ModificarAsync(dto, idUsuario);

                return Results.Ok("Periodo actualizado correctamente.");
            })
            .WithSummary("Modifica un periodo existente.");

            
            group.MapDelete("/{id:int}", async (int id, PeriodoService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var existente = await service.ObtenerPorIdAsync(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe un periodo con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                await service.EliminarAsync(id, idUsuario);

                return Results.Ok($"Periodo con ID {id} eliminado correctamente.");
            })
            .WithSummary("Elimina un periodo existente.");
        }

        private static List<string> ValidarPeriodo(int anio, int numeroPeriodo, DateTime fechaInicio, DateTime fechaFin)
        {
            var errores = new List<string>();

            if (anio < 2000 || anio > 2100)
                errores.Add("El año del periodo no es válido. (Debe estar entre 2000 y 2100)");

            if (numeroPeriodo <= 0 || numeroPeriodo > 4)
                errores.Add("El número de periodo debe estar entre 1 y 4.");

            if (fechaFin < fechaInicio)
                errores.Add("La fecha de fin debe ser posterior o igual a la fecha de inicio.");

            return errores;
        }
    }
}
