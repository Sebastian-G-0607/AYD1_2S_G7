using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionReportesUsuarios;

public static class ResolverReporteTutorEndpoint
{
    public static void MapResolverReporteTutor(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/reportes/tutores/{id:int}/dar-baja",
                DarBajaTutorAsync)
            .Produces<ResolverReporteResponseDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPut(
                "/reportes/tutores/{id:int}/rechazar",
                RechazarReporteAsync)
            .Produces<ResolverReporteResponseDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> DarBajaTutorAsync(
        int id,
        edu_connect_serviceContext dbContext,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
        var reporte = await dbContext.ReportesTutores
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Tutor)
                .ThenInclude(t => t.Usuario)
                .ThenInclude(u => u.Estado)
            .FirstOrDefaultAsync(
                r => r.Id == id,
                cancellationToken
            );

        if (reporte is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Reporte no encontrado",
                detail: $"No se encontró un reporte contra tutor con ID {id}."
            );
        }

        if (ReporteFinalizado(reporte.Estado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Reporte ya resuelto",
                detail: $"El reporte ya se encuentra en estado '{reporte.Estado}'."
            );
        }

        var tutor = reporte.Sesion.Tutor;

        if (!string.Equals(
                tutor.Usuario.Estado.Nombre,
                "APROBADO",
                StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Operación no permitida",
                detail:
                    $"El tutor no se encuentra activo. Su estado actual es " +
                    $"'{tutor.Usuario.Estado.Nombre}'."
            );
        }

        var estadoInactivo = await dbContext.EstadosUsuarios
            .FirstOrDefaultAsync(
                e => e.Nombre == "INACTIVO",
                cancellationToken
            );

        if (estadoInactivo is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "El estado 'INACTIVO' no está configurado en el sistema."
            );
        }

        var fechaBaja = DateTime.UtcNow;
        var motivoBaja =
            $"Cuenta dada de baja por reporte #{reporte.Id}: {reporte.Motivo}";

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            tutor.Usuario.EstadoId = estadoInactivo.Id;
            tutor.Usuario.Estado = estadoInactivo;
            tutor.Usuario.FechaBaja = fechaBaja;
            tutor.Usuario.MotivoBaja = motivoBaja;

            reporte.Estado = "RESUELTO";

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

        var nombreCompleto =
            $"{tutor.Nombre} {tutor.Apellido}".Trim();

        await emailService.SendBajaCuentaNotificacionAsync(
            tutor.Usuario.Correo,
            nombreCompleto,
            motivoBaja,
            cancellationToken
        );

        return Results.Ok(
            new ResolverReporteResponseDto(
                reporte.Id,
                reporte.Estado,
                tutor.UsuarioId,
                "INACTIVO",
                "El reporte fue resuelto y el tutor fue dado de baja."
            )
        );
    }

    private static async Task<IResult> RechazarReporteAsync(
        int id,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var reporte = await dbContext.ReportesTutores
            .Include(r => r.Sesion)
                .ThenInclude(s => s.Tutor)
                .ThenInclude(t => t.Usuario)
                .ThenInclude(u => u.Estado)
            .FirstOrDefaultAsync(
                r => r.Id == id,
                cancellationToken
            );

        if (reporte is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Reporte no encontrado",
                detail: $"No se encontró un reporte contra tutor con ID {id}."
            );
        }

        if (ReporteFinalizado(reporte.Estado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Reporte ya resuelto",
                detail: $"El reporte ya se encuentra en estado '{reporte.Estado}'."
            );
        }

        reporte.Estado = "DESESTIMADO";

        await dbContext.SaveChangesAsync(cancellationToken);

        var tutor = reporte.Sesion.Tutor;

        return Results.Ok(
            new ResolverReporteResponseDto(
                reporte.Id,
                reporte.Estado,
                tutor.UsuarioId,
                tutor.Usuario.Estado.Nombre,
                "La denuncia fue rechazada sin modificar el estado del tutor."
            )
        );
    }

    private static bool ReporteFinalizado(string estado)
    {
        return
            string.Equals(
                estado,
                "RESUELTO",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                estado,
                "DESESTIMADO",
                StringComparison.OrdinalIgnoreCase);
    }
}