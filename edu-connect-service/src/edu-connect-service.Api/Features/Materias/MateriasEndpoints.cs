using edu_connect_service.Api.Features.Materias.ObtenerMaterias;

namespace edu_connect_service.Api.Features.Materias;

public static class MateriasEndpoints
{
    public static void MapMaterias(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api/materias");
        apiGroup.MapObtenerMaterias();

        var rootGroup = app.MapGroup("/materias");
        rootGroup.MapObtenerMaterias();
    }
}
