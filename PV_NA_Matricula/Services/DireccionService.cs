using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;

namespace PV_NA_Matricula.Services
{
	public class DireccionService : IDireccionService
	{
		private readonly IDireccionRepository _repo;

		public DireccionService(IDireccionRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<Provincia>> GetProvinciasAsync() => await _repo.GetProvinciasAsync();

		public async Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia)
		{
			if (idProvincia <= 0)
				throw new Exception("El parámetro ID_Provincia es requerido y debe ser válido.");
			return await _repo.GetCantonesPorProvinciaAsync(idProvincia);
		}

		public async Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton)
		{
			if (idProvincia <= 0 || idCanton <= 0)
				throw new Exception("Los parámetros ID_Provincia e ID_Canton son requeridos y deben ser válidos.");
			return await _repo.GetDistritosAsync(idProvincia, idCanton);
		}
	}
}
 