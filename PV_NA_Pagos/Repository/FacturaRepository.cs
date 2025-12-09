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
                INSERT INTO Factura
                (ID_Estudiante, ID_Matricula, ID_Periodo, Monto, Impuesto, Estado, Fecha, MotivoReversion)
                VALUES
                (@ID_Estudiante, @ID_Matricula, @ID_Periodo, @Monto, @Impuesto, @Estado, @Fecha, @MotivoReversion);

                SELECT SCOPE_IDENTITY();";

            return await _connection.ExecuteScalarAsync<int>(sql, factura);
        }

        public async Task<int> CrearDetalleAsync(int idFactura)
        {
            const string sql = @"
                INSERT INTO DetalleFactura (ID_Factura, Descripcion)
                VALUES (@ID_Factura, 'Servicios estudiantiles');";

            return await _connection.ExecuteAsync(sql, new { ID_Factura = idFactura });
        }

        public async Task<Factura?> ObtenerPorIdAsync(int id)
        {
            const string sql = @"SELECT * FROM Factura WHERE ID_Factura = @id";
            return await _connection.QueryFirstOrDefaultAsync<Factura>(sql, new { id });
        }

        public async Task<IEnumerable<Factura>> ListarAsync(
            int? idEstudiante,
            int? idPeriodo,
            string? estado)
        {
            var sql = @"SELECT * FROM Factura WHERE 1 = 1";

            if (idEstudiante.HasValue)
                sql += " AND ID_Estudiante = @ID_Estudiante";

            if (idPeriodo.HasValue)
                sql += " AND ID_Periodo = @ID_Periodo";

            if (!string.IsNullOrWhiteSpace(estado))
                sql += " AND Estado = @Estado";

            sql += " ORDER BY Fecha DESC";

            return await _connection.QueryAsync<Factura>(sql, new
            {
                ID_Estudiante = idEstudiante,
                ID_Periodo = idPeriodo,
                Estado = estado
            });
        }

        public async Task<bool> ReversarAsync(int id, string motivo)
        {
            const string sql = @"
        UPDATE Factura
        SET Estado = 'Anulada',
            MotivoReversion = @Motivo
        WHERE ID_Factura = @id";

            int rows = await _connection.ExecuteAsync(sql, new { id, Motivo = motivo });
            return rows > 0;
        }
        public async Task<bool> MarcarComoPagadaAsync(int id)
        {
            const string sql = @"
        UPDATE Factura
        SET Estado = 'Pagada'
        WHERE ID_Factura = @id";

            int rows = await _connection.ExecuteAsync(sql, new { id });
            return rows > 0;
        }
    }
}
