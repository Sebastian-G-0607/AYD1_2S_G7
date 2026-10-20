using edu_connect_service.Api.Features.Tutores.ConfigurarHorario;
using edu_connect_service.Api.Features.Tutores.RegistrarTutor;
using edu_connect_service.Api.Features.Tutores.ExplorarTutores;
using edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;
using edu_connect_service.Api.Features.Tutores.HistorialSesiones;
using edu_connect_service.Api.Features.Tutores.Perfil;
using edu_connect_service.Api.Features.Tutores.EstadisticasDashboard;
using edu_connect_service.Api.Features.Tutores.CalificarEstudiante;
using edu_connect_service.Api.Features.Tutores.ReportarEstudiante;

namespace edu_connect_service.Api.Features.Tutores;

public static class TutoresEndpoints
{
    public static void MapTutores(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/tutores");

        apiGroup.MapRegistrarTutor();
        apiGroup.MapConfigurarHorario();
        apiGroup.MapObtenerHorario();
        apiGroup.MapExplorarTutores();
        apiGroup.MapConsultarDisponibilidad();
        apiGroup.MapHistorialSesiones();
        apiGroup.MapPerfilTutor();
        apiGroup.MapEstadisticasDashboard();
        apiGroup.MapCalificarEstudiante();
        apiGroup.MapListarCategoriasReporteEstudiante();
        apiGroup.MapReportarEstudiante();

        var rootGroup = app.MapGroup("/tutores");

        rootGroup.MapRegistrarTutor();
        rootGroup.MapConfigurarHorario();
        rootGroup.MapObtenerHorario();
        rootGroup.MapExplorarTutores();
        rootGroup.MapConsultarDisponibilidad();
        rootGroup.MapHistorialSesiones();
        rootGroup.MapPerfilTutor();
        rootGroup.MapEstadisticasDashboard();
        rootGroup.MapCalificarEstudiante();
        rootGroup.MapListarCategoriasReporteEstudiante();
        rootGroup.MapReportarEstudiante();

        var singularGroup = app.MapGroup("/tutor");
        singularGroup.MapEstadisticasDashboard();

        var apiSingularGroup = app.MapGroup("/api/tutor");
        apiSingularGroup.MapEstadisticasDashboard();
    }
}