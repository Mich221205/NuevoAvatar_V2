using Dapper;
using PV_NA_OfertaAcademica.Entities;
using System.Data;

namespace PV_NA_OfertaAcademica.Repository
{
    public class PeriodoRepository
    {
        private readonly IDbConnection _connection;

        public PeriodoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // Obtener todos los periodos
        public async Task<IEnumerable<Periodo>> GetAllAsync()
            => await _connection.QueryAsync<Periodo>("SELECT * FROM Periodo");

        public async Task<Periodo?> GetByIdAsync(int id)
            => await _connection.QueryFirstOrDefaultAsync<Periodo>("SELECT * FROM Periodo WHERE ID_Periodo = @id", new { id });

        // Crear nuevo periodo
        public async Task<int> CreateAsync(Periodo periodo)
        {
            string sql = @"INSERT INTO Periodo (Anno, Numero, Fecha_Inicio, Fecha_Fin)
                           VALUES (@Anno, @Numero, @Fecha_Inicio, @Fecha_Fin)";
            return await _connection.ExecuteAsync(sql, periodo);

        }

        // Actualizar periodo existente
        public async Task<int> UpdateAsync(Periodo periodo)
        {
            string sql = @"UPDATE Periodo SET Anno=@Anno, Numero=@Numero,
                           Fecha_Inicio=@Fecha_Inicio, Fecha_Fin=@Fecha_Fin
                           WHERE ID_Periodo=@ID_Periodo";
            return await _connection.ExecuteAsync(sql, periodo);
        }

        public async Task<int> DeleteAsync(int id)
            => await _connection.ExecuteAsync("DELETE FROM Periodo WHERE ID_Periodo = @id", new { id });

        public async Task<bool> ExisteMismoAnioNumeroAsync(int anio, int numero, int? excluirId = null)
        {
            string sql = @"SELECT COUNT(1) 
                   FROM Periodo 
                   WHERE Anno = @anio AND Numero = @numero
                     AND (@excluirId IS NULL OR ID_Periodo <> @excluirId)";
            return await _connection.ExecuteScalarAsync<int>(sql, new { anio, numero, excluirId }) > 0;
        }

        public async Task<bool> ExisteSolapamientoEnAnioAsync(int anio, DateTime ini, DateTime fin, int? excluirId = null)
        {
            string sql = @"
        SELECT COUNT(1)
        FROM Periodo
        WHERE Anno = @anio
          AND (@excluirId IS NULL OR ID_Periodo <> @excluirId)
          AND NOT (Fecha_Fin < @ini OR Fecha_Inicio > @fin)";
            return await _connection.ExecuteScalarAsync<int>(sql, new { anio, ini, fin, excluirId }) > 0;
        }
    }
}
