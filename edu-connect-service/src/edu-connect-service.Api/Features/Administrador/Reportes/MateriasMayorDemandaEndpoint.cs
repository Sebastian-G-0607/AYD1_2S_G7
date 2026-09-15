using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class MateriasMayorDemandaEndpoint
{
    public static void MapMateriasMayorDemanda(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/materias-mayor-demanda", HandleAsync)
            .Produces<List<MateriaDemandaReporteDto>>(StatusCodes.Status200OK)
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

        var totalGlobalSesiones = rawSesiones.Count;

        var report = rawSesiones
            .GroupBy(s => new
            {
                s.MateriaId,
                s.NombreMateria
            })
            .Select(g =>
            {
                var total = g.Count();
                var atendidas = g.Count(x => x.Estado == "ATENDIDA");
                var pendientes = g.Count(x => x.Estado == "PENDIENTE");
                var canceladas = g.Count(x => x.Estado.StartsWith("CANCELADA"));
                var porcentaje = totalGlobalSesiones > 0
                    ? Math.Round((double)total / totalGlobalSesiones * 100, 2)
                    : 0.0;

                return new MateriaDemandaReporteDto(
                    g.Key.MateriaId,
                    g.Key.NombreMateria,
                    TotalSesiones: total,
                    SesionesAtendidas: atendidas,
                    SesionesPendientes: pendientes,
                    SesionesCanceladas: canceladas,
                    PorcentajeDemanda: porcentaje
                );
            })
            .OrderByDescending(r => r.TotalSesiones)
            .ThenByDescending(r => r.SesionesAtendidas)
            .ThenBy(r => r.NombreMateria)
            .ToList();

        if (limit.HasValue && limit.Value > 0)
        {
            report = report.Take(limit.Value).ToList();
        }

        return Results.Ok(report);
    }
}
