using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class MateriasMayorCancelacionEndpoint
{
    public static void MapMateriasMayorCancelacion(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/materias-mayor-cancelacion", HandleAsync)
            .Produces<List<MateriaCancelacionReporteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var rawSesiones = await dbContext.Sesiones
            .AsNoTracking()
            .Select(s => new
            {
                s.MateriaId,
                NombreMateria = s.Materia.Nombre,
                Estado = s.Estado.Nombre
            })
            .ToListAsync(cancellationToken);

        var report = rawSesiones
            .GroupBy(s => new
            {
                s.MateriaId,
                s.NombreMateria
            })
            .Select(g =>
            {
                var total = g.Count();
                var canceladas = g.Count(x => x.Estado.StartsWith("CANCELADA"));
                var atendidas = g.Count(x => x.Estado == "ATENDIDA");
                var pendientes = g.Count(x => x.Estado == "PENDIENTE");
                var tasa = total > 0
                    ? Math.Round((double)canceladas / total * 100, 2)
                    : 0.0;

                return new MateriaCancelacionReporteDto(
                    g.Key.MateriaId,
                    g.Key.NombreMateria,
                    TotalSesiones: total,
                    SesionesCanceladas: canceladas,
                    SesionesAtendidas: atendidas,
                    SesionesPendientes: pendientes,
                    TasaCancelacion: tasa
                );
            })
            .Where(r => r.SesionesCanceladas > 0)
            .OrderByDescending(r => r.TasaCancelacion)
            .ThenByDescending(r => r.SesionesCanceladas)
            .ThenByDescending(r => r.TotalSesiones)
            .ThenBy(r => r.NombreMateria)
            .ToList();

        var take = limit.HasValue && limit.Value > 0 ? limit.Value : 10;
        report = report.Take(take).ToList();

        return Results.Ok(report);
    }
}
