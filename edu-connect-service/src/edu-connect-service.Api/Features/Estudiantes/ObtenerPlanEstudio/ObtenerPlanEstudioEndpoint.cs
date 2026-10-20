using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.ObtenerPlanEstudio;

public static class ObtenerPlanEstudioEndpoint
{
    public static void MapObtenerPlanEstudio(this IEndpointRouteBuilder app)
    {
        app.MapGet("/plan-estudio", HandleAsync)
            .RequireAuthorization()
            .Produces<PlanEstudioResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
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

        if (!string.Equals(rol, "Estudiante", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden consultar su plan de estudio."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(sesion => sesion.Tutor)
            .Include(sesion => sesion.Materia)
            .Include(sesion => sesion.PlanEstudio)
                .ThenInclude(planEstudio => planEstudio!.Recursos)
            .Where(sesion =>
                sesion.EstudianteId == idUsuario &&
                sesion.PlanEstudio != null
            )
            .OrderByDescending(sesion => sesion.PlanEstudio!.FechaCreacion)
            .FirstOrDefaultAsync(cancellationToken);

        if (sesion is null || sesion.PlanEstudio is null)
        {
            return Results.NoContent();
        }

        var response = new PlanEstudioResponse(
            sesion.FechaSesion.ToString("yyyy-MM-dd"),
            $"{sesion.Tutor.Nombre} {sesion.Tutor.Apellido}",
            sesion.Tutor.NumeroIdentificacion,
            sesion.Materia.Nombre,
            sesion.PlanEstudio.DificultadesIdentificadas,
            sesion.PlanEstudio.Recursos
                .Select(recurso => new RecursoRecomendadoResponse(
                    recurso.Nombre,
                    recurso.Tipo,
                    recurso.DescripcionUso ?? string.Empty
                ))
                .ToList()
        );

        return Results.Ok(response);
    }
}