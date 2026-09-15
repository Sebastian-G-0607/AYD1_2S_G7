using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ConfigurarHorario;

public static class ObtenerHorarioEndpoint
{
    public static void MapObtenerHorario(this IEndpointRouteBuilder app)
    {
        app.MapGet("/horarios", HandleAsync)
            .RequireAuthorization()
            .Produces<ObtenerHorarioResponseDto>(StatusCodes.Status200OK)
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

        if (!string.Equals(rol, "Tutor", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los tutores pueden consultar su horario de atención."
            );
        }

        var tutor = await dbContext.Tutores
            .AsNoTracking()
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

        var diasAtencion = await dbContext.TutoresDiasAtencion
            .AsNoTracking()
            .Where(dia => dia.TutorId == tutor.UsuarioId)
            .Select(dia => dia.DiaSemana)
            .OrderBy(dia => dia)
            .ToListAsync(cancellationToken);

        var response = new ObtenerHorarioResponseDto(
            tutor.UsuarioId,
            tutor.HoraInicio,
            tutor.HoraFin,
            diasAtencion
        );

        return Results.Ok(response);
    }
}