using System.Net.Http.Headers;
using System.Text.Json;
/*
namespace PV_NA_OfertaAcademica.Helpers
{
    public class TokenValidator
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TokenValidator(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> ValidateAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var baseUrl = _config["AuthService:BaseUrl"];
                var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/validate");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<bool>(json);
                return result;
            }
            catch
            {
                return false;
            }
        }

    }
}


/*
 Obtiene la URL base del servicio de autenticación desde configuración (AuthService:BaseUrl).

Envía el token en el header.

Si /validate responde true → el token es válido.
 
 
 */