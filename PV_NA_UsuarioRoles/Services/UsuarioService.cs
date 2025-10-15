using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Helpers;
using PV_NA_UsuariosRoles.Repository;
using System.Text;
using System.Text.Json;

namespace PV_NA_UsuariosRoles.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepository _repo;
        private readonly HttpClient _httpClient;

        public UsuarioService(UsuarioRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient();
        }

        /* =====================
           VALIDACIONES
           ===================== */
        private static void ValidarUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new Exception("El usuario no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El campo 'Nombre' no puede estar vacío o solo contener espacios.");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new Exception("El campo 'Email' es obligatorio.");

            if (string.IsNullOrWhiteSpace(usuario.Identificacion))
                throw new Exception("El campo 'Identificación' es obligatorio.");
        }

        private static void ValidarDominio(Usuario usuario)
        {
            if (usuario.Email.EndsWith("@cuc.cr"))
                usuario.ID_Rol = 1; // Estudiante
            else if (usuario.Email.EndsWith("@cuc.ac.cr"))
                usuario.ID_Rol = 2; // Profesor
            else
                throw new Exception("El email debe pertenecer a los dominios cuc.cr o cuc.ac.cr");
        }

        /* =====================
           OPERACIONES CRUD
           ===================== */

        public async Task<IEnumerable<Usuario>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Usuario?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Usuario>> FilterAsync(string? identificacion, string? nombre, string? tipo)
            => await _repo.FilterAsync(identificacion, nombre, tipo);

        public async Task CrearAsync(Usuario usuario, int idUsuarioAccion)
        {
            ValidarUsuario(usuario);
            ValidarDominio(usuario);

            usuario.Contrasena = AesEncryption.Encrypt(usuario.Contrasena);
            int nuevoId = await _repo.CreateAsync(usuario);
            usuario.ID_Usuario = nuevoId;

            var detalle = JsonSerializer.Serialize(usuario, new JsonSerializerOptions { WriteIndented = true });
            await RegistrarBitacora(idUsuarioAccion, detalle);
        }

        public async Task ActualizarAsync(Usuario usuario, int idUsuarioAccion)
        {
            ValidarUsuario(usuario);

            var antes = await _repo.GetByIdAsync(usuario.ID_Usuario);
            if (antes == null)
                throw new Exception("No se encontró el usuario a actualizar.");

            usuario.Contrasena = AesEncryption.Encrypt(usuario.Contrasena);
            await _repo.UpdateAsync(usuario);

            var despues = await _repo.GetByIdAsync(usuario.ID_Usuario);
            var detalle = JsonSerializer.Serialize(new { antes, despues }, new JsonSerializerOptions { WriteIndented = true });
            await RegistrarBitacora(idUsuarioAccion, detalle);
        }

        public async Task EliminarAsync(int id, int idUsuarioAccion)
        {
            var eliminado = await _repo.GetByIdAsync(id);
            if (eliminado == null)
                throw new Exception("El usuario no existe o ya fue eliminado.");

            await _repo.DeleteSessionsByUserIdAsync(id);
            await _repo.DeleteAsync(id);

            var detalle = JsonSerializer.Serialize(eliminado, new JsonSerializerOptions { WriteIndented = true });
            await RegistrarBitacora(idUsuarioAccion, detalle);
        }

        /* =====================
           BITÁCORA
           ===================== */
        private async Task RegistrarBitacora(int idUsuario, string accion)
        {
            try
            {
                var data = new { ID_Usuario = idUsuario, Accion = accion };
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("http://localhost:5210/bitacora", content);
            }
            catch
            {
                Console.WriteLine("No se pudo registrar la bitácora (servicio no disponible).");
            }
        }
    }
}
