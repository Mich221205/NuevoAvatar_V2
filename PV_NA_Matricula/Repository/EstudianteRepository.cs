using Dapper;
using System.Data;
using PV_NA_Matricula.Entities;

namespace PV_NA_Matricula.Repository
{
	public class EstudianteRepository : IEstudianteRepository
	{
		private readonly IDbConnectionFactory _factory;

		public EstudianteRepository(IDbConnectionFactory factory)
		{
			_factory = factory;
		}

		public async Task<IEnumerable<Estudiante>> GetAllAsync()
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryAsync<Estudiante>("SELECT * FROM Estudiante");
		} 

		public async Task<Estudiante?> GetByIdAsync(int id)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.QueryFirstOrDefaultAsync<Estudiante>(
				"SELECT * FROM Estudiante WHERE ID_Estudiante = @id", new { id });
		}

		public async Task<int> InsertAsync(Estudiante e)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"INSERT INTO Estudiante
                        (Identificacion, Tipo_Identificacion, Email, Nombre, Fecha_Nacimiento, Direccion, Telefono)
                        VALUES (@Identificacion, @Tipo_Identificacion, @Email, @Nombre, @Fecha_Nacimiento, @Direccion, @Telefono);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
			return await conn.ExecuteScalarAsync<int>(sql, e);
		}

		public async Task<int> UpdateAsync(Estudiante e)
		{
			using var conn = await _factory.CreateConnectionAsync();
			var sql = @"UPDATE Estudiante
                        SET Identificacion=@Identificacion, Tipo_Identificacion=@Tipo_Identificacion,
                            Email=@Email, Nombre=@Nombre, Fecha_Nacimiento=@Fecha_Nacimiento,
                            Direccion=@Direccion, Telefono=@Telefono
                        WHERE ID_Estudiante=@ID_Estudiante;";
			return await conn.ExecuteAsync(sql, e);
		}

		public async Task<int> DeleteAsync(int id)
		{
			using var conn = await _factory.CreateConnectionAsync();
			return await conn.ExecuteAsync("DELETE FROM Estudiante WHERE ID_Estudiante=@id", new { id });
		}
	}
}

