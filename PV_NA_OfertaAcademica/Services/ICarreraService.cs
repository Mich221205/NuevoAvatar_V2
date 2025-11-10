using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Services
{
    // Interface carrera service
    public interface ICarreraService
    { /*
        Task<IEnumerable<Carrera>> GetAll();
        Task<Carrera?> GetById(int id);
        Task<IEnumerable<Carrera>> GetByInstitucion(int idInstitucion);
        Task<bool> Create(Carrera carrera);
        Task<bool> Update(Carrera carrera);
        Task<bool> Delete(int id);
        */
        Task<IEnumerable<Carrera>> GetAll(string usuario);
        Task<Carrera?> GetById(int id, string usuario);
        Task<IEnumerable<Carrera>> GetByInstitucion(int idInstitucion, string usuario);
        Task<bool> Create(Carrera carrera, string usuario);
        Task<bool> Update(Carrera carrera, string usuario);
        Task<bool> Delete(int id, string usuario);


    }

}
