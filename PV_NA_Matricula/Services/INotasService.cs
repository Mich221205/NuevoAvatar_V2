using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Services
{
    public interface INotasService
    {
        Task<int> CargarDesgloseAsync(int idGrupo, List<NotaRubro> rubros, int idUsuario);
        Task<int> AsignarNotaRubroAsync(Nota nota, int idUsuario);
        Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo, int idUsuario);
        Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo, int idUsuario);
    }
}
