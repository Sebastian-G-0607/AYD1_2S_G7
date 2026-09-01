using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ConfigurarHorario;

public static class ConfigurarHorarioEndpoint
{
    public static void MapConfigurarHorario(this IEndpointRouteBuilder app)
    {
        app.MapPut("/horarios", HandleAsync)
            .RequireAuthorization()
            .Accepts<ConfigurarHorarioRequestDto>("application/json")
            .Produces<ConfigurarHorarioResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        ConfigurarHorarioRequestDto request,
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
                detail: "Solo los tutores pueden configurar horarios de atención."
            );
        }

        if (request.HoraFin <= request.HoraInicio)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Rango de horario inválido",
                detail: "La hora de fin debe ser posterior a la hora de inicio."
            );
        }

        if (request.DiasAtencion is null || request.DiasAtencion.Count == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Días de atención requeridos",
                detail: "Debe seleccionar al menos un día de atención."
            );
        }

        var diasAtencion = request.DiasAtencion
            .Distinct()
            .OrderBy(dia => dia)
            .ToList();

        if (diasAtencion.Any(dia => dia < 1 || dia > 7))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Día de atención inválido",
                detail: "Los días de atención deben estar entre 1 (Lunes) y 7 (Domingo)."
            );
        }

        var tutor = await dbContext.Tutores
            .FirstOrDefaultAsync(
                tutor => tutor.UsuarioId == idUsuario,
                cancellationToken
            );

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "No se encontró información de tutor asociada al usuario autenticado."
            );
        }

        // Obtener las sesiones activas del tutor.
        var sesionesActivas = await dbContext.Sesiones
            .AsNoTracking()
            .Include(sesion => sesion.Estado)
            .Where(sesion =>
                sesion.TutorId == tutor.UsuarioId &&
                sesion.Estado.Nombre == "PENDIENTE")
            .ToListAsync(cancellationToken);

        // Validar que ninguna sesión activa quede fuera del nuevo horario.
        var sesionFueraDelNuevoHorario = sesionesActivas.FirstOrDefault(sesion =>
        {
            var diaSemana = sesion.FechaSesion.DayOfWeek == DayOfWeek.Sunday
                ? 7
                : (int)sesion.FechaSesion.DayOfWeek;

            var diaNoDisponible =
                !diasAtencion.Contains(diaSemana);

            var iniciaAntesDelHorario =
                sesion.HoraInicio < request.HoraInicio;

            var iniciaFueraDelHorario =
                sesion.HoraInicio >= request.HoraFin;

            var terminaFueraDelHorario =
                sesion.HoraFin.HasValue &&
                sesion.HoraFin.Value > request.HoraFin;

            return diaNoDisponible
                || iniciaAntesDelHorario
                || iniciaFueraDelHorario
                || terminaFueraDelHorario;
        });

        if (sesionFueraDelNuevoHorario is not null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horario en conflicto con sesiones activas",
                detail:
                    $"No se puede actualizar el horario porque la sesión " +
                    $"{sesionFueraDelNuevoHorario.Id} programada para el " +
                    $"{sesionFueraDelNuevoHorario.FechaSesion:dd/MM/yyyy} a las " +
                    $"{sesionFueraDelNuevoHorario.HoraInicio.ToString("HH:mm")} " +
                    "quedaría fuera del nuevo horario de atención."
            );
        }

        // Actualizar el rango horario del tutor.
        tutor.HoraInicio = request.HoraInicio;
        tutor.HoraFin = request.HoraFin;

        // Obtener y eliminar los días configurados anteriormente.
        var diasActuales = await dbContext.TutoresDiasAtencion
            .Where(dia => dia.TutorId == tutor.UsuarioId)
            .ToListAsync(cancellationToken);

        dbContext.TutoresDiasAtencion.RemoveRange(diasActuales);

        // Registrar los nuevos días de atención.
        foreach (var dia in diasAtencion)
        {
            dbContext.TutoresDiasAtencion.Add(new TutorDiaAtencion
            {
                TutorId = tutor.UsuarioId,
                DiaSemana = dia
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ConfigurarHorarioResponseDto(
            tutor.UsuarioId,
            request.HoraInicio,
            request.HoraFin,
            diasAtencion
        );

        return Results.Ok(response);
    }
}