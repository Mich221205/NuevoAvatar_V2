using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public class RolRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RolRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Rol, Nombre FROM Rol ORDER BY Nombre";
            return await conn.QueryAsync<Rol>(sql);
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT ID_Rol, Nombre FROM Rol WHERE ID_Rol = @id";
            return await conn.QueryFirstOrDefaultAsync<Rol>(sql, new { id });
        }

        public async Task<int> CreateAsync(Rol rol)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"INSERT INTO Rol (Nombre)
                           OUTPUT INSERTED.ID_Rol
                           VALUES (@Nombre)";
            return await conn.ExecuteScalarAsync<int>(sql, new { rol.Nombre });
        }

        public async Task<int> UpdateAsync(Rol rol)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"UPDATE Rol SET Nombre = @Nombre
                           WHERE ID_Rol = @ID_Rol";
            return await conn.ExecuteAsync(sql, rol);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "DELETE FROM Rol WHERE ID_Rol = @id";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT 1
                           FROM Rol
                           WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(LTRIM(RTRIM(@nombre)))
                           " + (excludeId.HasValue ? "AND ID_Rol <> @excludeId" : "");
            return await conn.ExecuteScalarAsync<int?>(sql, new { nombre, excludeId }) is not null;
        }

        public async Task<bool> IsRolInUseAsync(int idRol)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = "SELECT TOP 1 1 FROM Usuario WHERE ID_Rol = @idRol";
            return await conn.ExecuteScalarAsync<int?>(sql, new { idRol }) is not null;
        }
    }
}

