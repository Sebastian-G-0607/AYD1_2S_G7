using edu_connect_service.Api.Features.Administrador.GestionEstudiantes;
using edu_connect_service.Api.Features.Administrador.GestionTutores;
using edu_connect_service.Api.Shared.Authorization;

namespace edu_connect_service.Api.Features.Administrador;

public static class AdministradorEndpoints
{
    public static void MapAdministrador(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        // Gestión de Estudiantes (HU-05)
        apiGroup.MapListarEstudiantesPendientes();
        apiGroup.MapActualizarEstadoEstudiante();

        // Gestión de Tutores (HU-06)
        apiGroup.MapListarTutoresPendientes();
        apiGroup.MapActualizarEstadoTutor();

        var rootGroup = app.MapGroup("/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        rootGroup.MapListarEstudiantesPendientes();
        rootGroup.MapActualizarEstadoEstudiante();
        rootGroup.MapListarTutoresPendientes();
        rootGroup.MapActualizarEstadoTutor();
    }
}

