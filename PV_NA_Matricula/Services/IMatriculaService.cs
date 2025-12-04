using PV_NA_Matricula.Dtos;
using PV_NA_Matricula.Entities;

public interface IMatriculaService
{
    Task<int> CreateAsync(Matricula m, int idUsuario);
    Task<int> UpdateAsync(Matricula m, int idUsuario);
    Task<int> DeleteAsync(int id, int idUsuario);
    Task<int> DeleteAsync(int id, int idUsuario, object? body);
    Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo, int idUsuario);
    Task<Matricula?> GetByIdAsync(int id, int idUsuario);
    Task<(IEnumerable<Adm19ListadoRowDto> datos, int total)> ListadoAdm19Async(
       int? idPeriodo,
       int? idCarrera,
       int? idCurso,
       int? idGrupo,
       int page,
       int size,
       string? sort,
       bool asc,
       int idUsuario);

    Task<byte[]> ExportListadoAdm19CsvAsync(
        int? idPeriodo,
        int? idCarrera,
        int? idCurso,
        int? idGrupo,
        string? sort,
        bool asc,
        int idUsuario);
}
