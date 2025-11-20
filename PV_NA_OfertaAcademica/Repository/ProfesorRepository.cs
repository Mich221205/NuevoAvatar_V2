using Dapper;
using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Repository
{
    public class ProfesorRepository : IProfesorRepository
    {
        private readonly IDbConnectionFactory _factory;
        public ProfesorRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<Profesor>> GetAllAsync()
        {
            using var conn = await _factory.CreateConnectionAsync();
            return await conn.QueryAsync<Profesor>("SELECT * FROM Profesor");
        }

        public async Task<Profesor?> GetByIdAsync(int id)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = "SELECT * FROM Profesor WHERE ID_Profesor = @id";
            return await conn.QueryFirstOrDefaultAsync<Profesor>(sql, new { id });
        }

        public async Task<int> InsertAsync(Profesor profesor)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = @"INSERT INTO Profesor (Identificacion, Tipo_Identificacion, Email, Nombre, Fecha_Nacimiento)
                    VALUES (@Identificacion, @Tipo_Identificacion, @Email, @Nombre, @Fecha_Nacimiento)";
            return await conn.ExecuteAsync(sql, profesor);
        }

        public async Task<int> UpdateAsync(Profesor profesor)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = @"UPDATE Profesor SET 
                    Identificacion=@Identificacion, 
                    Tipo_Identificacion=@Tipo_Identificacion, 
                    Email=@Email, 
                    Nombre=@Nombre, 
                    Fecha_Nacimiento=@Fecha_Nacimiento
                    WHERE ID_Profesor=@ID_Profesor";
            return await conn.ExecuteAsync(sql, profesor);
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = "DELETE FROM Profesor WHERE ID_Profesor=@id";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<bool> ExisteIdentificacionAsync(string identificacion)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM Profesor WHERE Identificacion=@identificacion";
            return await conn.ExecuteScalarAsync<int>(sql, new { identificacion }) > 0;
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            using var conn = await _factory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM Profesor WHERE Email=@email";
            return await conn.ExecuteScalarAsync<int>(sql, new { email }) > 0;
        }

        public async Task<bool> TieneGruposActivosAsync(int idProfesor)
        {
            using var conn = await _factory.CreateConnectionAsync();

            var sql = @"
                SELECT COUNT(1)
                FROM Grupo g
                INNER JOIN Periodo p ON g.ID_Periodo = p.ID_Periodo
                WHERE g.ID_Profesor = @idProfesor
                  AND GETDATE() BETWEEN p.Fecha_Inicio AND p.Fecha_Fin;";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { idProfesor });
            return count > 0;
        }

        public async Task<bool> EsDirectorDeAlgunaCarreraAsync(int idProfesor)
        {
            using var conn = await _factory.CreateConnectionAsync();

            var sql = @"
        SELECT COUNT(*)
        FROM Carrera
        WHERE ID_Profesor_Director = @idProfesor";

            var cantidad = await conn.ExecuteScalarAsync<int>(sql, new { idProfesor });
            return cantidad > 0;
        }
    }

}
