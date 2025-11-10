using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Services
{ 
    /*
    public interface IProfesorService
    {
        Task<IEnumerable<Profesor>> GetAllAsync();
        Task<Profesor?> GetByIdAsync(int id);
        Task<int> CreateAsync(Profesor profesor);
        Task<int> UpdateAsync(Profesor profesor);
        Task<int> DeleteAsync(int id);
    / */

    public interface IProfesorService
    {
        Task<IEnumerable<Profesor>> GetAllAsync(string usuario);
        Task<Profesor?> GetByIdAsync(int id, string usuario);
        Task<int> CreateAsync(Profesor profesor, string usuario);
        Task<int> UpdateAsync(Profesor profesor, string usuario);
        Task<int> DeleteAsync(int id, string usuario);
    }
}
