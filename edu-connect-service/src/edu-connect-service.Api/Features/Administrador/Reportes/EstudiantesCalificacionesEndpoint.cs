using edu_connect_service.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class EstudiantesCalificacionesEndpoint
{
    public static void MapEstudiantesCalificaciones(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/calificaciones-estudiantes", HandleAsync)
            .Produces<List<EstudianteCalificacionReporteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    public static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        [FromQuery] int? limit,
        [FromQuery] string? search,
        [FromQuery] string? orden,
        CancellationToken cancellationToken)
    {
        var estudiantesActivos = await dbContext.Estudiantes
            .AsNoTracking()
            .Where(e => e.Usuario.Estado.Nombre == "APROBADO")
            .Select(e => new
            {
                e.UsuarioId,
                e.Nombre,
                e.Apellido,
                e.Carnet,
                Correo = e.Usuario.Correo,
                e.FotografiaUrl
            })
            .ToListAsync(cancellationToken);

        var sesionesAtendidas = await dbContext.Sesiones
            .AsNoTracking()
            .Where(s => s.Estado.Nombre == "ATENDIDA" && s.Estudiante.Usuario.Estado.Nombre == "APROBADO")
            .Select(s => new
            {
                s.EstudianteId,
                TieneCalificacion = s.CalificacionEstudiante != null,
                Estrellas = s.CalificacionEstudiante != null ? s.CalificacionEstudiante.Estrellas : 0
            })
            .ToListAsync(cancellationToken);

        var sesionesPorEstudiante = sesionesAtendidas
            .GroupBy(s => s.EstudianteId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalAtendidas = g.Count(),
                    Calificaciones = g.Where(x => x.TieneCalificacion).Select(x => x.Estrellas).ToList()
                }
            );

        var report = estudiantesActivos
            .Select(e =>
            {
                var tieneInfo = sesionesPorEstudiante.TryGetValue(e.UsuarioId, out var sesionInfo);
                var totalAtendidas = tieneInfo && sesionInfo != null ? sesionInfo.TotalAtendidas : 0;
                var calificaciones = tieneInfo && sesionInfo != null ? sesionInfo.Calificaciones : [];
                var totalEvaluaciones = calificaciones.Count;
                var promedio = totalEvaluaciones > 0
                    ? Math.Round(calificaciones.Average(), 2)
                    : 0.0;

                return new EstudianteCalificacionReporteDto(
                    e.UsuarioId,
                    e.Nombre,
                    e.Apellido,
                    $"{e.Nombre} {e.Apellido}".Trim(),
                    e.Carnet,
                    e.Correo,
                    e.FotografiaUrl,
                    totalAtendidas,
                    totalEvaluaciones,
                    promedio
                );
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim();
            report = report
                .Where(r =>
                    r.NombreCompleto.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    r.Carnet.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    r.Correo.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var esAscendente = string.Equals(orden, "asc", StringComparison.OrdinalIgnoreCase);

        report = esAscendente
            ? report
                .OrderBy(r => r.PromedioCalificacion)
                .ThenBy(r => r.TotalEvaluaciones)
                .ThenBy(r => r.NombreCompleto)
                .ToList()
            : report
                .OrderByDescending(r => r.PromedioCalificacion)
                .ThenByDescending(r => r.TotalEvaluaciones)
                .ThenBy(r => r.NombreCompleto)
                .ToList();

        if (limit.HasValue && limit.Value > 0)
        {
            report = report.Take(limit.Value).ToList();
        }

        return Results.Ok(report);
    }
}
