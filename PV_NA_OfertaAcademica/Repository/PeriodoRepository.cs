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
    }
}
