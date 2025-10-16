using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;

	namespace PV_NA_Matricula.Services
	{
		public class PreMatriculaService : IPreMatriculaService
		{
			private readonly IPreMatriculaRepository _repo;

			public PreMatriculaService(IPreMatriculaRepository repo)
			{
				_repo = repo;
			}

			public async Task<IEnumerable<PreMatricula>> GetAllAsync() => await _repo.GetAllAsync();

			public async Task<PreMatricula?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

			public async Task<int> CreateAsync(PreMatricula pre)
			{
				// Aquí luego puedes agregar la validación del periodo futuro
				return await _repo.InsertAsync(pre);
			}

			public async Task<int> UpdateAsync(PreMatricula pre) => await _repo.UpdateAsync(pre);

			public async Task<int> DeleteAsync(int id) => await _repo.DeleteAsync(id);
		}
	}

 