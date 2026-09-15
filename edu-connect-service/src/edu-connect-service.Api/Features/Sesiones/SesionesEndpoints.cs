using edu_connect_service.Api.Features.Sesiones.ProgramarSesion;
using edu_connect_service.Api.Features.Sesiones.GestionarPendientes;
using edu_connect_service.Api.Features.Sesiones.CancelarSesion;
using edu_connect_service.Api.Features.Sesiones.ObtenerSesionesActivas;
using edu_connect_service.Api.Features.Sesiones.CancelarSesionEstudiante;

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
        apiGroup.MapObtenerSesionesActivas();
        apiGroup.MapCancelarSesionEstudiante();

        var rootGroup = app.MapGroup("/sesiones");
        rootGroup.MapProgramarSesion();
        rootGroup.MapListarPendientes();
        rootGroup.MapAtenderSesion();
        rootGroup.MapCancelarSesion();
        rootGroup.MapObtenerSesionesActivas();
        rootGroup.MapCancelarSesionEstudiante();
    }
}