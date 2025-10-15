using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Repository;
using System.Text;
using System.Text.Json;

namespace PV_NA_UsuariosRoles.Services
{
    public class ModuloService : IModuloService
    {
        private readonly ModuloRepository _repo;
        private readonly HttpClient _httpClient;

        public ModuloService(ModuloRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient();
        }

        /* =====================
           VALIDACIONES
           ===================== */
        private static void ValidarModulo(Modulo modulo)
        {
            if (modulo == null)
                throw new Exception("El módulo no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(modulo.Nombre))
                throw new Exception("El campo 'Nombre' es obligatorio.");

            // Validar solo letras y espacios
            foreach (char c in modulo.Nombre)
            {
                if (!(char.IsLetter(c) || char.IsWhiteSpace(c)))
                    throw new Exception("El nombre del módulo solo puede contener letras y espacios.");
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
                await _httpClient.PostAsync("http://localhost:5210/bitacora", content);
            }
            catch
            {
                Console.WriteLine("No se pudo registrar la bitácora (servicio no disponible).");
            }
        }

        /* =====================
           CRUD OPERATIONS
           ===================== */
        public async Task<IEnumerable<Modulo>> GetAllAsync(int idUsuarioAccion)
        {
            var data = await _repo.GetAllAsync();
            await RegistrarBitacora(idUsuarioAccion, "El usuario consulta módulos");
            return data;
        }

        public async Task<Modulo?> GetByIdAsync(int id, int idUsuarioAccion)
        {
            var modulo = await _repo.GetByIdAsync(id);
            await RegistrarBitacora(idUsuarioAccion, $"El usuario consulta módulo ID={id}");
            return modulo;
        }

        public async Task CrearAsync(Modulo modulo, int idUsuarioAccion)
        {
            ValidarModulo(modulo);

            if (await _repo.ExistsByNameAsync(modulo.Nombre))
                throw new Exception("Ya existe un módulo con ese nombre.");

            int nuevoId = await _repo.CreateAsync(modulo);
            modulo.ID_Modulo = nuevoId;

            await RegistrarBitacora(idUsuarioAccion, "Creación de módulo", modulo);
        }

        public async Task ActualizarAsync(Modulo modulo, int idUsuarioAccion)
        {
            ValidarModulo(modulo);

            var antes = await _repo.GetByIdAsync(modulo.ID_Modulo);
            if (antes is null)
                throw new Exception("No se encontró el módulo a actualizar.");

            if (await _repo.ExistsByNameAsync(modulo.Nombre, excludeId: modulo.ID_Modulo))
                throw new Exception("Ya existe otro módulo con ese nombre.");

            await _repo.UpdateAsync(modulo);
            var despues = await _repo.GetByIdAsync(modulo.ID_Modulo);
            await RegistrarBitacora(idUsuarioAccion, "Actualización de módulo", new { antes, despues });
        }

        public async Task EliminarAsync(int id, int idUsuarioAccion)
        {
            var eliminado = await _repo.GetByIdAsync(id);
            if (eliminado is null)
                throw new Exception("El módulo no existe o ya fue eliminado.");

            await _repo.DeleteAsync(id);
            await RegistrarBitacora(idUsuarioAccion, "Eliminación de módulo", eliminado);
        }
    }
}

