using Dapper;
using System.Data;
using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public class MatriculaRepository : IMatriculaRepository
	{
		private readonly IDbConnectionFactory _factory;

		public MatriculaRepository(IDbConnectionFactory factory)
		{
			_factory = factory;
		}

		public async Task<int> InsertAsync(Matricula m)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"
                INSERT INTO Matricula (ID_Estudiante, ID_Curso, ID_Grupo, ID_Periodo)
                VALUES (@ID_Estudiante, @ID_Curso, @ID_Grupo, @ID_Periodo);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
			return await conn.ExecuteScalarAsync<int>(sql, m);
		}

		public async Task<int> UpdateAsync(Matricula m)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"UPDATE Matricula SET
                        ID_Estudiante=@ID_Estudiante, ID_Curso=@ID_Curso, 
                        ID_Grupo=@ID_Grupo, ID_Periodo=@ID_Periodo
                        WHERE ID_Matricula=@ID_Matricula";
			return await conn.ExecuteAsync(sql, m);
		}

		public async Task<int> DeleteAsync(int id)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.ExecuteAsync("DELETE FROM Matricula WHERE ID_Matricula=@id", new { id });
		}

		public async Task<IEnumerable<object>> GetEstudiantesPorCursoYGrupoAsync(int idCurso, int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();

			var sql = @"
        SELECT 
            e.ID_Estudiante,
            e.Identificacion,
            e.Nombre,
            e.Email,
            m.ID_Curso,
            m.ID_Grupo,
            m.ID_Periodo
        FROM Matricula m
        INNER JOIN Estudiante e ON e.ID_Estudiante = m.ID_Estudiante
        WHERE m.ID_Curso = @idCurso AND m.ID_Grupo = @idGrupo;";

			return await conn.QueryAsync<object>(sql, new { idCurso, idGrupo });
		}
	}
}

