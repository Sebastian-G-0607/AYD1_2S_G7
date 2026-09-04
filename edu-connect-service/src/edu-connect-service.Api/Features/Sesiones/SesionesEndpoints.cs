using edu_connect_service.Api.Features.Sesiones.ProgramarSesion;

namespace edu_connect_service.Api.Features.Sesiones;

public static class SesionesEndpoints
{
    public static void MapSesiones(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/sesiones");
        apiGroup.MapProgramarSesion();

        var rootGroup = app.MapGroup("/sesiones");
        rootGroup.MapProgramarSesion();
    }
}