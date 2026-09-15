using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.EstadisticasDashboard;

public static class EstadisticasDashboardEndpoint
{
    public static void MapEstadisticasDashboard(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/estadisticas", HandleAsync)
            .RequireAuthorization()
            .Produces<TutorDashboardStatsResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var idUsuarioClaim =
            user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioClaim, out var idUsuario))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Usuario no autenticado",
                detail: "No fue posible identificar al usuario autenticado."
            );
        }

        var rol =
            user.FindFirstValue("rol")
            ?? user.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(rol, "Tutor", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los tutores pueden consultar sus estadísticas de dashboard."
            );
        }

        var tutorExiste = await dbContext.Tutores
            .AsNoTracking()
            .AnyAsync(
                tutor => tutor.UsuarioId == idUsuario,
                cancellationToken
            );

        if (!tutorExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "No se encontró información de tutor asociada al usuario autenticado."
            );
        }

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        var tutorSesiones = await dbContext.Sesiones
            .AsNoTracking()
            .Where(s => s.TutorId == idUsuario)
            .Select(s => new
            {
                Estado = s.Estado.Nombre,
                s.FechaSesion
            })
            .ToListAsync(cancellationToken);

        var pendientes = tutorSesiones.Count(s => s.Estado == "PENDIENTE");
        var pendientesHoy = tutorSesiones.Count(s => s.Estado == "PENDIENTE" && s.FechaSesion == hoy);
        var atendidasMes = tutorSesiones.Count(s => s.Estado == "ATENDIDA" && s.FechaSesion.Year == hoy.Year && s.FechaSesion.Month == hoy.Month);
        var canceladas = tutorSesiones.Count(s => s.Estado.StartsWith("CANCELADA"));

        var response = new TutorDashboardStatsResponseDto(
            SesionesPendientes: pendientes,
            PendientesHoy: pendientesHoy,
            SesionesAtendidasMes: atendidasMes,
            SesionesCanceladas: canceladas
        );

        return Results.Ok(response);
    }
}

