using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
	public static class PreMatriculaEndPoints
	{
		public static void MapPreMatriculaEndpoints(this WebApplication app)
		{
			var group = app.MapGroup("/prematricula");

			group.MapGet("/", async (IPreMatriculaService service) =>
			{
				var data = await service.GetAllAsync();
				return Results.Ok(data);
			}).RequireAuthorization();

			group.MapGet("/{id}", async (int id, IPreMatriculaService service) =>
			{
				var item = await service.GetByIdAsync(id);
				return item is not null ? Results.Ok(item) : Results.NotFound();
			});

			group.MapPost("/", async (PreMatricula pre, IPreMatriculaService service) =>
			{
				var id = await service.CreateAsync(pre);
				return Results.Created($"/prematricula/{id}", pre);
			}); 

			group.MapPut("/{id}", async (int id, PreMatricula pre, IPreMatriculaService service) =>
			{
				if (id != pre.ID_Prematricula) return Results.BadRequest();
				await service.UpdateAsync(pre);
				return Results.NoContent();
			});

			group.MapDelete("/{id}", async (int id, IPreMatriculaService service) =>
			{
				await service.DeleteAsync(id);
				return Results.NoContent();
			});
		}
	}
}
