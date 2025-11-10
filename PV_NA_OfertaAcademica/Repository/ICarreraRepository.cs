using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Repository
{
    // Interface carrera repository
    public interface ICarreraRepository
    {
        Task<IEnumerable<Carrera>> GetAllAsync();
        Task<Carrera?> GetByIdAsync(int id);
        Task<IEnumerable<Carrera>> GetByInstitucionAsync(int idInstitucion);
        Task<int> CreateAsync(Carrera carrera);
        Task<int> UpdateAsync(Carrera carrera);
        Task<int> DeleteAsync(int id);
    }
}
