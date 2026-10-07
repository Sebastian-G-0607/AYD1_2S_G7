using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionReportesUsuarios;

public static class ListarReportesEstudiantesEndpoint
{
    public static void MapListarReportesEstudiantes(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/estudiantes", HandleAsync)
            .Produces<List<ReporteEstudianteAdminResponseDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var reportes = await dbContext.ReportesEstudiantes
            .AsNoTracking()
            .Include(r => r.Categoria)
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Estudiante)
                .ThenInclude(e => e.Usuario)
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Tutor)
                .ThenInclude(t => t.Usuario)
            .OrderByDescending(r => r.FechaReporte)
            .ToListAsync(cancellationToken);

        var response = reportes
            .Select(r => new ReporteEstudianteAdminResponseDto(
                r.Id,
                r.SesionId,
                r.Categoria.Nombre,
                r.Motivo,
                r.Sesion.EstudianteId,
                $"{r.Sesion.Estudiante.Nombre} {r.Sesion.Estudiante.Apellido}".Trim(),
                r.Sesion.Estudiante.Usuario.Correo,
                r.Sesion.TutorId,
                $"{r.Sesion.Tutor.Nombre} {r.Sesion.Tutor.Apellido}".Trim(),
                r.Sesion.Tutor.Usuario.Correo,
                r.Sesion.FechaSesion,
                r.FechaReporte,
                r.Estado
            ))
            .ToList();

        return Results.Ok(response);
    }
}