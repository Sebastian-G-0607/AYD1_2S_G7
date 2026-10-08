using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class EstudiantesMasSesionesEndpoint
{
    public static void MapEstudiantesMasSesiones(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/estudiantes-mas-sesiones", HandleAsync)
            .Produces<List<EstudianteSesionesReporteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        IS3Service s3Service,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var rawData = await dbContext.Sesiones
            .AsNoTracking()
            .Select(s => new
            {
                s.EstudianteId,
                s.Estudiante.Nombre,
                s.Estudiante.Apellido,
                s.Estudiante.Carnet,
                Correo = s.Estudiante.Usuario.Correo,
                s.Estudiante.FotografiaUrl,
                Estado = s.Estado.Nombre
            })
            .ToListAsync(cancellationToken);

        var report = rawData
            .GroupBy(s => new
            {
                s.EstudianteId,
                s.Nombre,
                s.Apellido,
                s.Carnet,
                s.Correo,
                s.FotografiaUrl
            })
            .Select(g => new EstudianteSesionesReporteDto(
                g.Key.EstudianteId,
                g.Key.Nombre,
                g.Key.Apellido,
                $"{g.Key.Nombre} {g.Key.Apellido}".Trim(),
                g.Key.Carnet,
                g.Key.Correo,
                s3Service.GeneratePresignedUrl(g.Key.FotografiaUrl) ?? g.Key.FotografiaUrl,
                TotalSesionesProgramadas: g.Count(),
                SesionesAtendidas: g.Count(x => x.Estado == "ATENDIDA"),
                SesionesCanceladas: g.Count(x => x.Estado.StartsWith("CANCELADA")),
                SesionesPendientes: g.Count(x => x.Estado == "PENDIENTE")
            ))
            .OrderByDescending(r => r.TotalSesionesProgramadas)
            .ThenByDescending(r => r.SesionesAtendidas)
            .ThenBy(r => r.NombreCompleto)
            .ToList();

        if (limit.HasValue && limit.Value > 0)
        {
            report = report.Take(limit.Value).ToList();
        }

        return Results.Ok(report);
    }
}
