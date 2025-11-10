using Dapper;
using PV_NA_OfertaAcademica.Entities;
using System.Data;

namespace PV_NA_OfertaAcademica.Repository
{
    public class CarreraRepository : ICarreraRepository
    {
        private readonly IDbConnection _connection;

        public CarreraRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // Obtener todas las carreras
        public async Task<IEnumerable<Carrera>> GetAllAsync()
            => await _connection.QueryAsync<Carrera>("SELECT * FROM Carrera");

        //  Obtener carrera por ID
        public async Task<Carrera?> GetByIdAsync(int id)
            => await _connection.QueryFirstOrDefaultAsync<Carrera>(
                "SELECT * FROM Carrera WHERE ID_Carrera = @id", new { id });

        //  Obtener carreras por institución
        public async Task<IEnumerable<Carrera>> GetByInstitucionAsync(int idInstitucion)
            => await _connection.QueryAsync<Carrera>(
                "SELECT * FROM Carrera WHERE ID_Institucion = @idInstitucion", new { idInstitucion });

        //  Crear carrera
        public async Task<int> CreateAsync(Carrera carrera)
            => await _connection.ExecuteAsync(
                @"INSERT INTO Carrera (Nombre, ID_Institucion, ID_Profesor_Director) 
                  VALUES (@Nombre, @ID_Institucion, @ID_Profesor_Director)", carrera);

        //  Actualizar carrera
        public async Task<int> UpdateAsync(Carrera carrera)
            => await _connection.ExecuteAsync(
                @"UPDATE Carrera 
                  SET Nombre = @Nombre, 
                      ID_Institucion = @ID_Institucion, 
                      ID_Profesor_Director = @ID_Profesor_Director 
                  WHERE ID_Carrera = @ID_Carrera", carrera);

        //  Eliminar carrera
        public async Task<int> DeleteAsync(int id)
            => await _connection.ExecuteAsync(
                "DELETE FROM Carrera WHERE ID_Carrera = @id", new { id });
    }
}
