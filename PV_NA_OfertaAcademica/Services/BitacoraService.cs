using System.Net.Http.Json;

namespace PV_NA_OfertaAcademica.Services
{
    
    /// Servicio auxiliar para registrar acciones en la bitácora (GEN1).
    /// Llama al microservicio de Bitácora a través de HTTP.

    public class BitacoraService
    {
        private readonly HttpClient _client;

        public BitacoraService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("BitacoraClient");
        }

        public async Task RegistrarAsync(string usuario, string descripcion)
        {
            try
            {
                var data = new { usuario, descripcion };
                await _client.PostAsJsonAsync("/bitacora", data);
            }
            catch
            {
                // No interrumpe el flujo si el microservicio de bitácora está caído
            }
        }
    }
}
