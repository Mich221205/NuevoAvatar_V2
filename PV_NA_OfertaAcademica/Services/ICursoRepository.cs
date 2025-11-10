using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Services
{
    public interface ICursoRepository
    {
        // CRUD básico y consultas específicas.
        Task<int> CreateAsync(Curso e);
        Task<int> UpdateAsync(Curso e);
        Task<int> DeleteAsync(int id);
        Task<Curso?> GetByIdAsync(int id);
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<IEnumerable<Curso>> GetByCarreraAsync(int idCarrera);

        // Validaciones específicas.
        Task<bool> CarreraExistsAsync(int idCarrera);
        Task<bool> ExistsByNombreAsync(int idCarrera, string nombre, int? excludeId = null);
        Task<bool> HasGruposAsync(int idCurso);
    }
}
