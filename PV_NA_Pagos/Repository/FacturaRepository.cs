using Dapper;
using PV_NA_Pagos.Entities;
using System.Data;

namespace PV_NA_Pagos.Repository
{
    public class FacturaRepository
    {
        private readonly IDbConnection _connection;

        public FacturaRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> CrearAsync(Factura factura)
        {
            const string sql = @"
                INSERT INTO Factura (ID_Estudiante, Monto, Impuesto, Estado) VALUES (@ID_Estudiante, @Monto, @Impuesto, @Estado);
                SELECT SCOPE_IDENTITY();";
            return await _connection.ExecuteScalarAsync<int>(sql, factura);
        }

        public async Task<int> CrearDetalleAsync(int idFactura)
        {
            const string sql = @"INSERT INTO DetalleFactura (ID_Factura, Descripcion) VALUES (@ID_Factura, 'Servicios estudiantiles');";
            return await _connection.ExecuteAsync(sql, new { ID_Factura = idFactura });
        }

        public async Task<Factura?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"SELECT * FROM Factura WHERE ID_Factura = @id";
            return await _connection.QueryFirstOrDefaultAsync<Factura>(sql, new { id });
        }

        public async Task<IEnumerable<Factura>> ListarAsync()
        {
            const string sql = @"SELECT * FROM Factura ORDER BY Fecha DESC";
            return await _connection.QueryAsync<Factura>(sql);
        }

        public async Task<bool> ReversarAsync(int id)
        {
            const string sql = @"UPDATE Factura SET Estado = 'Anulada' WHERE ID_Factura = @id";
            int rows = await _connection.ExecuteAsync(sql, new { id });
            return rows > 0;
        }
    }
}
