using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using Microsoft.AspNetCore.Http;

namespace PV_NA_Matricula.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        //cliente hacia Oferta Académica para validar periodos
        private readonly HttpClient _ofertaClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //opciones de serialización
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        // DTO local que mapea /periodo/{id} de Oferta
        private sealed class PeriodoInfo
        {
            public int ID_Periodo { get; set; }
            public int Anno { get; set; }
            public int Numero { get; set; }
            public DateTime Fecha_Inicio { get; set; }
            public DateTime Fecha_Fin { get; set; }
        }

        // reglas de validación (lo dejamos aunque no lo usemos ahora)
        private enum ReglaPeriodo { SoloFuturos, SoloActivos }

        public MatriculaService(IMatriculaRepository repo, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
            _ofertaClient = httpClientFactory.CreateClient("OfertaClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreateAsync(Matricula m, int idUsuario)
        {
            // ✅ NUEVO: Validar que no exista ya la misma combinación estudiante–curso–grupo–periodo
            if (await _repo.ExistsAsync(m.ID_Estudiante, m.ID_Curso, m.ID_Grupo, m.ID_Periodo))
            {
                throw new Exception("Ya existe una matrícula para ese estudiante en ese curso, grupo y período.");
            }

            // validar periodo ACTIVO (fecha de inicio posterior a la actual)
            await ValidarPeriodoActivoAsync(m.ID_Periodo);

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
            // ⬇️ CAMBIO: validar periodo ACTIVO (fecha de inicio posterior a la actual)
            await ValidarPeriodoActivoAsync(m.ID_Periodo);

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
                body: (object?)entity ?? new { ID_Matricula = id, encontrado = false }
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

        // ======================================================
        // Validación específica de periodo ACTIVO (inicio > ahora)
        // ======================================================
        private async Task ValidarPeriodoActivoAsync(int idPeriodo)
        {
            // Reenvía el Authorization si viene en la solicitud
            var request = new HttpRequestMessage(HttpMethod.Get, $"/periodo/{idPeriodo}");
            var auth = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(auth))
                request.Headers.TryAddWithoutValidation("Authorization", auth);

            var resp = await _ofertaClient.SendAsync(request);

            if (!resp.IsSuccessStatusCode)
            {
                var detalle = await resp.Content.ReadAsStringAsync();
                throw new Exception($"No fue posible validar el periodo {idPeriodo} en Oferta Académica. " +
                                    $"Status: {(int)resp.StatusCode} {resp.StatusCode}. Respuesta: {detalle}");
            }

            var periodo = await resp.Content.ReadFromJsonAsync<PeriodoInfo>()
                          ?? throw new Exception("Respuesta de periodo inválida.");

            var ahora = DateTime.Now; // usa UtcNow si tus fechas están en UTC
            if (!(periodo.Fecha_Inicio > ahora))
                throw new Exception("Solo se pueden matricular periodos ACTIVOS (fecha de inicio posterior a la actual).");
        }
    }
}


