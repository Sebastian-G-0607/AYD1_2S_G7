using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.GestionarPendientes;

public static class AtenderSesionEndpoint
{
    public static void MapAtenderSesion(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:int}/atender", HandleAsync)
            .RequireAuthorization()
            .Accepts<AtenderSesionRequestDto>("application/json")
            .Produces<AtenderSesionResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        AtenderSesionRequestDto request,
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
                detail: "Solo los tutores pueden marcar sesiones como atendidas."
            );
        }

        if (string.IsNullOrWhiteSpace(request.DificultadesIdentificadas))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dificultades requeridas",
                detail: "Debe ingresar las dificultades identificadas para marcar la sesión como atendida."
            );
        }

        if (request.Recursos is null || request.Recursos.Count == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Recursos requeridos",
                detail: "Debe recomendar al menos un recurso de estudio."
            );
        }

        if (request.Recursos.Any(recurso =>
                recurso is null ||
                string.IsNullOrWhiteSpace(recurso.Nombre) ||
                string.IsNullOrWhiteSpace(recurso.Tipo) ||
                string.IsNullOrWhiteSpace(recurso.DescripcionUso)))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Recurso incompleto",
                detail: "Cada recurso debe incluir nombre, tipo y descripción de uso."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(sesion => sesion.Estado)
            .Include(sesion => sesion.PlanEstudio)
            .FirstOrDefaultAsync(
                sesion => sesion.Id == id,
                cancellationToken
            );

        if (sesion is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Sesión no encontrada",
                detail: "La sesión indicada no existe."
            );
        }

        if (sesion.TutorId != idUsuario)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "No puede atender una sesión que no le pertenece."
            );
        }

        if (sesion.Estado.Nombre != "PENDIENTE")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión no disponible",
                detail: "Solo se pueden atender sesiones que estén pendientes."
            );
        }

        if (sesion.PlanEstudio is not null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Plan de estudio existente",
                detail: "La sesión ya tiene un plan de estudio registrado."
            );
        }

        var estadoAtendida = await dbContext.EstadosSesiones
            .FirstOrDefaultAsync(
                estado => estado.Nombre == "ATENDIDA",
                cancellationToken
            );

        if (estadoAtendida is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Estado de sesión no configurado",
                detail: "No se encontró el estado ATENDIDA en el sistema."
            );
        }

        var resumenPartes = new List<string>();
        if (!string.IsNullOrWhiteSpace(request.Resumen))
        {
            resumenPartes.Add(request.Resumen.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.Recomendaciones))
        {
            resumenPartes.Add($"Recomendaciones: {request.Recomendaciones.Trim()}");
        }

        if (resumenPartes.Count > 0)
        {
            sesion.Resumen = string.Join("\n\n", resumenPartes);
        }

        var planEstudio = new PlanEstudio
        {
            SesionId = sesion.Id,
            Sesion = sesion,
            DificultadesIdentificadas = request.DificultadesIdentificadas.Trim(),
            FechaCreacion = DateTime.UtcNow,
            Recursos = request.Recursos.Select(recurso => new RecursoPlanEstudio
            {
                Nombre = recurso.Nombre!.Trim(),
                Tipo = recurso.Tipo!.Trim(),
                DescripcionUso = recurso.DescripcionUso!.Trim()
            }).ToList()
        };

        sesion.PlanEstudio = planEstudio;
        sesion.EstadoId = estadoAtendida.Id;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new AtenderSesionResponseDto(
            sesion.Id,
            estadoAtendida.Nombre,
            sesion.Resumen ?? string.Empty
        );

        return Results.Ok(response);
    }
}