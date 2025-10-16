using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public interface IMatriculaRepository
	{
		Task<int> InsertAsync(Matricula m);
		Task<int> UpdateAsync(Matricula m);
		Task<int> DeleteAsync(int id);
		Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo);
	}
}
 