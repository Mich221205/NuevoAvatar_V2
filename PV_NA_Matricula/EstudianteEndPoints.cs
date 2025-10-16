using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
	public static class EstudianteEndPoints
	{
		public static void MapEstudianteEndpoints(this WebApplication app)
		{
			var group = app.MapGroup("/expediente");

			group.MapGet("/", async (IEstudianteService service) =>
			{
				var data = await service.GetAllAsync();
				return Results.Ok(data);
			});

			group.MapGet("/{id}", async (int id, IEstudianteService service) =>
			{
				var item = await service.GetByIdAsync(id);
				return item is not null ? Results.Ok(item) : Results.NotFound();
			});

			group.MapPost("/", async (Estudiante e, IEstudianteService service) =>
			{
				var id = await service.CreateAsync(e);
				return Results.Created($"/expediente/{id}", e);
			});

			group.MapPut("/{id}", async (int id, Estudiante e, IEstudianteService service) =>
			{
				if (id != e.ID_Estudiante) return Results.BadRequest();
				await service.UpdateAsync(e);
				return Results.NoContent(); 
			});

			group.MapDelete("/{id}", async (int id, IEstudianteService service) =>
			{
				await service.DeleteAsync(id);
				return Results.NoContent();
			});
		}
	}
}
