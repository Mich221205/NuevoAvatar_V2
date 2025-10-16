using System.Data;

namespace PV_NA_Matricula.Repository
{
	public interface IDbConnectionFactory
	{
		Task<IDbConnection> CreateConnectionAsync();
	} 
}
