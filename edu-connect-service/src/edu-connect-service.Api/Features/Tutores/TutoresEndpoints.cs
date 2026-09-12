using edu_connect_service.Api.Features.Tutores.ConfigurarHorario;
using edu_connect_service.Api.Features.Tutores.RegistrarTutor;
using edu_connect_service.Api.Features.Tutores.ExplorarTutores;
using edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;
using edu_connect_service.Api.Features.Tutores.HistorialSesiones;
using edu_connect_service.Api.Features.Tutores.Perfil;
using edu_connect_service.Api.Features.Tutores.EstadisticasDashboard;

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

        var rootGroup = app.MapGroup("/tutores");

        rootGroup.MapRegistrarTutor();
        rootGroup.MapConfigurarHorario();
        rootGroup.MapObtenerHorario();
        rootGroup.MapExplorarTutores();
        rootGroup.MapConsultarDisponibilidad();
        rootGroup.MapHistorialSesiones();
        rootGroup.MapPerfilTutor();
        rootGroup.MapEstadisticasDashboard();

        var singularGroup = app.MapGroup("/tutor");
        singularGroup.MapEstadisticasDashboard();

        var apiSingularGroup = app.MapGroup("/api/tutor");
        apiSingularGroup.MapEstadisticasDashboard();
    }
}