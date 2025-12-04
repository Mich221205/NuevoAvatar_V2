using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using PV_NA_Matricula.Dtos;
using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;

namespace PV_NA_Matricula.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        private readonly HttpClient _ofertaClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        private sealed class PeriodoInfo
        {
            public int ID_Periodo { get; set; }
            public int Anno { get; set; }
            public int Numero { get; set; }
            public DateTime Fecha_Inicio { get; set; }
            public DateTime Fecha_Fin { get; set; }
        }

        private enum ReglaPeriodo { SoloFuturos, SoloActivos }

        public MatriculaService(
            IMatriculaRepository repo,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
            _ofertaClient = httpClientFactory.CreateClient("OfertaClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreateAsync(Matricula m, int idUsuario)
        {
            var existeDuplicado = await _repo.ExisteDuplicadoAsync(
                m.ID_Estudiante,
                m.ID_Curso,
                m.ID_Grupo,
                excluirId: null 
            );

            if (existeDuplicado)
                throw new Exception("Ya existe una matrícula para este estudiante, curso y grupo.");

            await ValidarPeriodoActivoAsync(m.ID_Periodo);

            int id = await _repo.InsertAsync(m);

            m.ID_Matricula = id;

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} creó una matrícula con ID {id} para el estudiante {m.ID_Estudiante}, curso {m.ID_Curso}, grupo {m.ID_Grupo}.",
                body: m
            );

            return id;
        }

        public async Task<int> UpdateAsync(Matricula m, int idUsuario)
        {
            var existeDuplicado = await _repo.ExisteDuplicadoAsync(
                m.ID_Estudiante,
                m.ID_Curso,
                m.ID_Grupo,
                excluirId: m.ID_Matricula 
            );

            if (existeDuplicado)
                throw new Exception("Ya existe una matrícula para este estudiante, curso y grupo.");

            await ValidarPeriodoActivoAsync(m.ID_Periodo);

            int result = await _repo.UpdateAsync(m);

            
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} actualizó la matrícula con ID {m.ID_Matricula}.",
                body: m
            );

            return result;
        }

        
        public async Task<int> DeleteAsync(int id, int idUsuario, object? body)
        {
            int result = await _repo.DeleteAsync(id);

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

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la matrícula con ID {id}."
            );

            return result;
        }

        public async Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo, int idUsuario)
        {
            var data = await _repo.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo);

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

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó la matrícula con ID {id}.",
                body: (object?)entity ?? new { ID_Matricula = id, encontrado = false }
            );

            return entity;
        }

        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, body: null);
        }

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

                    
                    const int max = 8000;
                    bodyJson = json.Length > max ? json.Substring(0, max) + "...(truncado)" : json;
                }

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

        private async Task ValidarPeriodoActivoAsync(int idPeriodo)
        {
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

            var ahora = DateTime.Now;
            if (!(periodo.Fecha_Inicio > ahora))
                throw new Exception("Solo se pueden matricular periodos ACTIVOS (fecha de inicio posterior a la actual).");
        }
        public async Task<(IEnumerable<Adm19ListadoRowDto> datos, int total)> ListadoAdm19Async(
    int? idPeriodo,
    int? idCarrera, 
    int? idCurso,
    int? idGrupo,
    int page,
    int size,
    string? sort,
    bool asc,
    int idUsuario)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;

            var lista = await _repo.ListadoAdm19Async(idPeriodo, idCarrera, idCurso, idGrupo);
            var enumerable = lista.AsEnumerable();

            enumerable = (sort ?? "").ToLower() switch
            {
                "nombre" => asc ? enumerable.OrderBy(x => x.Nombre) : enumerable.OrderByDescending(x => x.Nombre),
                "identificacion" => asc ? enumerable.OrderBy(x => x.Identificacion) : enumerable.OrderByDescending(x => x.Identificacion),
                "curso" => asc ? enumerable.OrderBy(x => x.ID_Curso) : enumerable.OrderByDescending(x => x.ID_Curso),
                "grupo" => asc ? enumerable.OrderBy(x => x.ID_Grupo) : enumerable.OrderByDescending(x => x.ID_Grupo),
                "carrera" => asc ? enumerable.OrderBy(x => x.ID_Carrera) : enumerable.OrderByDescending(x => x.ID_Carrera),
                _ => asc ? enumerable.OrderBy(x => x.ID_Matricula) : enumerable.OrderByDescending(x => x.ID_Matricula)
            };

            var total = enumerable.Count();

            var pageData = enumerable
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            await RegistrarBitacoraAsync(
                idUsuario,
                $"Consultó listado ADM19 (período={idPeriodo}, carrera={idCarrera}, curso={idCurso}, grupo={idGrupo}, page={page}, size={size})",
                new { idPeriodo, idCarrera, idCurso, idGrupo, total });

            return (pageData, total);
        }

        public async Task<byte[]> ExportListadoAdm19CsvAsync(
            int? idPeriodo,
            int? idCarrera,
            int? idCurso,
            int? idGrupo,
            string? sort,
            bool asc,
            int idUsuario)
        {
            var (datos, total) = await ListadoAdm19Async(
                idPeriodo, idCarrera, idCurso, idGrupo,
                page: 1,
                size: int.MaxValue,
                sort: sort,
                asc: asc,
                idUsuario: idUsuario);

            var sb = new StringBuilder();

            sb.AppendLine("ID_Matricula;ID_Estudiante;Tipo_Identificacion;Identificacion;Nombre;ID_Curso;ID_Grupo;ID_Periodo");

            foreach (var r in datos)
            {
                sb.AppendLine(
                    $"{r.ID_Matricula};{r.ID_Estudiante};{r.Tipo_Identificacion};{r.Identificacion};{r.Nombre};{r.ID_Curso};{r.ID_Grupo};{r.ID_Periodo}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            await RegistrarBitacoraAsync(
                idUsuario,
                "Exportó listado ADM19 en CSV",
                new { idPeriodo, idCarrera, idCurso, idGrupo, total });

            return bytes;
        }
    }
}
