using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
    public interface IDireccionService
    {
        Task<IEnumerable<Provincia>> GetProvinciasAsync(int idUsuario);
        Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia, int idUsuario);
        Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton, int idUsuario);
    }
}