using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;
using edu_connect_service.Api.Features.Estudiantes.HistorialSesiones;

namespace edu_connect_service.Api.Features.Estudiantes;

public static class EstudiantesEndpoints
{
    public static void MapEstudiantes(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/estudiantes");
        apiGroup.MapRegistrarEstudiante();
        apiGroup.MapHistorialSesionesEstudiante();

        var rootGroup = app.MapGroup("/estudiantes");
        rootGroup.MapRegistrarEstudiante();
        rootGroup.MapHistorialSesionesEstudiante();
    }
}