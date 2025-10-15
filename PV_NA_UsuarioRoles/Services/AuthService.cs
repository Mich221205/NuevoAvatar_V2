using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Repository;

namespace PV_NA_UsuariosRoles.Services
{
    public class AuthService
    {
        private readonly TokenService _tokenService;
        private readonly TokenOptions _opts;
        private readonly UsuarioRepository _usuarioRepo;
        private readonly SesionRepository _sesionRepo;
        private readonly ILogger<AuthService> _logger; //Inyectar logger

        public AuthService(
            TokenService tokenService,
            IOptions<TokenOptions> opts,
            UsuarioRepository usuarioRepo,
            SesionRepository sesionRepo,
            ILogger<AuthService> logger)
        {
            _tokenService = tokenService;
            _opts = opts.Value;
            _usuarioRepo = usuarioRepo;
            _sesionRepo = sesionRepo;
            _logger = logger; //Inyectar logger
        }

        /// <summary>
        /// Autentica un usuario y genera tokens JWT
        /// </summary>
        public async Task<object?> LoginAsync(string email, string password)
        {
            var user = await _usuarioRepo.GetUsuarioByEmailAsync(email);
            if (user == null || user.Contrasena != password)
                return null;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID_Usuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.ID_Rol.ToString())
            };

            var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Eliminar sesiones previas del usuario
            await _sesionRepo.DeleteOldSessionsAsync(user.ID_Usuario);

            // Guardar nueva sesión en base de datos
            var sesion = new Sesion
            {
                ID_Usuario = user.ID_Usuario,
                Token_JWT = accessToken,
                Refresh_Token = refreshToken,
                Expira = DateTime.UtcNow.AddMinutes(_opts.RefreshTokenMinutes)
            };
            await _sesionRepo.SaveAsync(sesion);

            return new
            {
                expires_in = expiresAt,
                access_token = accessToken,
                refresh_token = refreshToken,
                usuarioID = user.ID_Usuario
            };
        }

        /// <summary>
        /// Renueva los tokens usando un refresh token válido
        /// </summary>
        public async Task<object?> RefreshAsync(string refreshToken)
        {
            var sesion = await _sesionRepo.GetByRefreshTokenAsync(refreshToken);
            if (sesion == null || sesion.Expira < DateTime.UtcNow)
                return null;

            var user = await _usuarioRepo.GetUsuarioByIdAsync(sesion.ID_Usuario);
            if (user == null) return null;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID_Usuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.ID_Rol.ToString())
            };

            var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(claims);
            var newRefresh = _tokenService.GenerateRefreshToken();

            // Rotar tokens: eliminar sesión anterior y crear nueva
            await _sesionRepo.DeleteOldSessionsAsync(user.ID_Usuario);
            await _sesionRepo.SaveAsync(new Sesion
            {
                ID_Usuario = user.ID_Usuario,
                Token_JWT = accessToken,
                Refresh_Token = newRefresh,
                Expira = DateTime.UtcNow.AddMinutes(_opts.RefreshTokenMinutes)
            });

            return new
            {
                expires_in = expiresAt,
                access_token = accessToken,
                refresh_token = newRefresh,
                usuarioID = user.ID_Usuario
            };
        }

        /// <summary>
        /// Valida si un token JWT es válido
        /// </summary>
        public bool Validate(string token)
        {
            return _tokenService.ValidateToken(token);
        }
    }
}