using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
    public interface IPreMatriculaService
    {
        Task<IEnumerable<PreMatricula>> GetAllAsync(int idUsuario);
        Task<PreMatricula?> GetByIdAsync(int id, int idUsuario);
        Task<int> CreateAsync(PreMatricula pre, int idUsuario);
        Task<int> UpdateAsync(PreMatricula pre, int idUsuario);
        Task<int> DeleteAsync(int id, int idUsuario);
    }
}
