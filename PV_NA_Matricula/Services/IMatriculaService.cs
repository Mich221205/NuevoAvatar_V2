using PV_NA_Matricula.Entities;

public interface IMatriculaService
{
    Task<int> CreateAsync(Matricula m, int idUsuario);
    Task<int> UpdateAsync(Matricula m, int idUsuario);
    Task<int> DeleteAsync(int id, int idUsuario);
    Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo, int idUsuario);
}