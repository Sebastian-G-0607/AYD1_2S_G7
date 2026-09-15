using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.CancelarSesionEstudiante;

public static class CancelarSesionEstudianteEndpoint
{
    public static void MapCancelarSesionEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}/cancelar", HandleAsync)
            .RequireAuthorization()
            .Produces<CancelarSesionEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPut("/{id:int}/cancelar-estudiante", HandleAsync)
            .RequireAuthorization()
            .Produces<CancelarSesionEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public static void MapCancelarSesionEstudianteSubruta(this IEndpointRouteBuilder app)
    {
        app.MapPut("/sesiones/{id:int}/cancelar", HandleAsync)
            .RequireAuthorization()
            .Produces<CancelarSesionEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleAsync(
        int id,
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

        if (!string.Equals(rol, AppRoles.Estudiante, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden cancelar sus sesiones mediante este endpoint."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(s => s.Estado)
            .Include(s => s.Materia)
            .Include(s => s.Tutor)
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

        if (sesion.EstudianteId != idUsuario)
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
                e => e.Nombre == "CANCELADA_ESTUDIANTE",
                cancellationToken
            );

        if (estadoCancelada is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Estado de sesión no configurado",
                detail: "No se encontró el estado CANCELADA_ESTUDIANTE en el catálogo del sistema."
            );
        }

        sesion.EstadoId = estadoCancelada.Id;
        sesion.Estado = estadoCancelada;
        sesion.MotivoCancelacion = "Cancelada por el estudiante";

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreTutor = $"{sesion.Tutor.Nombre} {sesion.Tutor.Apellido}".Trim();

        var response = new CancelarSesionEstudianteResponseDto(
            sesion.Id,
            estadoCancelada.Nombre,
            sesion.FechaSesion,
            sesion.HoraInicio,
            sesion.Materia.Nombre,
            nombreTutor,
            "La sesión ha sido cancelada exitosamente por el estudiante."
        );

        return Results.Ok(response);
    }
}
