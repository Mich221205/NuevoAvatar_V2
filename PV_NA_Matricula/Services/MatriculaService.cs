using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public MatriculaService(IMatriculaRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<int> CreateAsync(Matricula m, int idUsuario)
        {
            int id = await _repo.InsertAsync(m);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} creó una matrícula con ID {id} para el estudiante {m.ID_Estudiante}, curso {m.ID_Curso}, grupo {m.ID_Grupo}.");

            return id;
        }

        public async Task<int> UpdateAsync(Matricula m, int idUsuario)
        {
            int result = await _repo.UpdateAsync(m);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} actualizó la matrícula con ID {m.ID_Matricula}.");

            return result;
        }

        public async Task<int> DeleteAsync(int id, int idUsuario)
        {
            int result = await _repo.DeleteAsync(id);

            //  Registrar en Bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} eliminó la matrícula con ID {id}.");

            return result;
        }

        public async Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo, int idUsuario)
        {
            var data = await _repo.GetEstudiantesPorCursoYGrupoAsync(idCurso, idGrupo);

            // Registrar consulta en bitácora
            await RegistrarBitacoraAsync(
                idUsuario,
                $"El usuario {idUsuario} consultó estudiantes del curso {idCurso} y grupo {idGrupo}."
            );

            return data;
        }


        //  Método para registrar acciones en la bitácora
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