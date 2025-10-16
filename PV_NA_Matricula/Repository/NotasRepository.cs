using Dapper;
using PV_NA_Matricula.Entities;
using System.Data;

namespace PV_NA_Matricula.Repository
{
	public class NotasRepository : INotasRepository
	{
		private readonly IDbConnectionFactory _factory;

		public NotasRepository(IDbConnectionFactory factory)
		{
			_factory = factory;
		}

		public async Task<IEnumerable<NotaRubro>> ObtenerDesgloseAsync(int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryAsync<NotaRubro>(
				"SELECT * FROM Nota_Rubro WHERE ID_Grupo = @idGrupo",
				new { idGrupo });
		}

		public async Task<IEnumerable<Nota>> ObtenerNotasAsync(int idEstudiante, int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"
                SELECT 
                    n.ID_Nota, 
                    n.ID_Rubro,  
                    n.ID_Estudiante, 
                    n.Valor,
					n.ID_Grupo
                FROM Nota n
                WHERE n.ID_Estudiante = @idEstudiante AND n.ID_Grupo = @idGrupo;";
			return await conn.QueryAsync<Nota>(sql, new { idEstudiante, idGrupo });
		}

		public async Task<int> CargarDesgloseAsync(List<NotaRubro> rubros, int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			using var tran = conn.BeginTransaction();

			// Eliminar rubros anteriores
			await conn.ExecuteAsync("DELETE FROM Nota_Rubro WHERE ID_Grupo = @idGrupo", new { idGrupo }, tran);

			int total = 0;
			foreach (var r in rubros)
			{
				var sql = @"INSERT INTO Nota_Rubro (ID_Grupo, Nombre, Porcentaje)
                            VALUES (@ID_Grupo, @Nombre, @Porcentaje)";
				total += await conn.ExecuteAsync(sql, r, tran);
			}

			tran.Commit();
			return total;
		}

		public async Task<int> AsignarNotaRubroAsync(Nota nota)
		{
			using var conn = await _factory.CreateConnectionAsync();

			var existe = await ExisteNotaAsync(nota.ID_Estudiante, nota.ID_Rubro, nota.ID_Grupo);
			if (existe)
			{
				var updateSql = "UPDATE Nota SET Valor=@Valor WHERE ID_Estudiante=@ID_Estudiante AND ID_Rubro=@ID_Rubro AND ID_Grupo = @ID_Grupo";
				return await conn.ExecuteAsync(updateSql, nota);
			}
			else
			{
				var insertSql = "INSERT INTO Nota (ID_Rubro, ID_Estudiante, Valor, ID_Grupo) VALUES (@ID_Rubro, @ID_Estudiante, @Valor, @ID_Grupo)";
				return await conn.ExecuteAsync(insertSql, nota);
			}
		}

		public async Task<decimal> SumaPorcentajesAsync(int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.ExecuteScalarAsync<decimal>(
				"SELECT ISNULL(SUM(Porcentaje),0) FROM Nota_Rubro WHERE ID_Grupo=@idGrupo",
				new { idGrupo });
		}

		public async Task<bool> ExisteNotasEnGrupoAsync(int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"
        SELECT COUNT(*) 
        FROM Nota n
        INNER JOIN Nota_Rubro nr ON n.ID_Rubro = nr.ID_Rubro
        WHERE nr.ID_Grupo = @idGrupo";
			var count = await conn.ExecuteScalarAsync<int>(sql, new { idGrupo });
			return count > 0;
		}

		public async Task<bool> ExisteRubroAsync(int idRubro)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var count = await conn.ExecuteScalarAsync<int>(
				"SELECT COUNT(*) FROM Nota_Rubro WHERE ID_Rubro=@idRubro",
				new { idRubro });
			return count > 0;
		}

		public async Task<bool> ExisteNotaAsync(int idEstudiante, int idRubro, int idGrupo)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var count = await conn.ExecuteScalarAsync<int>(
				"SELECT COUNT(*) FROM Nota WHERE ID_Estudiante=@idEstudiante AND ID_Rubro=@idRubro AND ID_Grupo=@idGrupo",
				new { idEstudiante, idRubro, idGrupo });
			return count > 0;
		}
	}
}
