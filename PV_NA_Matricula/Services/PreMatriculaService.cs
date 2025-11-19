using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace PV_NA_Matricula.Services
{
    public class PreMatriculaService : IPreMatriculaService
    {
        private readonly IPreMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;
        private readonly HttpClient _ofertaClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOpts = new()
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

        
        public PreMatriculaService(
            IPreMatriculaRepository repo,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
            _ofertaClient = httpClientFactory.CreateClient("OfertaClient");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<PreMatricula>> GetAllAsync(int idUsuario)
        {
            var data = await _repo.GetAllAsync();

            
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó todas las pre-matrículas.",
                new { total = data.Count(), resultado = data }
            );

            return data;
        }

        public async Task<PreMatricula?> GetByIdAsync(int id, int idUsuario)
        {
            var data = await _repo.GetByIdAsync(id);

            
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó la pre-matrícula con ID {id}.",
                body: (object?)data ?? new { ID_Prematricula = id, encontrado = false }
            );

            return data;
        }

        public async Task<int> CreateAsync(PreMatricula pre, int idUsuario)
        {
            
            await ValidarPeriodoFuturoAsync(pre.ID_Periodo);

            int id = await _repo.InsertAsync(pre);

            pre.ID_Prematricula = id;

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} creó una pre-matrícula con ID {id} para el estudiante {pre.ID_Estudiante}.",
                body: pre
            );

            return id;
        }

        public async Task<int> UpdateAsync(PreMatricula pre, int idUsuario)
        {
            
            await ValidarPeriodoFuturoAsync(pre.ID_Periodo);

            int result = await _repo.UpdateAsync(pre);

            
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} actualizó la pre-matrícula con ID {pre.ID_Prematricula}.",
                body: pre
            );

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

           
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la pre-matrícula con ID {id}."
            );

            return result;
        }

        
        public async Task<int> DeleteAsync(int id, int idUsuario, object? body)
        {
            int result = await _repo.DeleteAsync(id);

            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la pre-matrícula con ID {id}.",
                body: body ?? new { ID_Prematricula = id }
            );

            return result;
        }

       
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, body: null);
        }

        
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion, object? body)
        {
            try
            {
                string accionConBody = accion;

                if (body is not null)
                {
                    var json = JsonSerializer.Serialize(body, _jsonOpts);

                    
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
                Console.WriteLine($" Error al registrar bitácora: {ex.Message}");
            }
        }

        private async Task ValidarPeriodoFuturoAsync(int idPeriodo)
        {
            
            var req = new HttpRequestMessage(HttpMethod.Get, $"/periodo/{idPeriodo}");
            var auth = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(auth))
                req.Headers.TryAddWithoutValidation("Authorization", auth);

            var resp = await _ofertaClient.SendAsync(req);

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
                throw new Exception("Solo se pueden prematricular periodos FUTUROS (fecha de inicio posterior a la actual).");
        }
    }
}

