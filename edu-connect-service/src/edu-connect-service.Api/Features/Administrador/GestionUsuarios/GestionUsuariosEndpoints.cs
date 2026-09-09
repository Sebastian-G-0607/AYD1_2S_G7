using edu_connect_service.Api.Shared.Authorization;

namespace edu_connect_service.Api.Features.Administrador.GestionUsuarios;

public static class GestionUsuariosEndpoints
{
    public static void MapGestionUsuarios(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        apiGroup.MapListarUsuariosDadosDeBaja();

        var rootGroup = app.MapGroup("/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        rootGroup.MapListarUsuariosDadosDeBaja();
    }
}
