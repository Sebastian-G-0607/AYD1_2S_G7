using edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;

namespace edu_connect_service.Api.Features.Estudiantes;

public static class EstudiantesEndpoints
{
    public static void MapEstudiantes(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/estudiantes");
        apiGroup.MapRegistrarEstudiante();

        var rootGroup = app.MapGroup("/estudiantes");
        rootGroup.MapRegistrarEstudiante();
    }
}
