using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Repository;
using System.Text;
using System.Text.Json;

namespace PV_NA_UsuariosRoles.Services
{
    public class ParametroService : IParametroService
    {
        private readonly ParametroRepository _repo;
        private readonly HttpClient _httpClient;

        public ParametroService(ParametroRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient();
        }

        /* =====================
           VALIDACIONES
           ===================== */
        private static void ValidarParametro(Parametro parametro)
        {
            if (parametro == null)
                throw new Exception("El parámetro no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(parametro.ID_Parametro))
                throw new Exception("El campo 'ID_Parametro' es obligatorio.");

            if (string.IsNullOrWhiteSpace(parametro.Valor))
                throw new Exception("El campo 'Valor' es obligatorio.");

            // ID debe ser letras mayúsculas, máximo 10 caracteres
            if (parametro.ID_Parametro.Length > 10)
                throw new Exception("El identificador del parámetro no puede exceder los 10 caracteres.");

            if (!parametro.ID_Parametro.All(c => c >= 'A' && c <= 'Z'))
                throw new Exception("El identificador del parámetro solo puede contener letras mayúsculas (A-Z).");

            if (parametro.Valor.Length > 500)
                throw new Exception("El valor del parámetro no puede exceder los 500 caracteres.");
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
           OPERACIONES CRUD
           ===================== */
        public async Task<IEnumerable<Parametro>> GetAllAsync(int idUsuarioAccion)
        {
            var data = await _repo.GetAllAsync();
            await RegistrarBitacora(idUsuarioAccion, "El usuario consulta parámetros");
            return data;
        }

        public async Task<Parametro?> GetByIdAsync(string id, int idUsuarioAccion)
        {
            var parametro = await _repo.GetByIdAsync(id);
            await RegistrarBitacora(idUsuarioAccion, $"El usuario consulta parámetro ID={id}");
            return parametro;
        }

        public async Task CrearAsync(Parametro parametro, int idUsuarioAccion)
        {
            ValidarParametro(parametro);

            if (await _repo.ExistsAsync(parametro.ID_Parametro))
                throw new Exception("Ya existe un parámetro con ese identificador.");

            await _repo.CreateAsync(parametro);
            await RegistrarBitacora(idUsuarioAccion, "Creación de parámetro", parametro);
        }

        public async Task ActualizarAsync(Parametro parametro, int idUsuarioAccion)
        {
            ValidarParametro(parametro);

            var antes = await _repo.GetByIdAsync(parametro.ID_Parametro);
            if (antes is null)
                throw new Exception("No se encontró el parámetro a actualizar.");

            await _repo.UpdateAsync(parametro);
            var despues = await _repo.GetByIdAsync(parametro.ID_Parametro);
            await RegistrarBitacora(idUsuarioAccion, "Actualización de parámetro", new { antes, despues });
        }

        public async Task EliminarAsync(string id, int idUsuarioAccion)
        {
            var eliminado = await _repo.GetByIdAsync(id);
            if (eliminado is null)
                throw new Exception("El parámetro no existe o ya fue eliminado.");

            await _repo.DeleteAsync(id);
            await RegistrarBitacora(idUsuarioAccion, "Eliminación de parámetro", eliminado);
        }
    }
}

