using PV_NA_Matricula.Services;

namespace PV_NA_Matricula
{
	public static class DireccionEndPoints
	{
		public static void MapDireccionEndpoints(this WebApplication app)
		{
			var group = app.MapGroup("/direccion");

			// /direccion/provincias
			group.MapGet("/provincias", async (IDireccionService service) =>
			{
				var data = await service.GetProvinciasAsync();
				return Results.Ok(data);
			});

			// /direccion/cantones?idProvincia=1
			group.MapGet("/cantones", async (int idProvincia, IDireccionService service) =>
			{
				var data = await service.GetCantonesPorProvinciaAsync(idProvincia);
				return data.Any() ? Results.Ok(data) : Results.NotFound("No se encontraron cantones para esa provincia.");
			});

			// /direccion/distritos?idProvincia=1&idCanton=3
			group.MapGet("/distritos", async (int idProvincia, int idCanton, IDireccionService service) =>
			{
				var data = await service.GetDistritosAsync(idProvincia, idCanton);
				return data.Any() ? Results.Ok(data) : Results.NotFound("No se encontraron distritos para esos parámetros.");
			});
		}
	}
}
 