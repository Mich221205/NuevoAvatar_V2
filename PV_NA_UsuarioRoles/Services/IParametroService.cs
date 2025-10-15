using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public interface IParametroService
    {
        Task<IEnumerable<Parametro>> GetAllAsync(int idUsuarioAccion);
        Task<Parametro?> GetByIdAsync(string id, int idUsuarioAccion);
        Task CrearAsync(Parametro parametro, int idUsuarioAccion);
        Task ActualizarAsync(Parametro parametro, int idUsuarioAccion);
        Task EliminarAsync(string id, int idUsuarioAccion);
    }
}

