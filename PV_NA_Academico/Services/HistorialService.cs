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
           
            if (string.IsNullOrWhiteSpace(tipoIdentificacion))
                throw new ArgumentException("Debe indicar el tipo de identificación del estudiante.");

            if (string.IsNullOrWhiteSpace(identificacion))
                throw new ArgumentException("Debe indicar la identificación del estudiante.");

          
            var historial = await _repository.ObtenerHistorialAsync(tipoIdentificacion, identificacion);

            if (historial == null || !historial.Any())
                throw new InvalidOperationException("No se encontraron cursos para el estudiante indicado.");

           
            var accion = $"El usuario {idUsuario} consultó el historial académico del estudiante {identificacion}.";
            await _httpClient.PostAsJsonAsync("/bitacora", new
            {
                ID_Usuario = idUsuario,
                Accion = accion
            });

            return historial;
        }
    }
}
