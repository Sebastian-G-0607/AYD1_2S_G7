using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class CalificacionTutoresEndpoint
{
    public static void MapCalificacionTutores(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes/calificacion-tutores", HandleAsync)
            .Produces<List<CalificacionTutorReporteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    public static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var tutores = await dbContext.Tutores
            .AsNoTracking()
            .Where(t => t.Usuario.Estado.Nombre == "APROBADO")
            .Select(t => new
            {
                t.UsuarioId,
                t.Nombre,
                t.Apellido,
                Materias = t.TutorMaterias.Select(tm => tm.Materia.Nombre).ToList()
            })
            .ToListAsync(cancellationToken);

        var calificaciones = await dbContext.CalificacionesTutores
            .AsNoTracking()
            .Select(c => new { c.Sesion.TutorId, c.Estrellas })
            .ToListAsync(cancellationToken);

        var estrellasPorTutor = calificaciones
            .GroupBy(c => c.TutorId)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Estrellas).ToList());

        var reporte = tutores
            .Select(t =>
            {
                var estrellas = estrellasPorTutor.GetValueOrDefault(t.UsuarioId) ?? new List<int>();

                return new CalificacionTutorReporteDto(
                    t.UsuarioId,
                    $"{t.Nombre} {t.Apellido}".Trim(),
                    t.Materias.Count > 0
                        ? string.Join(", ", t.Materias)
                        : "Sin especialidad registrada",
                    CalcularPromedio(estrellas),
                    estrellas.Count
                );
            })
            .OrderByDescending(r => r.PromedioCalificacion ?? -1)
            .ThenBy(r => r.NombreCompleto)
            .ToList();

        return Results.Ok(reporte);
    }

    public static double? CalcularPromedio(IReadOnlyCollection<int> estrellas)
    {
        if (estrellas.Count == 0)
        {
            return null;
        }

        return Math.Round(estrellas.Average(), 2, MidpointRounding.AwayFromZero);
    }
}
