using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class PreMatriculaService : IPreMatriculaService
    {
        private readonly IPreMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public PreMatriculaService(IPreMatriculaRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<PreMatricula>> GetAllAsync(int idUsuario)
        {
            var data = await _repo.GetAllAsync();

            //  Registrar consulta en bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó todas las pre-matrículas.");

            return data;
        }

        public async Task<PreMatricula?> GetByIdAsync(int id, int idUsuario)
        {
            var data = await _repo.GetByIdAsync(id);

            //  Registrar consulta en bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó la pre-matrícula con ID {id}.");

            return data;
        }

        public async Task<int> CreateAsync(PreMatricula pre, int idUsuario)
        {
            int id = await _repo.InsertAsync(pre);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} creó una pre-matrícula con ID {id} para el estudiante {pre.ID_Estudiante}.");

            return id;
        }

        public async Task<int> UpdateAsync(PreMatricula pre, int idUsuario)
        {
            int result = await _repo.UpdateAsync(pre);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} actualizó la pre-matrícula con ID {pre.ID_Prematricula}.");

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} eliminó la pre-matrícula con ID {id}.");

            return result;
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
                Console.WriteLine($"⚠️ Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}

