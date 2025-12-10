using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class PagosService
    {
        private readonly IHttpClientFactory _httpFactory;

        public PagosService(IHttpClientFactory factory)
        {
            _httpFactory = factory;
        }

        public async Task<int> CountFacturasPendientesAsync(int idEstudiante, string token)
        {
            var client = _httpFactory.CreateClient("PagosClient");

            // 🔹 Enviamos el token al API de Pagos
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // 🔹 Endpoint real (tu backend NO tiene /Factura/listar)
            var url = $"/factura?idEstudiante={idEstudiante}&estado=Pendiente";

            try
            {
                var data = await client.GetFromJsonAsync<List<object>>(url);
                return data?.Count ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PagosService] ERROR → {ex.Message}");
                return 0;
            }
        }
    }
}
