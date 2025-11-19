using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    public class RolModuloRepository
    {
        private readonly IDbConnectionFactory _db;

        public RolModuloRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RolModulo>> GetByRolAsync(int idRol)
        {
            using var conn = _db.CreateConnection();
            string sql = "SELECT * FROM RolModulo WHERE ID_Rol = @idRol";
            return await conn.QueryAsync<RolModulo>(sql, new { idRol });
        }

        public async Task GuardarAsync(int idRol, List<RolModulo> permisos)
        {
            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync("DELETE FROM RolModulo WHERE ID_Rol = @idRol", new { idRol });

            string insert = @"
                INSERT INTO RolModulo (ID_Rol, ID_Modulo, PuedeVer)
                VALUES (@ID_Rol, @ID_Modulo, @PuedeVer)";

            foreach (var p in permisos)
            {
                await conn.ExecuteAsync(insert, p);
            }
        }
    }
}
