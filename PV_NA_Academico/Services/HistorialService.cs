using PV_NA_Academico.Entities;
using PV_NA_Academico.Repository;
using System.Net.Http.Json;

namespace PV_NA_Academico.Services
{
    public class HistorialService
    {
        private readonly HistorialRepository _repository;
        private readonly HttpClient _httpClient;

        public HistorialService(HistorialRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _httpClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<HistorialAcademico>> ObtenerHistorialAsync(
            string tipoIdentificacion, string identificacion, int idUsuario)
        {
            var historial = await _repository.ObtenerHistorialAsync(tipoIdentificacion, identificacion);

            var accion = $"El usuario {idUsuario} consultó historial académico del estudiante {identificacion}.";
            await _httpClient.PostAsJsonAsync("/bitacora", new
            {
                ID_Usuario = idUsuario,
                Accion = accion
            });

            return historial;
        }
    }
}
