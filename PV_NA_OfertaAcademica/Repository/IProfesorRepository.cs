using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Repository
{
    public interface IProfesorRepository
    {
        Task<IEnumerable<Profesor>> GetAllAsync();
        Task<Profesor?> GetByIdAsync(int id);
        Task<int> InsertAsync(Profesor profesor);
        Task<int> UpdateAsync(Profesor profesor);
        Task<int> DeleteAsync(int id);
        Task<bool> ExisteIdentificacionAsync(string identificacion);
        Task<bool> ExisteEmailAsync(string email);
        Task<bool> TieneGruposActivosAsync(int idProfesor);

        Task<bool> EsDirectorDeAlgunaCarreraAsync(int idProfesor);
    }
}
