using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
	public interface IDireccionService
	{
		Task<IEnumerable<Provincia>> GetProvinciasAsync();
		Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia);
		Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton);
	}
}
 