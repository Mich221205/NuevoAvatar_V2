using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;
//para serializar el body en Accion
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PV_NA_Matricula.Services
{
    public class DireccionService : IDireccionService
    {
        private readonly IDireccionRepository _repo;
        private readonly HttpClient _bitacoraClient;

        //opciones de serialización compacta
        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public DireccionService(IDireccionRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Provincia>> GetProvinciasAsync(int idUsuario)
        {
            var provincias = await _repo.GetProvinciasAsync();

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó las provincias.",
                new { total = provincias.Count(), resultado = provincias }
            );

            return provincias;
        }

        public async Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia, int idUsuario)
        {
            if (idProvincia <= 0)
                throw new Exception("El parámetro ID_Provincia es requerido y debe ser válido.");

            var cantones = await _repo.GetCantonesPorProvinciaAsync(idProvincia);

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó los cantones de la provincia {idProvincia}.",
                new { idProvincia, total = cantones.Count(), resultado = cantones }
            );

            return cantones;
        }

        public async Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton, int idUsuario)
        {
            if (idProvincia <= 0 || idCanton <= 0)
                throw new Exception("Los parámetros ID_Provincia e ID_Canton son requeridos y deben ser válidos.");

            var distritos = await _repo.GetDistritosAsync(idProvincia, idCanton);

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó los distritos de la provincia {idProvincia} y cantón {idCanton}.",
                new { idProvincia, idCanton, total = distritos.Count(), resultado = distritos }
            );

            return distritos;
        }

        //  Método reutilizable para registrar acciones en Bitácora
        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
            await RegistrarBitacoraAsync(idUsuario, accion, null);
        }

        //Bitácora que incrusta el body dentro del texto Accion
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
    }
}
