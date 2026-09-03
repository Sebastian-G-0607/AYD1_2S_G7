using edu_connect_service.Api.Features.Tutores.ConfigurarHorario;
using edu_connect_service.Api.Features.Tutores.RegistrarTutor;
using edu_connect_service.Api.Features.Tutores.ExplorarTutores;
using edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;

namespace edu_connect_service.Api.Features.Tutores;

public static class TutoresEndpoints
{
    public static void MapTutores(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/tutores");
        apiGroup.MapRegistrarTutor();
        apiGroup.MapConfigurarHorario();
        apiGroup.MapExplorarTutores();
        apiGroup.MapConsultarDisponibilidad();

        var rootGroup = app.MapGroup("/tutores");
        rootGroup.MapRegistrarTutor();
        rootGroup.MapConfigurarHorario();
        rootGroup.MapExplorarTutores();
        rootGroup.MapConsultarDisponibilidad();
    
    }
}