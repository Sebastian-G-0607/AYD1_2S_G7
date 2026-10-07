using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionReportesUsuarios;

public static class ListarReportesTutoresEndpoint
{
    public static void MapListarReportesTutores(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/tutores", HandleAsync)
            .Produces<List<ReporteTutorAdminResponseDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var reportes = await dbContext.ReportesTutores
            .AsNoTracking()
            .Include(r => r.Categoria)
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Tutor)
                .ThenInclude(t => t.Usuario)
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Estudiante)
                .ThenInclude(e => e.Usuario)
            .OrderByDescending(r => r.FechaReporte)
            .ToListAsync(cancellationToken);

        var response = reportes
            .Select(r => new ReporteTutorAdminResponseDto(
                r.Id,
                r.SesionId,
                r.Categoria.Nombre,
                r.Motivo,
                r.Sesion.TutorId,
                $"{r.Sesion.Tutor.Nombre} {r.Sesion.Tutor.Apellido}".Trim(),
                r.Sesion.Tutor.Usuario.Correo,
                r.Sesion.EstudianteId,
                $"{r.Sesion.Estudiante.Nombre} {r.Sesion.Estudiante.Apellido}".Trim(),
                r.Sesion.Estudiante.Usuario.Correo,
                r.Sesion.FechaSesion,
                r.FechaReporte,
                r.Estado
            ))
            .ToList();

        return Results.Ok(response);
    }
}