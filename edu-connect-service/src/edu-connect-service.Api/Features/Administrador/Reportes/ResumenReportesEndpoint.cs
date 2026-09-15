using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class ResumenReportesEndpoint
{
    public static void MapResumenReportes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/resumen", HandleAsync)
            .Produces<ReportesResumenDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var rawSesiones = await dbContext.Sesiones
            .AsNoTracking()
            .Select(s => new
            {
                s.TutorId,
                NombreTutor = s.Tutor.Nombre + " " + s.Tutor.Apellido,
                s.MateriaId,
                NombreMateria = s.Materia.Nombre,
                Estado = s.Estado.Nombre
            })
            .ToListAsync(cancellationToken);

        var totalSesiones = rawSesiones.Count;
        var atendidas = rawSesiones.Count(s => s.Estado == "ATENDIDA");
        var pendientes = rawSesiones.Count(s => s.Estado == "PENDIENTE");
        var canceladas = rawSesiones.Count(s => s.Estado.StartsWith("CANCELADA"));

        var tasaEfectividad = totalSesiones > 0
            ? Math.Round((double)atendidas / totalSesiones * 100, 2)
            : 0.0;

        var tutoresAtendidosGroup = rawSesiones
            .Where(s => s.Estado == "ATENDIDA")
            .GroupBy(s => new { s.TutorId, s.NombreTutor })
            .Select(g => new { g.Key.NombreTutor, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        var materiasDemandaGroup = rawSesiones
            .GroupBy(s => new { s.MateriaId, s.NombreMateria })
            .Select(g => new { g.Key.NombreMateria, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        var totalTutoresConAtenciones = rawSesiones
            .Where(s => s.Estado == "ATENDIDA")
            .Select(s => s.TutorId)
            .Distinct()
            .Count();

        var totalMateriasConDemanda = rawSesiones
            .Select(s => s.MateriaId)
            .Distinct()
            .Count();

        var resumen = new ReportesResumenDto(
            TotalSesiones: totalSesiones,
            TotalSesionesAtendidas: atendidas,
            TotalSesionesPendientes: pendientes,
            TotalSesionesCanceladas: canceladas,
            TasaEfectividad: tasaEfectividad,
            TotalTutoresConAtenciones: totalTutoresConAtenciones,
            TotalMateriasConDemanda: totalMateriasConDemanda,
            TutorTopNombre: tutoresAtendidosGroup?.NombreTutor,
            TutorTopAtenciones: tutoresAtendidosGroup?.Count ?? 0,
            MateriaTopNombre: materiasDemandaGroup?.NombreMateria,
            MateriaTopSesiones: materiasDemandaGroup?.Count ?? 0
        );

        return Results.Ok(resumen);
    }
}
