using PV_NA_Matricula.Entities;
namespace PV_NA_Matricula.Repository
{
	public interface IPreMatriculaRepository
	{
		Task<IEnumerable<PreMatricula>> GetAllAsync();
		Task<PreMatricula?> GetByIdAsync(int id);
		Task<int> InsertAsync(PreMatricula pre);
		Task<int> UpdateAsync(PreMatricula pre);
		Task<int> DeleteAsync(int id);
	}
}
 