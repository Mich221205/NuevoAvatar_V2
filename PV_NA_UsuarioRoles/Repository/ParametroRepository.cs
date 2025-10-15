using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public class ParametroRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ParametroRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Parametro>> GetAllAsync()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Parametro, Valor FROM Parametro ORDER BY ID_Parametro";
            return await conn.QueryAsync<Parametro>(sql);
        }

        public async Task<Parametro?> GetByIdAsync(string id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Parametro, Valor FROM Parametro WHERE ID_Parametro = @id";
            return await conn.QueryFirstOrDefaultAsync<Parametro>(sql, new { id });
        }

        public async Task<int> CreateAsync(Parametro parametro)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"INSERT INTO Parametro (ID_Parametro, Valor)
                           VALUES (@ID_Parametro, @Valor)";
            return await conn.ExecuteAsync(sql, parametro);
        }

        public async Task<int> UpdateAsync(Parametro parametro)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"UPDATE Parametro 
                           SET Valor = @Valor
                           WHERE ID_Parametro = @ID_Parametro";
            return await conn.ExecuteAsync(sql, parametro);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "DELETE FROM Parametro WHERE ID_Parametro = @id";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT 1 FROM Parametro WHERE ID_Parametro = @id";
            return await conn.ExecuteScalarAsync<int?>(sql, new { id }) is not null;
        }
    }
}
