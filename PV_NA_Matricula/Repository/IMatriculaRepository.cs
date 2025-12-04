using PV_NA_Matricula.Dtos;
using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public interface IMatriculaRepository
	{
		Task<int> InsertAsync(Matricula m);
		Task<int> UpdateAsync(Matricula m);
		Task<int> DeleteAsync(int id);
		Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo);
		Task<Matricula?> GetByIdAsync(int id);
		Task<bool> ExisteDuplicadoAsync(int idEstudiante, int idCurso, int idGrupo, int? excluirId = null);

        Task<IEnumerable<Adm19ListadoRowDto>> ListadoAdm19Async(
            int? idPeriodo,
            int? idCarrera,
            int? idCurso,
            int? idGrupo);
    }
}
 