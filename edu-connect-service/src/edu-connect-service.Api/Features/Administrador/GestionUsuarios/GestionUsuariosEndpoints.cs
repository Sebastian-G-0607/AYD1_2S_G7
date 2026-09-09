using edu_connect_service.Api.Shared.Authorization;

namespace edu_connect_service.Api.Features.Administrador.GestionUsuarios;

public static class GestionUsuariosEndpoints
{
    public static void MapGestionUsuarios(this IEndpointRouteBuilder app)
    {
        app.MapListarUsuariosDadosDeBaja();
    }
}
