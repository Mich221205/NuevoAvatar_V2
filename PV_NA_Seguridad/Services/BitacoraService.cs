using PV_NA_Seguridad.Entities;
using PV_NA_Seguridad.Repository;

namespace PV_NA_Seguridad.Services
{
    public class BitacoraService
    {
        private readonly BitacoraRepository _repository;

        public BitacoraService(BitacoraRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> RegistrarAsync(Bitacora bitacora)
        {
            if (bitacora.ID_Usuario <= 0)
                throw new ArgumentException("El ID del usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(bitacora.Accion))
                throw new ArgumentException("La acción no puede estar vacía.");

            await _repository.InsertAsync(bitacora);
            return true;
        }

        public Task<IEnumerable<Bitacora>> ListarAsync() => _repository.GetAllAsync();
        public Task<IEnumerable<Bitacora>> ListarPorUsuarioAsync(int idUsuario) => _repository.GetByUsuarioAsync(idUsuario);
    }
}
