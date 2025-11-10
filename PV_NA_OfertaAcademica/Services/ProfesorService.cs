using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Repository;
using System.Net.Http.Json;
using System.Text.Json;

namespace PV_NA_OfertaAcademica.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly IProfesorRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public ProfesorService(IProfesorRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Profesor>> GetAllAsync(string usuario)
        {
            var lista = await _repo.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de profesores");
            return lista;
        }

        public async Task<Profesor?> GetByIdAsync(int id, string usuario)
        {
            var profesor = await _repo.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó profesor con ID {id}");
            return profesor;
        }

        public async Task<int> CreateAsync(Profesor profesor, string usuario)
        {
            if (string.IsNullOrWhiteSpace(profesor.Identificacion))
                throw new Exception("La identificación es obligatoria.");
            if (string.IsNullOrWhiteSpace(profesor.Tipo_Identificacion))
                throw new Exception("El tipo de identificación es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesor.Email))
                throw new Exception("El correo electrónico es obligatorio.");
            if (string.IsNullOrWhiteSpace(profesor.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (!profesor.Email.EndsWith("@cuc.ac.cr", StringComparison.OrdinalIgnoreCase))
                throw new Exception("El correo debe ser institucional (@cuc.ac.cr).");

            if (await _repo.ExisteIdentificacionAsync(profesor.Identificacion))
                throw new Exception("Ya existe un profesor con esa identificación.");
            if (await _repo.ExisteEmailAsync(profesor.Email))
                throw new Exception("Ya existe un profesor con ese correo.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(profesor.Nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$"))
                throw new Exception("El nombre solo puede contener letras y espacios.");

            var edad = DateTime.Today.Year - profesor.Fecha_Nacimiento.Year;
            if (profesor.Fecha_Nacimiento > DateTime.Today.AddYears(-edad)) edad--;
            if (edad < 18)
                throw new Exception("El profesor debe ser mayor de edad (mínimo 18 años).");

            var id = await _repo.InsertAsync(profesor);
            await RegistrarBitacoraAsync(usuario, $"Creó profesor: {JsonSerializer.Serialize(profesor)}");
            return id;
        }

        public async Task<int> UpdateAsync(Profesor profesor, string usuario)
        {
            if (profesor.ID_Profesor <= 0)
                throw new Exception("Debe especificar un ID de profesor válido.");

            var anterior = await _repo.GetByIdAsync(profesor.ID_Profesor);
            var result = await _repo.UpdateAsync(profesor);

            if (result > 0)
            {
                var descripcion = $"Actualizó profesor: Antes={JsonSerializer.Serialize(anterior)}, Ahora={JsonSerializer.Serialize(profesor)}";
                await RegistrarBitacoraAsync(usuario, descripcion);
            }

            return result;
        }

        public async Task<int> DeleteAsync(int id, string usuario)
        {
            if (id <= 0)
                throw new Exception("Debe especificar un ID de profesor válido.");

            var profesor = await _repo.GetByIdAsync(id);
            var result = await _repo.DeleteAsync(id);

            if (result > 0)
                await RegistrarBitacoraAsync(usuario, $"Eliminó profesor: {JsonSerializer.Serialize(profesor)}");

            return result;
        }

        // 🔹 Método auxiliar para registrar bitácoras (GEN1)
        private async Task RegistrarBitacoraAsync(string idUsuario, string accion)
        {
            try
            {
                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}
