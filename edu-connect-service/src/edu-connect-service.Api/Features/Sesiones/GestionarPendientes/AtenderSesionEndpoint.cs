using System.Security.Claims;
using edu_connect_service.Api.Data;
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

    private static async Task<IResult> HandleAsync(
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

        if (string.IsNullOrWhiteSpace(request.Resumen))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Resumen requerido",
                detail: "Debe ingresar el resumen de la sesión para marcarla como atendida."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(sesion => sesion.Estado)
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

        var resumenCompleto = string.IsNullOrWhiteSpace(request.Recomendaciones)
            ? request.Resumen.Trim()
            : $"{request.Resumen.Trim()}\n\nRecomendaciones: {request.Recomendaciones.Trim()}";

        sesion.EstadoId = estadoAtendida.Id;
        sesion.Resumen = resumenCompleto;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new AtenderSesionResponseDto(
            sesion.Id,
            estadoAtendida.Nombre,
            sesion.Resumen
        );

        return Results.Ok(response);
    }
}