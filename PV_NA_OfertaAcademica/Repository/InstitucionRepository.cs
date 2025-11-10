using Dapper;
using Microsoft.Data.SqlClient;
using PV_NA_OfertaAcademica.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PV_NA_OfertaAcademica.Repository
{
    public class InstitucionRepository : IInstitucionRepository
    {
        private readonly IDbConnection _connection;

        public InstitucionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Institucion>> GetAllAsync()
        {
            return await _connection.QueryAsync<Institucion>("SELECT * FROM Institucion");
        }

        public async Task<Institucion?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Institucion>(
                "SELECT * FROM Institucion WHERE ID_Institucion = @id", new { id });
        }

        public async Task<int> CreateAsync(Institucion institucion)
        {
            var sql = "INSERT INTO Institucion (Nombre) VALUES (@Nombre); SELECT SCOPE_IDENTITY();";
            return await _connection.ExecuteScalarAsync<int>(sql, institucion);
        }

        public async Task<bool> UpdateAsync(Institucion institucion)
        {
            var sql = "UPDATE Institucion SET Nombre = @Nombre WHERE ID_Institucion = @ID_Institucion";
            return await _connection.ExecuteAsync(sql, institucion) > 0;
        }
        /*
        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Institucion WHERE ID_Institucion = @id";
            return await _connection.ExecuteAsync(sql, new { id }) > 0;
        }*/

        public async Task<bool> DeleteAsync(int id)
        {
            var tieneCarreras = await _connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Carrera WHERE ID_Institucion = @id", new { id });

            if (tieneCarreras > 0)
                throw new InvalidOperationException("No se puede eliminar la institución porque tiene carreras asociadas.");

            var sql = "DELETE FROM Institucion WHERE ID_Institucion = @id";
            return await _connection.ExecuteAsync(sql, new { id }) > 0;
        }


    }
}  


