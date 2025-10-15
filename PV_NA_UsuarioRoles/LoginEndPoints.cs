using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
//using PV_NA_UsuariosRoles.DTOs;
using PV_NA_UsuariosRoles.Services;

namespace PV_NA_UsuariosRoles.Controllers
{
    /// <summary>
    /// Controlador para gestionar la autenticación de usuarios
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<LoginController> _logger;

        /// <summary>
        /// Constructor del controlador de autenticación
        /// </summary>
        /// <param name="authService">Servicio de autenticación</param>
        /// <param name="logger">Logger para registro de eventos</param>
        public LoginController(AuthService authService, ILogger<LoginController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia sesión y genera los tokens JWT (access y refresh).
        /// </summary>
        /// <param name="usuario">Correo electrónico del usuario.</param>
        /// <param name="contrasena">Contraseña del usuario.</param>
        /// <returns>
        /// 201 Created: Tokens generados exitosamente con access token, refresh token y fecha de expiración.
        /// 400 Bad Request: Cuando faltan los encabezados de usuario o contraseña.
        /// 401 Unauthorized: Cuando las credenciales son incorrectas.
        /// 500 Internal Server Error: Cuando ocurre un error inesperado en el servidor.
        /// </returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromHeader(Name = "usuario")] string usuario,
            [FromHeader(Name = "contrasena")] string contrasena)
        {
            // Validar que los parámetros obligatorios estén presentes
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
                return BadRequest(new { message = "Debe incluir los encabezados 'usuario' y 'contrasena'." });

            try
            {
                // Intentar autenticar al usuario con las credenciales proporcionadas
                var result = await _authService.LoginAsync(usuario, contrasena);

                // Si la autenticación falla, retornar error 401
                if (result == null)
                    return Unauthorized(new { message = "Usuario y/o contraseña incorrectos." });

                // Retornar 201 Created con los tokens generados
                return Created("", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de login para usuario {Usuario}", usuario);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }

        }

        /// <summary>
        /// Genera nuevos tokens a partir de un refresh_token válido.
        /// </summary>
        /// <param name="request">Objeto que contiene el refresh token actual.</param>
        /// <returns>
        /// 201 Created: Nuevos tokens generados exitosamente (access_token y refresh_token).
        /// 400 Bad Request: Cuando no se proporciona el refresh token.
        /// 401 Unauthorized: Cuando el refresh token es inválido o ha expirado.
        /// </returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            // Validar que se haya proporcionado un refresh token
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Debe enviar el refresh_token" });

            // Generar nuevos tokens usando el refresh token proporcionado
            var result = await _authService.RefreshAsync(request.RefreshToken);

            // Si el refresh token es inválido, retornar error 401
            if (result == null)
                return Unauthorized(new { message = "Token inválido o expirado" });

            // Retornar 201 Created con los nuevos tokens
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Valida si un access_token sigue siendo válido.
        /// </summary>
        /// <param name="token">Access token a validar.</param>
        /// <returns>
        /// 200 OK: Token válido con valor true.
        /// 400 Bad Request: Cuando no se proporciona el token.
        /// 401 Unauthorized: Cuando el token es inválido o ha expirado.
        /// 500 Internal Server Error: Cuando ocurre un error inesperado en el servidor.
        /// </returns>
        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Validate([FromQuery] string token)
        {
            // Validar que se haya proporcionado un token
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Debe enviar el parámetro 'token'." });

            try
            {
                // Validar el token usando el servicio de autenticación
                bool valido = _authService.Validate(token);

                // Si el token no es válido, retornar error 401
                if (!valido)
                    return Unauthorized(new { message = "Token inválido o expirado." });

                // Retornar 200 OK indicando que el token es válido
                return Ok(true);
            }
            catch (Exception ex)
            {
                // Registrar error en logs y retornar error 500
                _logger.LogError(ex, "Error en la validación del token.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno del servidor." });
            }
        }
    }
}