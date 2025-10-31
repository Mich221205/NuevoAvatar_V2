using PV_NA_Matricula.Entities;

public interface IEstudianteService
{
    Task<IEnumerable<Estudiante>> GetAllAsync(int idUsuario);
    Task<Estudiante?> GetByIdAsync(int id, int idUsuario);
    Task<int> CreateAsync(Estudiante e, int idUsuario);
    Task<int> UpdateAsync(Estudiante e, int idUsuario);
    Task<int> DeleteAsync(int id, int idUsuario);
}