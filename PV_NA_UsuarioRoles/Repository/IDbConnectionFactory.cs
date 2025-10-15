using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

