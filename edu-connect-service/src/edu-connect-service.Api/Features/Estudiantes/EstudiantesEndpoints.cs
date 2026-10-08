using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;
using edu_connect_service.Api.Features.Estudiantes.HistorialSesiones;
using edu_connect_service.Api.Features.Estudiantes.ImprimirPlanEstudio;
using edu_connect_service.Api.Features.Estudiantes.ObtenerPlanEstudio;
using edu_connect_service.Api.Features.Estudiantes.Perfil;
using edu_connect_service.Api.Features.Estudiantes.ReportarTutor;
using edu_connect_service.Api.Features.Sesiones.ObtenerSesionesActivas;
using edu_connect_service.Api.Features.Sesiones.CancelarSesionEstudiante;

namespace edu_connect_service.Api.Features.Estudiantes;

public static class EstudiantesEndpoints
{
    public static void MapEstudiantes(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/estudiantes");

        apiGroup.MapRegistrarEstudiante();
        apiGroup.MapHistorialSesionesEstudiante();
        apiGroup.MapImprimirPlanEstudio();
        apiGroup.MapObtenerPlanEstudio();
        apiGroup.MapPerfilEstudiante();
        apiGroup.MapObtenerSesionesActivasEstudiante();
        apiGroup.MapCancelarSesionEstudianteSubruta();
        apiGroup.MapReportarTutor();

        var rootGroup = app.MapGroup("/estudiantes");

        rootGroup.MapRegistrarEstudiante();
        rootGroup.MapHistorialSesionesEstudiante();
        rootGroup.MapImprimirPlanEstudio();
        rootGroup.MapObtenerPlanEstudio();
        rootGroup.MapPerfilEstudiante();
        rootGroup.MapObtenerSesionesActivasEstudiante();
        rootGroup.MapCancelarSesionEstudianteSubruta();
        rootGroup.MapReportarTutor();
    }
}