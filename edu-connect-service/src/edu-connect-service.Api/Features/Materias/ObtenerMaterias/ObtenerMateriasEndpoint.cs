using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Materias.ObtenerMaterias;

public static class ObtenerMateriasEndpoint
{
    public static void MapObtenerMaterias(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .Produces<List<MateriaResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] string? search,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Materias.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmedSearch = search.Trim();
            query = query.Where(m => EF.Functions.Like(m.Nombre, $"%{trimmedSearch}%"));
        }

        var materias = await query
            .OrderBy(m => m.Id)
            .Select(m => new MateriaResponseDto(m.Id, m.Nombre))
            .ToListAsync(cancellationToken);

        return Results.Ok(materias);
    }
}
