using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;
using edu_connect_service.Api.Features.Sesiones.ObtenerSesionesActivas;
using edu_connect_service.Api.Features.Sesiones.CancelarSesionEstudiante;

namespace edu_connect_service.Api.Features.Estudiantes;

public static class EstudiantesEndpoints
{
    public static void MapEstudiantes(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/estudiantes");
        apiGroup.MapRegistrarEstudiante();
        apiGroup.MapObtenerSesionesActivasEstudiante();
        apiGroup.MapCancelarSesionEstudianteSubruta();

        var rootGroup = app.MapGroup("/estudiantes");
        rootGroup.MapRegistrarEstudiante();
        rootGroup.MapObtenerSesionesActivasEstudiante();
        rootGroup.MapCancelarSesionEstudianteSubruta();
    }
}
