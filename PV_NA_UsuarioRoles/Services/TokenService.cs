using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PV_NA_UsuariosRoles.Entities;

namespace PV_NA_UsuariosRoles.Services
{
    public class TokenService
    {
        private readonly TokenOptions _options;

        public TokenService(IOptions<TokenOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Genera un token JWT de acceso con los claims proporcionados
        /// </summary>
        public (string token, DateTime expiresAt) GenerateAccessToken(IEnumerable<Claim> claims)
        {  //validar que el secret este bien configurado
            if (string.IsNullOrEmpty(_options.Secret) || _options.Secret.Length < 16)
                throw new InvalidOperationException("JWT Secret no configurado correctamente");
           
            //Validar configuración 
            if (string.IsNullOrEmpty(_options.Issuer))
                throw new InvalidOperationException("JWT Issuer no configurado correctamente");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expires);
        }

        /// <summary>
        /// Genera un token de refresco criptográficamente seguro
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Valida la firma, emisor, audiencia y expiración de un token JWT
        /// </summary>
        public bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));

            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _options.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}