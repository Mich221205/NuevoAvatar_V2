using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
	public interface IMatriculaService
	{
		Task<int> CreateAsync(Matricula m);
		Task<int> UpdateAsync(Matricula m);
		Task<int> DeleteAsync(int id);
		Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo);
	}
}
 