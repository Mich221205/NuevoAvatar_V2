using PV_NA_Academico.Entities;
using PV_NA_Academico.Repository;
using System.Net.Http.Json;

namespace PV_NA_Academico.Services
{
    public class ListadoEstudiantesService
    {
        private readonly ListadoEstudiantesRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public ListadoEstudiantesService(
            ListadoEstudiantesRepository repo,
            IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<ListadoEstudiante>> ObtenerPorPeriodoAsync(string periodo, int idUsuario)
        {
          
            if (string.IsNullOrWhiteSpace(periodo))
                throw new ArgumentException("Debe indicar el período a consultar.");

           
            var listado = await _repo.ObtenerPorPeriodoAsync(periodo);

            
            if (listado == null || !listado.Any())
                throw new InvalidOperationException("No se encontraron estudiantes para el período indicado.");

    
            var accion = $"El usuario {idUsuario} consultó el listado de estudiantes del período {periodo}.";
            try
            {
                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accion
                });
            }
            catch
            {
                
            }

            return listado;
        }
    }
}
