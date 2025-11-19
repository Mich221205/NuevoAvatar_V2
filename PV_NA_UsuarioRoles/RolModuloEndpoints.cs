using Microsoft.AspNetCore.Mvc;
using PV_NA_UsuariosRoles.Entities;
using PV_NA_UsuariosRoles.Services;

namespace PV_NA_UsuariosRoles.Endpoints
{
    public static class RolModuloEndpoints
    {
        public static void MapRolModuloEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/rolmodulo").RequireAuthorization();

            group.MapGet("/{idRol:int}", async (int idRol, IRolModuloService service) =>
            {
                var data = await service.GetByRolAsync(idRol);
                return Results.Ok(data);
            });

            group.MapPut("/{idRol:int}", async (int idRol, List<RolModulo> permisos, IRolModuloService service) =>
            {
                await service.GuardarAsync(idRol, permisos);
                return Results.Ok(new { message = "Permisos actualizados" });
            });
        }
    }
}
