using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public interface IEstudianteRepository
	{
		Task<IEnumerable<Estudiante>> GetAllAsync();
		Task<Estudiante?> GetByIdAsync(int id);
		Task<int> InsertAsync(Estudiante e);
		Task<int> UpdateAsync(Estudiante e);
		Task<int> DeleteAsync(int id);
	}
}
 