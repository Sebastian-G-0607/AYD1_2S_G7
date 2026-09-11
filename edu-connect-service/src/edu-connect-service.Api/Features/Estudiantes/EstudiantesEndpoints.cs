using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;
using edu_connect_service.Api.Features.Estudiantes.HistorialSesiones;
using edu_connect_service.Api.Features.Estudiantes.Perfil;
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
        apiGroup.MapPerfilEstudiante();
        apiGroup.MapObtenerSesionesActivasEstudiante();
        apiGroup.MapCancelarSesionEstudianteSubruta();

        var rootGroup = app.MapGroup("/estudiantes");

        rootGroup.MapRegistrarEstudiante();
        rootGroup.MapHistorialSesionesEstudiante();
        rootGroup.MapPerfilEstudiante();
        rootGroup.MapObtenerSesionesActivasEstudiante();
        rootGroup.MapCancelarSesionEstudianteSubruta();
    }
}