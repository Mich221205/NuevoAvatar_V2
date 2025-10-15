using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public interface IModuloService
    {
        Task<IEnumerable<Modulo>> GetAllAsync(int idUsuarioAccion);
        Task<Modulo?> GetByIdAsync(int id, int idUsuarioAccion);
        Task CrearAsync(Modulo modulo, int idUsuarioAccion);
        Task ActualizarAsync(Modulo modulo, int idUsuarioAccion);
        Task EliminarAsync(int id, int idUsuarioAccion);
    }
}

