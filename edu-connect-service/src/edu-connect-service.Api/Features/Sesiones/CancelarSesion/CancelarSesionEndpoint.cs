using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.CancelarSesion;

public static class CancelarSesionEndpoint
{
    public static void MapCancelarSesion(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:int}/cancelar", HandleAsync)
            .RequireAuthorization()
            .Accepts<CancelarSesionRequestDto>("application/json")
            .Produces<CancelarSesionResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] CancelarSesionRequestDto? request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        IEmailService emailService,
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
                detail: "Solo los tutores pueden cancelar sesiones de tutoría."
            );
        }

        if (request is null || string.IsNullOrWhiteSpace(request.Motivo))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Motivo requerido",
                detail: "Debe ingresar el motivo de cancelación de la sesión."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(s => s.Estado)
            .Include(s => s.Materia)
            .Include(s => s.Tutor)
            .Include(s => s.Estudiante)
                .ThenInclude(e => e.Usuario)
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken
            );

        if (sesion is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Sesión no encontrada",
                detail: $"No se encontró una sesión con ID {id}."
            );
        }

        if (sesion.TutorId != idUsuario)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "No puede cancelar una sesión que no le pertenece."
            );
        }

        if (sesion.Estado.Nombre != "PENDIENTE")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión no cancelable",
                detail: $"Solo se pueden cancelar sesiones que estén en estado PENDIENTE. El estado actual es '{sesion.Estado.Nombre}'."
            );
        }

        var estadoCancelada = await dbContext.EstadosSesiones
            .FirstOrDefaultAsync(
                e => e.Nombre == "CANCELADA_TUTOR",
                cancellationToken
            );

        if (estadoCancelada is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Estado de sesión no configurado",
                detail: "No se encontró el estado CANCELADA_TUTOR en el catálogo del sistema."
            );
        }

        var motivoCancelacion = request.Motivo.Trim();

        sesion.EstadoId = estadoCancelada.Id;
        sesion.Estado = estadoCancelada;
        sesion.MotivoCancelacion = motivoCancelacion;

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreEstudiante = $"{sesion.Estudiante.Nombre} {sesion.Estudiante.Apellido}".Trim();
        var nombreTutor = $"{sesion.Tutor.Nombre} {sesion.Tutor.Apellido}".Trim();

        await emailService.SendCancelacionSesionTutorNotificacionAsync(
            sesion.Estudiante.Usuario.Correo,
            nombreEstudiante,
            nombreTutor,
            sesion.Materia.Nombre,
            sesion.FechaSesion,
            sesion.HoraInicio,
            sesion.Motivo,
            motivoCancelacion,
            request.MensajeDisculpa,
            cancellationToken
        );

        var response = new CancelarSesionResponseDto(
            sesion.Id,
            estadoCancelada.Nombre,
            motivoCancelacion,
            sesion.FechaSesion,
            sesion.HoraInicio,
            sesion.Materia.Nombre,
            nombreEstudiante,
            "La sesión ha sido cancelada exitosamente y el estudiante ha sido notificado por correo electrónico."
        );

        return Results.Ok(response);
    }
}
