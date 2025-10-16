using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;

namespace PV_NA_Matricula.Services
{
	public class MatriculaService : IMatriculaService
	{
		private readonly IMatriculaRepository _repo;

		public MatriculaService(IMatriculaRepository repo)
		{
			_repo = repo;
		}

		public async Task<int> CreateAsync(Matricula m)
		{
			// Puedes agregar más adelante la validación de periodo activo.
			return await _repo.InsertAsync(m);
		}

		public async Task<int> UpdateAsync(Matricula m)
		{
			return await _repo.UpdateAsync(m);
		}

		public async Task<int> DeleteAsync(int id)
		{
			return await _repo.DeleteAsync(id);
		}
		 
		public async Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo)
		{
			return await _repo.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo);
		}
	}
}

