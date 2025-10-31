using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class DireccionService : IDireccionService
    {
        private readonly IDireccionRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public DireccionService(IDireccionRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Provincia>> GetProvinciasAsync(int idUsuario)
        {
            var provincias = await _repo.GetProvinciasAsync();

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó las provincias.");

            return provincias;
        }

        public async Task<IEnumerable<Canton>> GetCantonesPorProvinciaAsync(int idProvincia, int idUsuario)
        {
            if (idProvincia <= 0)
                throw new Exception("El parámetro ID_Provincia es requerido y debe ser válido.");

            var cantones = await _repo.GetCantonesPorProvinciaAsync(idProvincia);

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó los cantones de la provincia {idProvincia}.");

            return cantones;
        }

        public async Task<IEnumerable<Distrito>> GetDistritosAsync(int idProvincia, int idCanton, int idUsuario)
        {
            if (idProvincia <= 0 || idCanton <= 0)
                throw new Exception("Los parámetros ID_Provincia e ID_Canton son requeridos y deben ser válidos.");

            var distritos = await _repo.GetDistritosAsync(idProvincia, idCanton);

            //  Registrar evento en Bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó los distritos de la provincia {idProvincia} y cantón {idCanton}.");

            return distritos;
        }

        //  Método reutilizable para registrar acciones en Bitácora
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
            catch (Exception ex)
            {
                Console.WriteLine($" Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}