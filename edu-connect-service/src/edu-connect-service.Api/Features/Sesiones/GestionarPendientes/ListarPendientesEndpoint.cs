using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.GestionarPendientes;

public static class ListarPendientesEndpoint
{
    public static void MapListarPendientes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/pendientes", HandleAsync)
            .RequireAuthorization()
            .Produces<List<SesionPendienteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
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
                detail: "Solo los tutores pueden consultar sus sesiones pendientes."
            );
        }

        var sesiones = await dbContext.Sesiones
            .AsNoTracking()
            .Include(sesion => sesion.Estudiante)
            .Include(sesion => sesion.Materia)
            .Include(sesion => sesion.Estado)
            .Where(sesion =>
                sesion.TutorId == idUsuario &&
                sesion.Estado.Nombre == "PENDIENTE")
            .OrderBy(sesion => sesion.FechaSesion)
            .ThenBy(sesion => sesion.HoraInicio)
            .Select(sesion => new SesionPendienteDto(
                sesion.Id,
                sesion.FechaSesion.ToString("dd MMM, yyyy"),
                sesion.HoraInicio.ToString("hh:mm tt"),
                sesion.Estudiante.Nombre + " " + sesion.Estudiante.Apellido,
                sesion.Estudiante.Carnet,
                sesion.Materia.Nombre,
                sesion.Motivo,
                sesion.Estado.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(sesiones);
    }
}