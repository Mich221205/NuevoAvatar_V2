using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
	public interface IPreMatriculaService
	{
		Task<IEnumerable<PreMatricula>> GetAllAsync();
		Task<PreMatricula?> GetByIdAsync(int id);
		Task<int> CreateAsync(PreMatricula pre);
		Task<int> UpdateAsync(PreMatricula pre);
		Task<int> DeleteAsync(int id);
	}
}
 