using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Text.RegularExpressions;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PV_NA_Matricula.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _repo;
        private readonly HttpClient _bitacoraClient;

        //opciones de serialización compacta
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public EstudianteService(IEstudianteRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Estudiante>> GetAllAsync(int idUsuario)
        {
            var data = await _repo.GetAllAsync();

            // Incluir body dentro de Accion
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó el listado de estudiantes.",
                body: new { resultado = data }
            );

            return data;
        }

        public async Task<Estudiante?> GetByIdAsync(int id, int idUsuario)
        {
            var est = await _repo.GetByIdAsync(id);

            // Incluir body del encontrado
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó el estudiante con ID {id}.",
                body: (object?)est ?? new { ID_Estudiante = id, encontrado = false }
            );

            return est;
        }

        public async Task<int> CreateAsync(Estudiante e, int idUsuario)
        {
            ValidarEstudiante(e);
            int id = await _repo.InsertAsync(e);

            // reflejamos el ID generado en el body
            e.ID_Estudiante = id;

            // Registrar en Bitácora con body
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} creó un estudiante con ID {id}, nombre {e.Nombre}.",
                body: e
            );

            return id;
        }

        public async Task<int> UpdateAsync(Estudiante e, int idUsuario)
        {
            ValidarEstudiante(e);
            int result = await _repo.UpdateAsync(e);

            // Registrar en Bitácora con body
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} actualizó el estudiante con ID {e.ID_Estudiante}, nombre {e.Nombre}.",
                body: e
            );

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} eliminó el estudiante con ID {id}.");

            return result;
        }

        //Overload para pasar el objeto eliminado y guardarlo como body en bitácora
        public async Task<int> DeleteAsync(int id, int idUsuario, object? body)
        {
            int result = await _repo.DeleteAsync(id);

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó el estudiante con ID {id}.",
                body: body ?? new { ID_Estudiante = id }
            );

            return result;
        }

        // ======================================================
        //  Bitácora
        //  Versión original 
        // ======================================================
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, body: null);
        }

        // Bitácora con body incrustado en Accion
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion, object? body)
        {
            try
            {
                string accionConBody = accion;

                if (body is not null)
                {
                    var json = JsonSerializer.Serialize(body, _jsonOpts);

                    //limita para no exceder el tamaño de la columna Accion
                    const int max = 8000;
                    var bodyJson = json.Length > max ? json.Substring(0, max) + "...(truncado)" : json;

                    accionConBody = $"{accion} | Body: {bodyJson}";
                }

                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accionConBody
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