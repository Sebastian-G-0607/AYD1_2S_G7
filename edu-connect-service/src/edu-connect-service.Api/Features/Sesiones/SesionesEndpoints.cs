using edu_connect_service.Api.Features.Sesiones.ProgramarSesion;
using edu_connect_service.Api.Features.Sesiones.GestionarPendientes;
using edu_connect_service.Api.Features.Sesiones.CancelarSesion;

namespace edu_connect_service.Api.Features.Sesiones;

public static class SesionesEndpoints
{
    public static void MapSesiones(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/sesiones");
        apiGroup.MapProgramarSesion();
        apiGroup.MapListarPendientes();
        apiGroup.MapAtenderSesion();
        apiGroup.MapCancelarSesion();

        var rootGroup = app.MapGroup("/sesiones");
        rootGroup.MapProgramarSesion();
        rootGroup.MapListarPendientes();
        rootGroup.MapAtenderSesion();
        rootGroup.MapCancelarSesion();
    }
}