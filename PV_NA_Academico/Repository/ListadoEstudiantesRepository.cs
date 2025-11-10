using Dapper;
using PV_NA_Academico.Entities;
using System.Data;

namespace PV_NA_Academico.Repository
{
    public class ListadoEstudiantesRepository
    {
        private readonly IDbConnection _connection;
        public ListadoEstudiantesRepository(IDbConnection connection) => _connection = connection;

        public async Task<IEnumerable<ListadoEstudiante>> ObtenerPorPeriodoAsync(string periodo)
        {
            const string sql = @"SELECT Periodo, TipoIdentificacion, Identificacion, NombreCompleto,Carrera, Curso, Grupo FROM ListadoEstudiantesPeriodo WHERE Periodo = @Periodo ORDER BY NombreCompleto";

            return await _connection.QueryAsync<ListadoEstudiante>(sql, new { Periodo = periodo });
        }
    }
}
