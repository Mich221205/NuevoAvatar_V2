using Dapper;
using PV_NA_Notificaciones.Entities;
using System.Data;

namespace PV_NA_Notificaciones.Repository
{
    public class NotificacionRepository
    {
        private readonly IDbConnection _connection;
        public NotificacionRepository(IDbConnection connection) => _connection = connection;

        public async Task<int> InsertarAsync(Notificacion n)
        {
            const string sql = @"
                INSERT INTO Notificacion (Email_Destino, Asunto, Cuerpo, Estado) VALUES (@Email_Destino, @Asunto, @Cuerpo, @Estado);
                SELECT SCOPE_IDENTITY();";
            return await _connection.ExecuteScalarAsync<int>(sql, n);
        }

        public async Task<IEnumerable<Notificacion>> ListarAsync()
        {
            const string sql = "SELECT * FROM Notificacion ORDER BY FechaEnvio DESC";
            return await _connection.QueryAsync<Notificacion>(sql);
        }

        public async Task<Notificacion?> ObtenerPorIdAsync(int id)
        {
            const string sql = "SELECT * FROM Notificacion WHERE ID_Notificacion = @id";
            return await _connection.QueryFirstOrDefaultAsync<Notificacion>(sql, new { id });
        }
    }
}
