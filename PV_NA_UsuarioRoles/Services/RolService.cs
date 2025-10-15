using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Repository;
using System.Text;
using System.Text.Json;

namespace PV_NA_UsuariosRoles.Services
{
    public class RolService : IRolService
    {
        private readonly RolRepository _repo;
        private readonly HttpClient _httpClient;

        public RolService(RolRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient();
        }

        /* =====================
           VALIDACIONES
           ===================== */
        private static void ValidarRol(Rol rol)
        {
            if (rol == null) throw new Exception("El rol no puede ser nulo.");

            rol.Nombre = rol.Nombre?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rol.Nombre))
                throw new Exception("El nombre del rol es obligatorio y no puede estar vacío.");

            // Solo letras y espacios (mismo criterio que el CHECK de la BD)
            foreach (char c in rol.Nombre)
            {
                if (!(char.IsLetter(c) || char.IsWhiteSpace(c)))
                    throw new Exception("El nombre del rol solo puede contener letras y espacios.");
            }
        }

        /* =====================
           BITÁCORA
           ===================== */
        private async Task RegistrarBitacora(int idUsuario, string accion, object? data = null)
        {
            try
            {
                string jsonDetalle = data == null
                    ? accion
                    : $"{accion} -> " + JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

                var body = new { ID_Usuario = idUsuario, Accion = jsonDetalle };
                var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                // Ajusta el puerto si tu GEN1 corre en otro lugar:
                await _httpClient.PostAsync("http://localhost:5210/bitacora", content);
            }
            catch
            {
                Console.WriteLine("No se pudo registrar la bitácora (servicio no disponible).");
            }
        }

        /* =====================
           OPERACIONES
           ===================== */
        public async Task<IEnumerable<Rol>> GetAllAsync(int idUsuarioAccion)
        {
            var data = await _repo.GetAllAsync();
            await RegistrarBitacora(idUsuarioAccion, "El usuario consulta roles");
            return data;
        }

        public async Task<Rol?> GetByIdAsync(int id, int idUsuarioAccion)
        {
            var rol = await _repo.GetByIdAsync(id);
            await RegistrarBitacora(idUsuarioAccion, $"El usuario consulta rol por ID={id}");
            return rol;
        }

        public async Task CrearAsync(Rol rol, int idUsuarioAccion)
        {
            ValidarRol(rol);

            if (await _repo.ExistsByNameAsync(rol.Nombre))
                throw new Exception("Ya existe un rol con ese nombre.");

            int nuevoId = await _repo.CreateAsync(rol);
            rol.ID_Rol = nuevoId;

            await RegistrarBitacora(idUsuarioAccion, "Creación de rol", rol);
        }

        public async Task ActualizarAsync(Rol rol, int idUsuarioAccion)
        {
            ValidarRol(rol);

            var antes = await _repo.GetByIdAsync(rol.ID_Rol);
            if (antes is null)
                throw new Exception("No se encontró el rol a actualizar.");

            if (await _repo.ExistsByNameAsync(rol.Nombre, excludeId: rol.ID_Rol))
                throw new Exception("Ya existe otro rol con ese nombre.");

            await _repo.UpdateAsync(rol);
            var despues = await _repo.GetByIdAsync(rol.ID_Rol);
            await RegistrarBitacora(idUsuarioAccion, "Actualización de rol", new { antes, despues });
        }

        public async Task EliminarAsync(int id, int idUsuarioAccion)
        {
            var eliminado = await _repo.GetByIdAsync(id);
            if (eliminado is null)
                throw new Exception("El rol no existe o ya fue eliminado.");

            // Protección por FK: no permitir eliminar si hay usuarios asociados
            if (await _repo.IsRolInUseAsync(id))
                throw new Exception("No es posible eliminar el rol: existen usuarios asociados.");

            await _repo.DeleteAsync(id);
            await RegistrarBitacora(idUsuarioAccion, "Eliminación de rol", eliminado);
        }
    }
}
