using Microsoft.Data.SqlClient;
using System.Data;

namespace PV_NA_OfertaAcademica.Repository
{
    // Implementación de la fábrica de conexiones a la base de datos
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            return connection;
        }
    }
}
