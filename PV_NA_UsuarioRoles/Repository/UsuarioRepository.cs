using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public class UsuarioRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UsuarioRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT U.*, R.Nombre AS RolNombre
                           FROM Usuario U
                           INNER JOIN Rol R ON U.ID_Rol = R.ID_Rol";
            return await conn.QueryAsync<Usuario>(sql);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT U.*, R.Nombre AS RolNombre
                           FROM Usuario U
                           INNER JOIN Rol R ON U.ID_Rol = R.ID_Rol
                           WHERE U.ID_Usuario = @id";
            return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        }

        public async Task<IEnumerable<Usuario>> FilterAsync(string? identificacion, string? nombre, string? tipo)
        {
            using var conn = _dbConnectionFactory.CreateConnection();

            string sql = @"
                SELECT U.*, R.Nombre AS RolNombre
                FROM Usuario U
                INNER JOIN Rol R ON U.ID_Rol = R.ID_Rol
                WHERE (@identificacion IS NULL OR U.Identificacion LIKE '%' + @identificacion + '%')
                AND (@nombre IS NULL OR U.Nombre LIKE '%' + @nombre + '%')
                AND (@tipo IS NULL OR R.Nombre LIKE '%' + @tipo + '%')"; //filtra por rol (Profesor/Estudiante)

            return await conn.QueryAsync<Usuario>(sql, new { identificacion, nombre, tipo });
        }


        public async Task<int> CreateAsync(Usuario usuario)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"
                INSERT INTO Usuario 
                    (Email, Tipo_Identificacion, Identificacion, Nombre, Contrasena, ID_Rol)
                OUTPUT INSERTED.ID_Usuario
                VALUES (@Email, @Tipo_Identificacion, @Identificacion, @Nombre, @Contrasena, @ID_Rol);";
            return await conn.ExecuteScalarAsync<int>(sql, usuario);
        }

        public async Task<int> UpdateAsync(Usuario usuario)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"UPDATE Usuario SET 
                            Email = @Email,
                            Tipo_Identificacion = @Tipo_Identificacion,
                            Identificacion = @Identificacion,
                            Nombre = @Nombre,
                            Contrasena = @Contrasena,
                            ID_Rol = @ID_Rol
                           WHERE ID_Usuario = @ID_Usuario";
            return await conn.ExecuteAsync(sql, usuario);
        }

        public async Task DeleteSessionsByUserIdAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            await conn.ExecuteAsync("DELETE FROM Sesion WHERE ID_Usuario = @id", new { id });
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            return await conn.ExecuteAsync("DELETE FROM Usuario WHERE ID_Usuario = @id", new { id });
        }

        // =====================================
        // Métodos utilizados por AuthService
        // =====================================
        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT U.ID_Usuario, U.Email, U.Contrasena, R.Nombre AS RolNombre, U.ID_Rol
                   FROM Usuario U
                   INNER JOIN Rol R ON U.ID_Rol = R.ID_Rol
                   WHERE U.Email = @Email";
            return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            string sql = @"SELECT U.ID_Usuario, U.Email, U.Contrasena, R.Nombre AS RolNombre, U.ID_Rol
                   FROM Usuario U
                   INNER JOIN Rol R ON U.ID_Rol = R.ID_Rol
                   WHERE U.ID_Usuario = @Id";
            return await conn.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

    }
}
