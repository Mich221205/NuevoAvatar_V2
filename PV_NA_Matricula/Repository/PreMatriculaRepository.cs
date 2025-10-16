using Dapper;
using System.Data;
using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public class PreMatriculaRepository : IPreMatriculaRepository
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public PreMatriculaRepository(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<IEnumerable<PreMatricula>> GetAllAsync()
		{
			using var connection = await _connectionFactory.CreateConnectionAsync();
			return await connection.QueryAsync<PreMatricula>("SELECT * FROM PreMatricula");
		}

		public async Task<PreMatricula?> GetByIdAsync(int id)
		{
			using var connection = await _connectionFactory.CreateConnectionAsync();
			return await connection.QueryFirstOrDefaultAsync<PreMatricula>(
				"SELECT * FROM PreMatricula WHERE ID_Prematricula = @id", new { id });
		}

		public async Task<int> InsertAsync(PreMatricula pre)
		{ 
			using var connection = await _connectionFactory.CreateConnectionAsync();
			var sql = @"
                INSERT INTO PreMatricula (ID_Estudiante, ID_Carrera, ID_Curso, ID_Periodo, Observaciones)
                VALUES (@ID_Estudiante, @ID_Carrera, @ID_Curso, @ID_Periodo, @Observaciones);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
			return await connection.ExecuteScalarAsync<int>(sql, pre);
		}

		public async Task<int> UpdateAsync(PreMatricula pre)
		{
			using var connection = await _connectionFactory.CreateConnectionAsync();
			var sql = @"
                UPDATE PreMatricula
                SET ID_Estudiante=@ID_Estudiante, ID_Carrera=@ID_Carrera, 
                    ID_Curso=@ID_Curso, ID_Periodo=@ID_Periodo, Observaciones=@Observaciones
                WHERE ID_Prematricula=@ID_Prematricula";
			return await connection.ExecuteAsync(sql, pre);
		}

		public async Task<int> DeleteAsync(int id)
		{
			using var connection = await _connectionFactory.CreateConnectionAsync();
			return await connection.ExecuteAsync("DELETE FROM PreMatricula WHERE ID_Prematricula=@id", new { id });
		}
	}
}
