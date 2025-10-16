using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public interface INotasRepository
	{
		Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo);
		Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo);
		Task<int> CargarDesgloseAsync(List<NotaRubro> rubros, int idGrupo);
		Task<int> AsignarNotaRubroAsync(Nota nota);
		Task<decimal> SumaPorcentajesAsync(int idGrupo);
		Task<bool> ExisteNotasEnGrupoAsync(int idGrupo);
		Task<bool> ExisteRubroAsync(int idRubro);
		Task<bool> ExisteNotaAsync(int idEstudiante, int idRubro, int idGrupo);
	}
}
 