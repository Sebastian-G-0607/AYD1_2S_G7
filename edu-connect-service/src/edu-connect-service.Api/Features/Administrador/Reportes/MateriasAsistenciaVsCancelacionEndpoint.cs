using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class MateriasAsistenciaVsCancelacionEndpoint
{
    public static void MapMateriasAsistenciaVsCancelacion(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/materias-asistencia-vs-cancelacion", HandleAsync)
            .Produces<List<MateriaAsistenciaVsCancelacionReporteDto>>(StatusCodes.Status200OK)
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
                var atendidas = g.Count(x => x.Estado == "ATENDIDA");
                var canceladas = g.Count(x => x.Estado.StartsWith("CANCELADA"));
                var pendientes = g.Count(x => x.Estado == "PENDIENTE");

                var tasaAsistencia = total > 0
                    ? Math.Round((double)atendidas / total * 100, 2)
                    : 0.0;
                var tasaCancelacion = total > 0
                    ? Math.Round((double)canceladas / total * 100, 2)
                    : 0.0;

                return new MateriaAsistenciaVsCancelacionReporteDto(
                    g.Key.MateriaId,
                    g.Key.NombreMateria,
                    TotalSesiones: total,
                    SesionesAtendidas: atendidas,
                    SesionesCanceladas: canceladas,
                    SesionesPendientes: pendientes,
                    TasaAsistencia: tasaAsistencia,
                    TasaCancelacion: tasaCancelacion
                );
            })
            .OrderByDescending(r => r.TotalSesiones)
            .ThenByDescending(r => r.TasaAsistencia)
            .ThenBy(r => r.NombreMateria)
            .ToList();

        var take = limit.HasValue && limit.Value > 0 ? limit.Value : 10;
        report = report.Take(take).ToList();

        return Results.Ok(report);
    }
}
