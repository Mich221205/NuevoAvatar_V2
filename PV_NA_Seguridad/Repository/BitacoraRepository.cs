using Dapper;
using System.Data;
using PV_NA_Seguridad.Entities;

namespace PV_NA_Seguridad.Repository
{
    public class BitacoraRepository
    {
        private readonly IDbConnection _connection;

        public BitacoraRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> InsertAsync(Bitacora bitacora)
        {
            string sql = @"INSERT INTO Bitacora (ID_Usuario, Accion) VALUES (@ID_Usuario, @Accion)";
            return await _connection.ExecuteAsync(sql, bitacora);
        }

        public async Task<IEnumerable<Bitacora>> GetAllAsync()
        {
            string sql = "SELECT * FROM Bitacora ORDER BY Fecha DESC";
            return await _connection.QueryAsync<Bitacora>(sql);
        }

        public async Task<IEnumerable<Bitacora>> GetByUsuarioAsync(int idUsuario)
        {
            string sql = @"SELECT * FROM Bitacora  WHERE ID_Usuario = @idUsuario ORDER BY Fecha DESC";
            return await _connection.QueryAsync<Bitacora>(sql, new { idUsuario });
        }
    }
}
