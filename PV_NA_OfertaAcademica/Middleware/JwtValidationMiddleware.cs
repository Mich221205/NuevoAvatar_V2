/*using PV_NA_OfertaAcademica.Helpers;
/*
namespace PV_NA_OfertaAcademica.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }//

        public async Task InvokeAsync(HttpContext context, TokenValidator tokenValidator)
        {
            // Solo proteger endpoints del módulo ACD1
            if (context.Request.Path.StartsWithSegments("/institucion"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                var token = authHeader.Replace("Bearer ", "");

                bool valido = await tokenValidator.ValidateAsync(token);

                if (!valido)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido o expirado");
                    return;
                }
            }

            await _next(context);
        }//
    }//

    public static class JwtValidationExtensions
    {
        public static IApplicationBuilder UseJwtValidation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtValidationMiddleware>();
        }
    }//

}

/*
 Intercepta cada request al endpoint /institucion.

Busca el header Authorization.

Valida el token mediante el helper.

Si no es válido → corta el pipeline con 401.
 
 
 */