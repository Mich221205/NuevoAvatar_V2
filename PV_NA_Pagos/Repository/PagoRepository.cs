using Dapper;
using PV_NA_Pagos.Entities;
using System.Data;

namespace PV_NA_Pagos.Repository
{
    public class PagoRepository
    {
        private readonly IDbConnection _connection;
        public PagoRepository(IDbConnection connection) => _connection = connection;

        public async Task<int> CrearAsync(Pago pago)
        {
            const string sql = @"
                INSERT INTO Pago (ID_Factura, Monto, Estado) VALUES (@ID_Factura, @Monto, 'Aplicado');
                SELECT SCOPE_IDENTITY();";
            return await _connection.ExecuteScalarAsync<int>(sql, pago);
        }

        public async Task<Pago?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"SELECT * FROM Pago WHERE ID_Pago = @id";
            return await _connection.QueryFirstOrDefaultAsync<Pago>(sql, new { id });
        }

        public async Task<IEnumerable<Pago>> ListarAsync()
        {
            const string sql = @"SELECT * FROM Pago ORDER BY FechaPago DESC";
            return await _connection.QueryAsync<Pago>(sql);
        }

        public async Task<bool> ReversarAsync(int id)
        {
            const string sql = @"UPDATE Pago SET Estado = 'Anulado' WHERE ID_Pago = @id";
            int rows = await _connection.ExecuteAsync(sql, new { id });
            return rows > 0;
        }
    }
}
