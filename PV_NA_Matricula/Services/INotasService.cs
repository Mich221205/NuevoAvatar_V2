 using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
	public interface INotasService
	{
		Task<int> CargarDesgloseAsync(int idGrupo, List<NotaRubro> rubros);
		Task<int> AsignarNotaRubroAsync(Nota nota);
		Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo);
		Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo);
	}
}
