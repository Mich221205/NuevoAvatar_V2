using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Repository;

namespace PV_NA_UsuariosRoles.Services
{
    public class RolModuloService : IRolModuloService
    {
        private readonly RolModuloRepository _repo;

        public RolModuloService(RolModuloRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<RolModulo>> GetByRolAsync(int idRol)
            => await _repo.GetByRolAsync(idRol);

        public async Task GuardarAsync(int idRol, List<RolModulo> permisos)
            => await _repo.GuardarAsync(idRol, permisos);
    }
}
