using Dapper;
using PV_NA_Academico.Entities;
using System.Data;

namespace PV_NA_Academico.Repository
{
    public class HistorialRepository
    {
        private readonly IDbConnection _connection;

        public HistorialRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<HistorialAcademico>> ObtenerHistorialAsync(string tipoIdentificacion, string identificacion)
        {
            string sql = @"SELECT CodigoCurso, NombreCurso,Promedio,Periodo FROM HistorialAcademico WHERE TipoIdentificacion = @TipoIdentificacion AND Identificacion = @Identificacion";
            return await _connection.QueryAsync<HistorialAcademico>(sql, new
            {
                TipoIdentificacion = tipoIdentificacion,
                Identificacion = identificacion
            });
        }
    }
}
