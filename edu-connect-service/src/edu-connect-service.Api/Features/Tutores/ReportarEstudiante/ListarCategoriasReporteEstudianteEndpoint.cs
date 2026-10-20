using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ReportarEstudiante;

public static class ListarCategoriasReporteEstudianteEndpoint
{
    public static void MapListarCategoriasReporteEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes-estudiante/categorias", HandleAsync)
            .RequireAuthorization()
            .Produces<List<CategoriaReporteEstudianteResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    public static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var categorias = await dbContext.CategoriasReportesEstudiantes
            .OrderBy(categoria => categoria.Nombre)
            .Select(categoria => new CategoriaReporteEstudianteResponse(
                categoria.Id,
                categoria.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(categorias);
    }
}