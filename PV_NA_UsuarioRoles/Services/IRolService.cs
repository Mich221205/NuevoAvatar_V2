using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync(int idUsuarioAccion);
        Task<Rol?> GetByIdAsync(int id, int idUsuarioAccion);
        Task CrearAsync(Rol rol, int idUsuarioAccion);
        Task ActualizarAsync(Rol rol, int idUsuarioAccion);
        Task EliminarAsync(int id, int idUsuarioAccion);
    }
}

