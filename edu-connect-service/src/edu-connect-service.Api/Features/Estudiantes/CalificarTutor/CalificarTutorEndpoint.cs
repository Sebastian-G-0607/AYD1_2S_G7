using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.CalificarTutor;

public static class CalificarTutorEndpoint
{
    public static void MapCalificarTutor(this IEndpointRouteBuilder app)
    {
        app.MapPost("/sesiones/{id:int}/calificar-tutor", HandleAsync)
            .RequireAuthorization()
            .Accepts<CalificarTutorRequestDto>("application/json")
            .Produces<CalificarTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        CalificarTutorRequestDto request,
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

        if (!string.Equals(rol, "Estudiante", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden calificar la atención de una tutoría."
            );
        }

        if (request.Estrellas is null || request.Estrellas < 0 || request.Estrellas > 5)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Calificación inválida",
                detail: "Debe seleccionar una calificación entre 0 y 5 estrellas."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(sesion => sesion.Estado)
            .Include(sesion => sesion.CalificacionTutor)
            .FirstOrDefaultAsync(sesion => sesion.Id == id, cancellationToken);

        if (sesion is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Sesión no encontrada",
                detail: "La sesión indicada no existe."
            );
        }

        if (sesion.EstudianteId != idUsuario)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "No puede calificar una sesión que no le pertenece."
            );
        }

        if (sesion.Estado.Nombre != "ATENDIDA")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión no disponible",
                detail: "Solo se pueden calificar sesiones que estén atendidas."
            );
        }

        if (sesion.CalificacionTutor is not null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Calificación existente",
                detail: "Esta sesión ya tiene una calificación registrada."
            );
        }

        var calificacion = new CalificacionTutor
        {
            SesionId = sesion.Id,
            Sesion = sesion,
            Estrellas = request.Estrellas.Value,
            Comentario = string.IsNullOrWhiteSpace(request.Comentario)
                ? null
                : request.Comentario.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.CalificacionesTutores.Add(calificacion);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new CalificarTutorResponseDto(
            sesion.Id,
            calificacion.Estrellas,
            calificacion.Comentario
        ));
    }
}