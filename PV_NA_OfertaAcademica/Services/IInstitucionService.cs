using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Services
{
    public interface IInstitucionService
    {
        Task<IEnumerable<Institucion>> GetAll(string usuario);
        Task<Institucion?> GetById(int id, string usuario);
        Task<bool> Create(Institucion inst, string usuario);
        Task<bool> Update(Institucion inst, string usuario);
        Task<bool> Delete(int id, string usuario);
    }
}
