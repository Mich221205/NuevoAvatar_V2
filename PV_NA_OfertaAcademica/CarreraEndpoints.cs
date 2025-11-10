using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Services;
using System.Security.Claims;

namespace PV_NA_OfertaAcademica
{
    public static class CarreraEndpoints
    {
        public static void MapCarreraEndpoints(this IEndpointRouteBuilder app)
        {
            //  Obtener todas las carreras
            app.MapGet("/carrera", async (ICarreraService service, HttpContext ctx) =>
            {
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var carreras = await service.GetAll(idUsuario);
                return Results.Ok(carreras);
            });


            //  Obtener carrera por ID
            app.MapGet("/carrera/{id:int}", async (int id, ICarreraService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var carrera = await service.GetById(id, idUsuario);
                return carrera is not null ? Results.Ok(carrera) : Results.NotFound("Carrera no encontrada.");
            });


            //  Obtener carreras por institución
            app.MapGet("/carrera/institucion/{idInstitucion:int}", async (int idInstitucion, ICarreraService service, HttpContext ctx) =>
            {
                if (idInstitucion <= 0)
                    return Results.BadRequest("El ID de la institución debe ser mayor a 0.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var carreras = await service.GetByInstitucion(idInstitucion, idUsuario);
                return Results.Ok(carreras);
            });


            //  Crear carrera
            app.MapPost("/carrera", async (Carrera carrera, ICarreraService service, HttpContext ctx) =>
            {
                var errores = ValidarCarrera(carrera);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                // Se obtiene el claim del usuario autenticado
                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";

                var creada = await service.Create(carrera, idUsuario);

                return creada
                    ? Results.Created($"/carrera/{carrera.ID_Carrera}", carrera)
                    : Results.Problem("Error al crear la carrera.");
            });


            //  Actualizar carrera
            app.MapPut("/carrera/{id:int}", async (int id, Carrera carrera, ICarreraService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                carrera.ID_Carrera = id;

                var errores = ValidarCarrera(carrera);
                if (errores.Any())
                    return Results.BadRequest(new { Mensaje = "Datos inválidos", Errores = errores });

                var existente = await service.GetById(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe una carrera con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var actualizada = await service.Update(carrera, idUsuario);

                return actualizada
                    ? Results.Ok("Carrera actualizada correctamente.")
                    : Results.Problem("Error al actualizar la carrera.");
            });


            //  Eliminar carrera
            app.MapDelete("/carrera/{id:int}", async (int id, ICarreraService service, HttpContext ctx) =>
            {
                if (id <= 0)
                    return Results.BadRequest("El ID debe ser mayor a 0.");

                var existente = await service.GetById(id, "Sistema");
                if (existente is null)
                    return Results.NotFound("No existe una carrera con ese ID.");

                var idUsuario = ctx.User.FindFirst("usuarioID")?.Value ?? "0";
                var eliminada = await service.Delete(id, idUsuario);

                return eliminada
                    ? Results.Ok("Carrera eliminada correctamente.")
                    : Results.Problem("Error al eliminar la carrera.");
            });
           
        }

        //  Método auxiliar de validación
        private static List<string> ValidarCarrera(Carrera carrera)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(carrera.Nombre))
                errores.Add("El nombre de la carrera es obligatorio.");
            else if (!carrera.Nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                errores.Add("El nombre de la carrera solo puede contener letras y espacios.");

            if (carrera.ID_Institucion <= 0)
                errores.Add("Debe indicar una institución válida.");

            if (carrera.ID_Profesor_Director <= 0)
                errores.Add("Debe indicar un profesor director válido.");

            return errores;
        }
    }
}
