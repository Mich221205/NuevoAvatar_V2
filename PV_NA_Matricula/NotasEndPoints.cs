using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
	public static class NotasEndPoints
	{
		public static void MapNotasEndpoints(this WebApplication app)
		{
			var group = app.MapGroup("/Notas");

			// POST /mat5/cargardesglose
			group.MapPost("/cargardesglose", async (int idGrupo, List<NotaRubro> rubros, INotasService service) =>
			{
				var result = await service.CargarDesgloseAsync(idGrupo, rubros);
				return Results.Ok(new { Mensaje = $"Se cargaron {result} rubros correctamente (suma 100%)." });
			});

			// POST /mat5/asignarnotarubro
			group.MapPost("/asignarnotarubro", async (Nota nota, INotasService service) =>
			{
				var result = await service.AsignarNotaRubroAsync(nota);
				return Results.Ok(new { Mensaje = "Nota registrada o actualizada correctamente." });
			});

			// GET /mat5/obtenerdesglose/{idGrupo}
			group.MapGet("/obtenerdesglose/{idGrupo}", async (int idGrupo, INotasService service) =>
			{
				var data = await service.ObtenerDesgloseAsync(idGrupo);
				return Results.Ok(data);
			});

			// GET /mat5/obtenernotas?idEstudiante=&idGrupo=
			group.MapGet("/obtenernotas", async (int idEstudiante, int idGrupo, INotasService service) =>
			{
				var data = await service.ObtenerNotasAsync(idEstudiante, idGrupo);
				return Results.Ok(data);
			});
		} 
	}
}
