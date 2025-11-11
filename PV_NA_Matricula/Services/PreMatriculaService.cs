using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;
//para serializar el body en Accion
using System.Text.Json;
using System.Text.Json.Serialization;
// NUEVO
using Microsoft.AspNetCore.Http;

namespace PV_NA_Matricula.Services
{
    public class PreMatriculaService : IPreMatriculaService
    {
        private readonly IPreMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        // cliente hacia Oferta Académica para validar periodos
        private readonly HttpClient _ofertaClient;

        // NUEVO: para leer el Authorization entrante y reenviarlo
        private readonly IHttpContextAccessor _httpContextAccessor;

        // opciones de serialización compacta
        private static readonly JsonSerializerOptions _jsonOpts = new()
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

        // Agrego IHttpContextAccessor 
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

            //  Registrar consulta en bitácora
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

            //  Registrar consulta en bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó la pre-matrícula con ID {id}.",
                body: (object?)data ?? new { ID_Prematricula = id, encontrado = false }
            );

            return data;
        }

        public async Task<int> CreateAsync(PreMatricula pre, int idUsuario)
        {
            //validar periodo FUTURO (fecha de inicio posterior a la actual)
            await ValidarPeriodoFuturoAsync(pre.ID_Periodo);

            int id = await _repo.InsertAsync(pre);

            // reflejar el ID generado en el body
            pre.ID_Prematricula = id;

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} creó una pre-matrícula con ID {id} para el estudiante {pre.ID_Estudiante}.",
                body: pre
            );

            return id;
        }

        public async Task<int> UpdateAsync(PreMatricula pre, int idUsuario)
        {
            // validar periodo FUTURO (fecha de inicio posterior a la actual)
            await ValidarPeriodoFuturoAsync(pre.ID_Periodo);

            int result = await _repo.UpdateAsync(pre);

            //  Registrar en Bitácora
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

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} eliminó la pre-matrícula con ID {id}."
            );

            return result;
        }

        // overload para pasar el objeto eliminado y guardarlo como body en bitácora
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

        //  Método reutilizable para registrar acciones en Bitácora
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, body: null);
        }

        // Bitácora que incrusta el body dentro del texto Accion
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion, object? body)
        {
            try
            {
                string accionConBody = accion;

                if (body is not null)
                {
                    var json = JsonSerializer.Serialize(body, _jsonOpts);

                    //limitar para no exceder el tamaño de la columna Accion
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

        // ======================================================
        // Validación específica de periodo FUTURO (inicio > ahora)
        // ======================================================
        private async Task ValidarPeriodoFuturoAsync(int idPeriodo)
        {
            // Reenviar Authorization si viene en la request
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

            var ahora = DateTime.Now; // usa UtcNow si tus fechas están en UTC

            if (!(periodo.Fecha_Inicio > ahora))
                throw new Exception("Solo se pueden prematricular periodos FUTUROS (fecha de inicio posterior a la actual).");
        }
    }
}

