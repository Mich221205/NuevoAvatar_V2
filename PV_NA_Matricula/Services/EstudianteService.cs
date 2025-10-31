using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Text.RegularExpressions;
using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public EstudianteService(IEstudianteRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Estudiante>> GetAllAsync(int idUsuario)
        {
            var data = await _repo.GetAllAsync();

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó el listado de estudiantes."
            );

            return data;
        }

        public async Task<Estudiante?> GetByIdAsync(int id, int idUsuario)
        {
            var est = await _repo.GetByIdAsync(id);

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó el estudiante con ID {id}."
            );

            return est;
        }


        public async Task<int> CreateAsync(Estudiante e, int idUsuario)
        {
            ValidarEstudiante(e);
            int id = await _repo.InsertAsync(e);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} creó un estudiante con ID {id}, nombre {e.Nombre}.");

            return id;
        }

        public async Task<int> UpdateAsync(Estudiante e, int idUsuario)
        {
            ValidarEstudiante(e);
            int result = await _repo.UpdateAsync(e);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} actualizó el estudiante con ID {e.ID_Estudiante}, nombre {e.Nombre}.");

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} eliminó el estudiante con ID {id}.");

            return result;
        }

        //  Método para registrar acciones en la bitácora
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
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

        //  Validaciones de la HU MAT3
        private void ValidarEstudiante(Estudiante e)
        {
            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(e.Identificacion) ||
                string.IsNullOrWhiteSpace(e.Tipo_Identificacion) ||
                string.IsNullOrWhiteSpace(e.Email) ||
                string.IsNullOrWhiteSpace(e.Nombre) ||
                string.IsNullOrWhiteSpace(e.Direccion) ||
                string.IsNullOrWhiteSpace(e.Telefono))
            {
                throw new Exception("Todos los campos son requeridos.");
            }

            // Validar formato del nombre
            var regexNombre = new Regex(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$");
            if (!regexNombre.IsMatch(e.Nombre))
                throw new Exception("El nombre solo puede contener letras y espacios.");

            // Validar formato del correo
            if (!Regex.IsMatch(e.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del correo electrónico no es válido.");

            // Validar dominio del correo
            if (!e.Email.EndsWith("@cuc.cr", StringComparison.OrdinalIgnoreCase))
                throw new Exception("El correo debe pertenecer al dominio @cuc.cr.");

            // Validar mayoría de edad
            int edad = DateTime.Now.Year - e.Fecha_Nacimiento.Year;
            if (e.Fecha_Nacimiento > DateTime.Now.AddYears(-edad))
                edad--;
            if (edad < 18)
                throw new Exception("El estudiante debe ser mayor de edad.");
        }
    }
}
