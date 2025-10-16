using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public interface IDireccionRepository
	{
		Task<IEnumerable<Provincia>> GetProvinciasAsync();
		Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia);
		Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton);
	}
}
 