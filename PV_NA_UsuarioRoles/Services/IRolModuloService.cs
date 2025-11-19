using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public interface IRolModuloService
    {
        Task<IEnumerable<RolModulo>> GetByRolAsync(int idRol);
        Task GuardarAsync(int idRol, List<RolModulo> permisos);
    }
}

