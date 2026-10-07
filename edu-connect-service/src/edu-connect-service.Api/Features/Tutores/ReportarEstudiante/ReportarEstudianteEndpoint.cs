using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ReportarEstudiante;

public static class ReportarEstudianteEndpoint
{
    public static void MapReportarEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPost("/sesiones/{id:int}/reportar-estudiante", HandleAsync)
            .RequireAuthorization()
            .Accepts<ReportarEstudianteRequest>("application/json")
            .Produces<ReportarEstudianteResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        ReportarEstudianteRequest request,
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
                detail: "Solo los tutores pueden reportar estudiantes."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Explicacion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Explicación requerida",
                detail: "Debe describir el motivo del reporte."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(sesion => sesion.Estado)
            .FirstOrDefaultAsync(sesion => sesion.Id == id, cancellationToken);

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
                detail: "No puede reportar una sesión que no le pertenece."
            );
        }

        if (sesion.Estado.Nombre != "ATENDIDA")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión no disponible",
                detail: "Solo se pueden reportar sesiones que estén atendidas."
            );
        }

        var categoria = await dbContext.CategoriasReportesEstudiantes
            .FirstOrDefaultAsync(categoria => categoria.Id == request.CategoriaId, cancellationToken);

        if (categoria is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Categoría inválida",
                detail: "La categoría de reporte indicada no existe."
            );
        }

        var reporte = new ReporteEstudiante
        {
            SesionId = sesion.Id,
            Sesion = sesion,
            CategoriaId = categoria.Id,
            Categoria = categoria,
            Motivo = request.Explicacion.Trim(),
            Estado = "PENDIENTE",
            FechaReporte = DateTime.UtcNow
        };

        dbContext.ReportesEstudiantes.Add(reporte);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ReportarEstudianteResponse(
            reporte.Id,
            sesion.Id,
            categoria.Nombre,
            reporte.Estado
        );

        return Results.Ok(response);
    }
}