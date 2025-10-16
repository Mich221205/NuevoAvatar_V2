using Dapper;
using PV_NA_Matricula.Entities;
using System.Data;

namespace PV_NA_Matricula.Repository
{
	public class DireccionRepository : IDireccionRepository
	{
		private readonly IDbConnectionFactory _factory;

		public DireccionRepository(IDbConnectionFactory factory)
		{
			_factory = factory;
		}

		public async Task<IEnumerable<Provincia>> GetProvinciasAsync()
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryAsync<Provincia>("SELECT ID_Provincia, Nombre FROM Provincia"); 
		}

		public async Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryAsync<Canton>(
				"SELECT ID_Canton, Nombre, ID_Provincia FROM Canton WHERE ID_Provincia = @idProvincia",
				new { idProvincia });
		}

		public async Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryAsync<Distrito>(
				@"SELECT ID_Distrito, Nombre, ID_Canton, ID_Provincia
                  FROM Distrito
                  WHERE ID_Provincia = @idProvincia AND ID_Canton = @idCanton",
				new { idProvincia, idCanton });
		}
	}
}
