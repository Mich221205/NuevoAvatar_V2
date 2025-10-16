using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
	public interface IEstudianteService
	{
		Task<IEnumerable<Estudiante>> GetAllAsync();
		Task<Estudiante?> GetByIdAsync(int id);
		Task<int> CreateAsync(Estudiante e);
		Task<int> UpdateAsync(Estudiante e);
		Task<int> DeleteAsync(int id);
	}
}
 