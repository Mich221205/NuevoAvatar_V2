using PV_NA_Notificaciones.Entities;
using PV_NA_Notificaciones.Repository;
using System.Net.Http.Json;

namespace PV_NA_Notificaciones.Services
{
    public class NotificacionService
    {
        private readonly NotificacionRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public NotificacionService(NotificacionRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<Notificacion> EnviarAsync(string email, string asunto, string cuerpo, int idUsuario)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Debe indicar el correo destino.");

            if (string.IsNullOrWhiteSpace(asunto))
                throw new ArgumentException("Debe indicar el asunto del mensaje.");

            if (string.IsNullOrWhiteSpace(cuerpo))
                throw new ArgumentException("El cuerpo del mensaje no puede estar vacío.");

            var notificacion = new Notificacion
            {
                Email_Destino = email,
                Asunto = asunto,
                Cuerpo = cuerpo,
                Estado = "Enviado",
                FechaEnvio = DateTime.Now
            };

            int id = await _repo.InsertarAsync(notificacion);
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} envió una notificación a {email} con asunto '{asunto}'.");

            notificacion.ID_Notificacion = id;
            return notificacion;
        }

        public async Task<IEnumerable<Notificacion>> ListarAsync() => await _repo.ListarAsync();

        public async Task<Notificacion?> ObtenerPorIdAsync(int id) => await _repo.ObtenerPorIdAsync(id);

        private async Task RegistrarBitacoraAsync(int idUsuario, string accion)
        {
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
        }
    }
}
