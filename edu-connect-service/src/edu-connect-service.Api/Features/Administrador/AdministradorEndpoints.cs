using edu_connect_service.Api.Features.Administrador.GestionEstudiantes;
using edu_connect_service.Api.Features.Administrador.GestionTutores;
using edu_connect_service.Api.Features.Administrador.Reportes;
using edu_connect_service.Api.Shared.Authorization;

namespace edu_connect_service.Api.Features.Administrador;

public static class AdministradorEndpoints
{
    public static void MapAdministrador(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        // Gestión de Estudiantes Pendientes (HU-05)
        apiGroup.MapListarEstudiantesPendientes();
        apiGroup.MapActualizarEstadoEstudiante();

        // Gestión de Estudiantes Activos (HU-07)
        apiGroup.MapListarEstudiantesActivos();
        apiGroup.MapDarBajaEstudiante();

        // Gestión de Tutores Pendientes (HU-06)
        apiGroup.MapListarTutoresPendientes();
        apiGroup.MapActualizarEstadoTutor();

        // Gestión de Tutores Activos (HU-07)
        apiGroup.MapListarTutoresActivos();
        apiGroup.MapDarBajaTutor();

        // Reportes del Sistema (HU-08)
        apiGroup.MapReportes();

        var rootGroup = app.MapGroup("/administrador")
            .RequireAuthorization(p => p.RequireRole(AppRoles.Administrador));

        rootGroup.MapListarEstudiantesPendientes();
        rootGroup.MapActualizarEstadoEstudiante();
        rootGroup.MapListarEstudiantesActivos();
        rootGroup.MapDarBajaEstudiante();

        rootGroup.MapListarTutoresPendientes();
        rootGroup.MapActualizarEstadoTutor();
        rootGroup.MapListarTutoresActivos();
        rootGroup.MapDarBajaTutor();

        // Reportes del Sistema (HU-08)
        rootGroup.MapReportes();
    }
}

