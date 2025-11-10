using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Repository
{
    public interface IInstitucionRepository
    {  
        Task<IEnumerable<Institucion>> GetAllAsync();
        Task<Institucion?> GetByIdAsync(int id);
        Task<int> CreateAsync(Institucion institucion);
        Task<bool> UpdateAsync(Institucion institucion);
        Task<bool> DeleteAsync(int id);

    }
}
