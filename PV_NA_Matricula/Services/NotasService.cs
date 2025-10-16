using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;

namespace PV_NA_Matricula.Services
{
	public class NotasService : INotasService
	{
		private readonly INotasRepository _repo;
		public NotasService(INotasRepository repo)
		{
			_repo = repo;
		}

		// /cargardesglose
		public async Task<int> CargarDesgloseAsync(int idGrupo, List<NotaRubro> rubros)
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
			 
			// Reemplaza los rubros anteriores
			return await _repo.CargarDesgloseAsync(rubros, idGrupo);
		}

		// /asignarnotarubro
		public async Task<int> AsignarNotaRubroAsync(Nota nota)
		{
			if (!await _repo.ExisteRubroAsync(nota.ID_Rubro))
				throw new Exception("El rubro especificado no existe.");

			if (nota.Valor < 1 || nota.Valor > 100)
				throw new Exception("El valor de la nota debe estar entre 1 y 100.");

			return await _repo.AsignarNotaRubroAsync(nota);
		}

		// /obtenerdesglose
		public async Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo)
		{
			var data = await _repo.ObtenerDesgloseAsync(idGrupo);
			if (!data.Any())
				throw new Exception("No se encontraron rubros para este grupo.");
			return data;
		}

		// /obtenernotas
		public async Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo)
		{
			var data = await _repo.ObtenerNotasAsync(idEstudiante, idGrupo);
			if (!data.Any())
				throw new Exception("No existen notas registradas para este estudiante en este grupo.");
			return data;
		}
	}
}
