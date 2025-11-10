using System.Data;
namespace PV_NA_OfertaAcademica.Repository
{
    // Conexion a la base de datos
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}