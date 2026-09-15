using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.HistorialSesiones;

public static class HistorialSesionesEndpoint
{
    public static void MapHistorialSesionesEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapGet("/historial", HandleAsync)
            .RequireAuthorization()
            .Produces<List<HistorialSesionEstudianteResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
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

        if (!string.Equals(
                rol,
                "Estudiante",
                StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden consultar su historial de sesiones."
            );
        }

        var estudianteExiste = await dbContext.Estudiantes
            .AsNoTracking()
            .AnyAsync(
                estudiante => estudiante.UsuarioId == idUsuario,
                cancellationToken
            );

        if (!estudianteExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: "No se encontró información de estudiante asociada al usuario autenticado."
            );
        }

        var historial = await dbContext.Sesiones
            .AsNoTracking()
            .Where(sesion =>
                sesion.EstudianteId == idUsuario &&
                (
                    sesion.Estado.Nombre == "ATENDIDA" ||
                    sesion.Estado.Nombre == "CANCELADA_ESTUDIANTE" ||
                    sesion.Estado.Nombre == "CANCELADA_TUTOR"
                )
            )
            .OrderByDescending(sesion => sesion.FechaSesion)
            .ThenByDescending(sesion => sesion.HoraInicio)
            .Select(sesion => new HistorialSesionEstudianteResponseDto(
                sesion.Id,
                sesion.FechaSesion,
                sesion.Tutor.Nombre + " " + sesion.Tutor.Apellido,
                sesion.Materia.Nombre,
                sesion.Tutor.DireccionTutoria,
                sesion.Motivo,

                sesion.Estado.Nombre == "ATENDIDA"
                    ? sesion.Resumen
                    : null,

                sesion.Estado.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(historial);
    }
}