using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Net.Http.Json;

namespace PV_NA_Matricula.Services
{
    public class NotasService : INotasService
    {
        private readonly INotasRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public NotasService(INotasRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        // /cargardesglose
        public async Task<int> CargarDesgloseAsync(int idGrupo, List<NotaRubro> rubros, int idUsuario)
        {
            if (rubros == null || !rubros.Any())
                throw new Exception("Debe proporcionar al menos un rubro.");

            // No se puede modificar si ya existen notas
            if (await _repo.ExisteNotasEnGrupoAsync(idGrupo))
                throw new Exception("No se pueden modificar los rubros, ya existen notas asignadas.");

            // Validar que la suma sea exactamente 100
            var suma = rubros.Sum(r => r.Porcentaje);
            if (suma != 100)
                throw new Exception("La suma de los porcentajes debe ser exactamente 100%.");

            // Validar porcentajes mayores a 0
            foreach (var rubro in rubros)
            {
                if (rubro.Porcentaje <= 0)
                    throw new Exception($"El rubro '{rubro.Nombre}' debe tener un porcentaje mayor a 0.");
            }

            int result = await _repo.CargarDesgloseAsync(rubros, idGrupo);

            //  Registrar evento en bitácora
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} cargó el desglose de rubros para el grupo {idGrupo}.");

            return result;
        }

        // /asignarnotarubro
        public async Task<int> AsignarNotaRubroAsync(Nota nota, int idUsuario)
        {
            if (!await _repo.ExisteRubroAsync(nota.ID_Rubro))
                throw new Exception("El rubro especificado no existe.");

            if (nota.Valor < 1 || nota.Valor > 100)
                throw new Exception("El valor de la nota debe estar entre 1 y 100.");

            int result = await _repo.AsignarNotaRubroAsync(nota);

            //  Registrar evento en bitácora
            await RegistrarBitacoraAsync(idUsuario,
                $"El usuario {idUsuario} asignó una nota de {nota.Valor} al rubro {nota.ID_Rubro} para el estudiante {nota.ID_Estudiante}.");

            return result;
        }

        // /obtenerdesglose
        public async Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo, int idUsuario)
        {
            var data = await _repo.ObtenerDesgloseAsync(idGrupo);
            if (!data.Any())
                throw new Exception("No se encontraron rubros para este grupo.");

            //  Registrar consulta
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó el desglose de rubros para el grupo {idGrupo}.");

            return data;
        }

        // /obtenernotas
        public async Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo, int idUsuario)
        {
            var data = await _repo.ObtenerNotasAsync(idEstudiante, idGrupo);
            if (!data.Any())
                throw new Exception("No existen notas registradas para este estudiante en este grupo.");

            //  Registrar consulta
            await RegistrarBitacoraAsync(idUsuario, $"El usuario {idUsuario} consultó las notas del estudiante {idEstudiante} en el grupo {idGrupo}.");

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