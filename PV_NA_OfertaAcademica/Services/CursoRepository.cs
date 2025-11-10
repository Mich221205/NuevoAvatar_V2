using Dapper;
using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Services;
using System.Data;

namespace PV_NA_OfertaAcademica.Repository
{
    public class CursoRepository : ICursoRepository
    {
        private readonly IDbConnection _db;
        public CursoRepository(IDbConnection db) => _db = db;

        public async Task<int> CreateAsync(Curso e)
        {
            var sql = @"INSERT INTO dbo.Curso(Nombre, Nivel, ID_Carrera)
                        VALUES(@Nombre, @Nivel, @ID_Carrera);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _db.ExecuteScalarAsync<int>(sql, e);
        }

        public Task<int> UpdateAsync(Curso e)
        {
            var sql = @"UPDATE dbo.Curso
                        SET Nombre=@Nombre, Nivel=@Nivel, ID_Carrera=@ID_Carrera
                        WHERE ID_Curso=@ID_Curso";
            return _db.ExecuteAsync(sql, e);
        }

        // Eliminar curso por ID
        public Task<int> DeleteAsync(int id)
            => _db.ExecuteAsync("DELETE FROM dbo.Curso WHERE ID_Curso=@id", new { id });

        // . Obtener curso por ID
        public Task<Curso?> GetByIdAsync(int id)
            => _db.QueryFirstOrDefaultAsync<Curso>("SELECT * FROM dbo.Curso WHERE ID_Curso=@id", new { id });

        // . Obtener todos los cursos
        public Task<IEnumerable<Curso>> GetAllAsync()
            => _db.QueryAsync<Curso>("SELECT * FROM dbo.Curso ORDER BY ID_Carrera, Nivel, Nombre");

        public Task<IEnumerable<Curso>> GetByCarreraAsync(int idCarrera)
            => _db.QueryAsync<Curso>("SELECT * FROM dbo.Curso WHERE ID_Carrera=@idCarrera ORDER BY Nivel, Nombre", new { idCarrera });

        public Task<bool> CarreraExistsAsync(int idCarrera)
            => _db.ExecuteScalarAsync<int>("SELECT 1 FROM dbo.Carrera WHERE ID_Carrera=@id", new { id = idCarrera })
                  .ContinueWith(t => t.Result == 1);

        public Task<bool> ExistsByNombreAsync(int idCarrera, string nombre, int? excludeId = null)
        {
            var sql = @"SELECT COUNT(1) FROM dbo.Curso 
                        WHERE ID_Carrera=@idCarrera AND Nombre=@nombre
                        AND (@excludeId IS NULL OR ID_Curso<>@excludeId)";
            return _db.ExecuteScalarAsync<int>(sql, new { idCarrera, nombre, excludeId })
                      .ContinueWith(t => t.Result > 0);
        }

        public Task<bool> HasGruposAsync(int idCurso)
            => _db.ExecuteScalarAsync<int>("SELECT TOP 1 1 FROM dbo.Grupo WHERE ID_Curso=@id", new { id = idCurso })
                  .ContinueWith(t => t.Result == 1);
    }
}