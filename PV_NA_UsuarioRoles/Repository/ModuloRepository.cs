using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public class ModuloRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ModuloRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Modulo>> GetAllAsync()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Modulo, Nombre FROM Modulo ORDER BY ID_Modulo";
            return await conn.QueryAsync<Modulo>(sql);
        }

        public async Task<Modulo?> GetByIdAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Modulo, Nombre FROM Modulo WHERE ID_Modulo = @id";
            return await conn.QueryFirstOrDefaultAsync<Modulo>(sql, new { id });
        }

        public async Task<int> CreateAsync(Modulo modulo)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"INSERT INTO Modulo (Nombre)
                           OUTPUT INSERTED.ID_Modulo
                           VALUES (@Nombre)";
            return await conn.ExecuteScalarAsync<int>(sql, modulo);
        }

        public async Task<int> UpdateAsync(Modulo modulo)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"UPDATE Modulo SET Nombre = @Nombre WHERE ID_Modulo = @ID_Modulo";
            return await conn.ExecuteAsync(sql, modulo);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "DELETE FROM Modulo WHERE ID_Modulo = @id";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT 1 FROM Modulo 
                           WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(LTRIM(RTRIM(@nombre))) 
                           " + (excludeId.HasValue ? "AND ID_Modulo <> @excludeId" : "");
            return await conn.ExecuteScalarAsync<int?>(sql, new { nombre, excludeId }) is not null;
        }
    }
}