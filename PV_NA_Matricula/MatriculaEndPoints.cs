using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
	public static class MatriculaEndPoints
	{
		public static void MapMatriculaEndpoints(this WebApplication app)
		{
			var group = app.MapGroup("/matricula");

			group.MapPost("/", async (Matricula m, IMatriculaService service) =>
			{
				var id = await service.CreateAsync(m);
				return Results.Created($"/matricula/{id}", m);
			});

			group.MapPut("/{id}", async (int id, Matricula m, IMatriculaService service) =>
			{
				if (id != m.ID_Matricula) return Results.BadRequest();
				await service.UpdateAsync(m);
				return Results.NoContent();
			});

			group.MapDelete("/{id}", async (int id, IMatriculaService service) =>
			{
				await service.DeleteAsync(id);
				return Results.NoContent();
			});

			group.MapGet("/curso/{idCurso}/grupo/{idGrupo}", async (int idCurso, int idGrupo, IMatriculaService service) =>
			{
				var data = await service.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo);
				return data.Any() ? Results.Ok(data) : Results.NotFound("No hay estudiantes matriculados en ese curso y grupo.");
			});
		}
	} 
}