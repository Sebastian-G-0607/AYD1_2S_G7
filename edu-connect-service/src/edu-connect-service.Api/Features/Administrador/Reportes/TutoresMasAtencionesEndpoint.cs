using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class TutoresMasAtencionesEndpoint
{
    public static void MapTutoresMasAtenciones(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/tutores-mas-atendidos", HandleAsync)
            .Produces<List<TutorAtencionesReporteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var rawData = await dbContext.Sesiones
            .AsNoTracking()
            .Where(s => s.Estado.Nombre == "ATENDIDA")
            .Select(s => new
            {
                s.TutorId,
                s.Tutor.Nombre,
                s.Tutor.Apellido,
                Carnet = s.Tutor.CarnetId,
                Correo = s.Tutor.Usuario.Correo,
                s.Tutor.FotografiaUrl,
                s.EstudianteId
            })
            .ToListAsync(cancellationToken);

        var report = rawData
            .GroupBy(s => new
            {
                s.TutorId,
                s.Nombre,
                s.Apellido,
                s.Carnet,
                s.Correo,
                s.FotografiaUrl
            })
            .Select(g => new TutorAtencionesReporteDto(
                g.Key.TutorId,
                g.Key.Nombre,
                g.Key.Apellido,
                $"{g.Key.Nombre} {g.Key.Apellido}".Trim(),
                g.Key.Carnet,
                g.Key.Correo,
                g.Key.FotografiaUrl,
                TotalSesionesAtendidas: g.Count(),
                TotalEstudiantesAtendidos: g.Select(x => x.EstudianteId).Distinct().Count()
            ))
            .OrderByDescending(r => r.TotalSesionesAtendidas)
            .ThenByDescending(r => r.TotalEstudiantesAtendidos)
            .ThenBy(r => r.NombreCompleto)
            .ToList();

        if (limit.HasValue && limit.Value > 0)
        {
            report = report.Take(limit.Value).ToList();
        }

        return Results.Ok(report);
    }
}
