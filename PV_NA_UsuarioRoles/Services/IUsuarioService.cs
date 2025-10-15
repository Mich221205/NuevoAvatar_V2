using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<IEnumerable<Usuario>> FilterAsync(string? identificacion, string? nombre, string? tipo);
        Task CrearAsync(Usuario usuario, int idUsuarioAccion);
        Task ActualizarAsync(Usuario usuario, int idUsuarioAccion);
        Task EliminarAsync(int id, int idUsuarioAccion);
    }
}
