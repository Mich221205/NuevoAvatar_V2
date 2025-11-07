using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PV_NA_Matricula.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        //opciones de serialización
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public MatriculaService(IMatriculaRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<int> CreateAsync(Matricula m, int idUsuario)
        {
            int id = await _repo.InsertAsync(m);

            // Aseguramos que el body tenga el ID generado
            m.ID_Matricula = id;

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} creó una matrícula con ID {id} para el estudiante {m.ID_Estudiante}, curso {m.ID_Curso}, grupo {m.ID_Grupo}.",
                body: m
            );

            return id;
        }

        public async Task<int> UpdateAsync(Matricula m, int idUsuario)
        {
            int result = await _repo.UpdateAsync(m);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} actualizó la matrícula con ID {m.ID_Matricula}.",
                body: m
            );

            return result;
        }

        //Overload que permite pasar el body del eliminado
        public async Task<int> DeleteAsync(int id, int idUsuario, object? body)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la matrícula con ID {id}.",
                body: body ?? new { ID_Matricula = id }
            );

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la matrícula con ID {id}."
            );

            return result;
        }

        public async Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo, int idUsuario)
        {
            var data = await _repo.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo);

            // Registrar consulta en bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó estudiantes del curso {idCurso} y grupo {idGrupo}.",
                body: new { idCurso, idGrupo, resultado = data }
            );

            return data;
        }

        public async Task<Matricula?> GetByIdAsync(int id, int idUsuario)
        {
            var entity = await _repo.GetByIdAsync(id);

            // Registrar lectura en bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó la matrícula con ID {id}.",
                body: (object?)entity ?? new { ID_Matricula = id, encontrado = false } // <-- FIX
            );

            return entity;
        }

        // ======================================================
        //  Método para registrar acciones en la bitácora
        // ======================================================
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, body: null);
        }

        // ======================================================
        //  Registrar en Bitácora con body opcional (como JSON)
        // ======================================================
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion, object? body)
        {
            try
            {
                string? bodyJson = null;

                if (body is not null)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(
                        body,
                        _jsonOpts
                    );

                    // limita el tamaño para no romper tu columna de Accion
                    const int max = 8000;
                    bodyJson = json.Length > max ? json.Substring(0, max) + "...(truncado)" : json;
                }

                // Empaquetamos el body dentro del mismo campo Accion
                var accionConBody = bodyJson is null ? accion : $"{accion} | Body: {bodyJson}";

                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accionConBody
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}

