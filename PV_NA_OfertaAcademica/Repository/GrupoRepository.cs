  using Dapper;
using PV_NA_OfertaAcademica.Entities;
using System.Data;

namespace PV_NA_OfertaAcademica.Repository
{
    public class GrupoRepository
    {
        private readonly IDbConnection _connection;
        // Constructor que recibe la conexión a la base de datos
        public GrupoRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // get todos los grupos
        public async Task<IEnumerable<Grupo>> GetAllAsync()
        {
            string sql = "SELECT * FROM Grupo";
            return await _connection.QueryAsync<Grupo>(sql);
        }

        // get grupo por id
        public async Task<Grupo?> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM Grupo WHERE ID_Grupo = @ID";
            return await _connection.QueryFirstOrDefaultAsync<Grupo>(sql, new { ID = id });
        }

        // Create nuevo grupo  
        public async Task<int> CreateAsync(Grupo grupo)
        {
            string sql = @"INSERT INTO Grupo (Numero, ID_Curso, ID_Profesor, Horario, ID_Periodo)
                   VALUES (@Numero, @ID_Curso, @ID_Profesor, @Horario, @ID_Periodo)";
            return await _connection.ExecuteAsync(sql, new
            {
                Numero = grupo.Numero_Grupo,  // MAPEO DE LA VARIABLE Numero_Grupo A Numero
                grupo.ID_Curso,
                grupo.ID_Profesor,
                grupo.Horario, 
                grupo.ID_Periodo
            });
        }

        // update grupo
        public async Task<int> UpdateAsync(Grupo grupo)
        {
            string sql = @"UPDATE Grupo SET 
                    Numero = @Numero,
                    ID_Curso = @ID_Curso,
                    ID_Profesor = @ID_Profesor,
                    Horario = @Horario,
                    ID_Periodo = @ID_Periodo
                   WHERE ID_Grupo = @ID_Grupo";

            return await _connection.ExecuteAsync(sql, new
            {
                Numero = grupo.Numero_Grupo,   // mapeo de numero_grupo
                grupo.ID_Curso,
                grupo.ID_Profesor,
                grupo.Horario,
                grupo.ID_Periodo,
                grupo.ID_Grupo
            });
        }

        // delete grupo
        public async Task<int> DeleteAsync(int id)
        {
            string sql = "DELETE FROM Grupo WHERE ID_Grupo = @ID";
            return await _connection.ExecuteAsync(sql, new { ID = id });
        }
    }// 
}
