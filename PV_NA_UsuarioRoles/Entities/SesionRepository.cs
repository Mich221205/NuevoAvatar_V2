using Dapper;
using PV_NA_UsuariosRoles.Entities;
using System.Data;

namespace PV_NA_UsuariosRoles.Repository
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de base de datos relacionadas con las sesiones de usuarios
    /// </summary>
    public class SesionRepository
    {
        private readonly IDbConnectionFactory _factory;

        /// <summary>
        /// Constructor del repositorio de sesiones
        /// </summary>
        /// <param name="factory">Factory para crear conexiones a la base de datos</param>
        public SesionRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Obtiene una sesión basada en el refresh token proporcionado
        /// </summary>
        /// <param name="refreshToken">Token de refresco para buscar la sesión</param>
        /// <returns>
        /// Objeto Sesion si se encuentra una sesión con el refresh token, 
        /// null si no se encuentra ninguna sesión
        /// </returns>
        public async Task<Sesion?> GetByRefreshTokenAsync(string refreshToken)
        {
            // Crear una nueva conexión a la base de datos
            using var conn = _factory.CreateConnection();

            // Query SQL para buscar la sesión por refresh token
            string query = "SELECT * FROM Sesion WHERE Refresh_Token = @refreshToken";

            // Ejecutar la consulta y retornar el primer resultado o null si no existe
            return await conn.QueryFirstOrDefaultAsync<Sesion>(query, new { refreshToken });
        }

        /// <summary>
        /// Guarda una nueva sesión en la base de datos
        /// </summary>
        /// <param name="sesion">Objeto Sesion con la información a guardar</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        public async Task SaveAsync(Sesion sesion)
        {
            // Crear una nueva conexión a la base de datos
            using var conn = _factory.CreateConnection();

            // Query SQL para insertar una nueva sesión
            string sql = @"INSERT INTO Sesion (ID_Usuario, Token_JWT, Refresh_Token, Expira)
                           VALUES (@ID_Usuario, @Token_JWT, @Refresh_Token, @Expira)";

            // Ejecutar el comando INSERT con los parámetros del objeto sesion
            await conn.ExecuteAsync(sql, sesion);
        }

        /// <summary>
        /// Elimina todas las sesiones antiguas de un usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario cuyas sesiones se eliminarán</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        /// <remarks>
        /// Este método es útil para mantener la seguridad eliminando sesiones previas
        /// cuando un usuario inicia sesión nuevamente o para limpieza periódica
        /// </remarks>
        public async Task DeleteOldSessionsAsync(int userId)
        {
            // Crear una nueva conexión a la base de datos
            using var conn = _factory.CreateConnection();

            // Query SQL para eliminar todas las sesiones del usuario
            string sql = "DELETE FROM Sesion WHERE ID_Usuario = @userId";

            // Ejecutar el comando DELETE con el parámetro userId
            await conn.ExecuteAsync(sql, new { userId });
        }
    }
}