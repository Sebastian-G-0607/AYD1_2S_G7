using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.ReportarTutor;

public static class ReportarTutorEndpoint
{
    public const int MotivoMinLength = 10;
    public const int MotivoMaxLength = 1000;
    public const string EstadoInicial = "PENDIENTE";

    public static void MapReportarTutor(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reportes-tutor/categorias", ListarCategoriasAsync)
            .RequireAuthorization()
            .Produces<List<CategoriaReporteTutorResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        app.MapPost("/sesiones/{id:int}/reportar-tutor", HandleAsync)
            .RequireAuthorization()
            .Accepts<ReportarTutorRequestDto>("application/json")
            .Produces<ReportarTutorResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> ListarCategoriasAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var acceso = ValidarEstudiante(user, out _);
        if (acceso is not null)
        {
            return acceso;
        }

        var categorias = await dbContext.CategoriasReportesTutores
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CategoriaReporteTutorResponseDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync(cancellationToken);

        return Results.Ok(categorias);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        ReportarTutorRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var acceso = ValidarEstudiante(user, out var idUsuario);
        if (acceso is not null)
        {
            return acceso;
        }

        if (request.CategoriaId <= 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Categoría requerida",
                detail: "Debe seleccionar una categoría para el reporte."
            );
        }

        var motivo = request.Motivo?.Trim() ?? string.Empty;

        if (motivo.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Explicación requerida",
                detail: "Debe explicar el motivo del reporte."
            );
        }

        if (motivo.Length < MotivoMinLength)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Explicación muy corta",
                detail: $"La explicación debe tener al menos {MotivoMinLength} caracteres."
            );
        }

        if (motivo.Length > MotivoMaxLength)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Explicación muy larga",
                detail: $"La explicación no puede superar los {MotivoMaxLength} caracteres."
            );
        }

        var categoria = await dbContext.CategoriasReportesTutores
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId, cancellationToken);

        if (categoria is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Categoría inválida",
                detail: "La categoría seleccionada no existe."
            );
        }

        var sesion = await dbContext.Sesiones
            .Include(s => s.Estado)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sesion is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Sesión no encontrada",
                detail: "No se encontró la sesión indicada."
            );
        }

        if (sesion.EstudianteId != idUsuario)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo puede reportar tutores de sus propias sesiones."
            );
        }

        if (!string.Equals(sesion.Estado.Nombre, "ATENDIDA", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión no reportable",
                detail: "Solo se pueden reportar sesiones con estado Atendida."
            );
        }

        var yaReportada = await dbContext.ReportesTutores
            .AnyAsync(r => r.SesionId == id, cancellationToken);

        if (yaReportada)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión ya reportada",
                detail: "Ya existe un reporte registrado para esta sesión."
            );
        }

        var reporte = new ReporteTutor
        {
            SesionId = sesion.Id,
            CategoriaId = categoria.Id,
            Motivo = motivo,
            FechaReporte = DateTime.UtcNow,
            Estado = EstadoInicial
        };

        dbContext.ReportesTutores.Add(reporte);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/api/estudiantes/sesiones/{sesion.Id}/reportar-tutor",
            new ReportarTutorResponseDto(
                reporte.Id,
                reporte.SesionId,
                categoria.Nombre,
                reporte.Estado,
                "El reporte fue enviado al administrador."
            )
        );
    }

    private static IResult? ValidarEstudiante(ClaimsPrincipal user, out int idUsuario)
    {
        var idUsuarioClaim =
            user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioClaim, out idUsuario))
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
                detail: "Solo los estudiantes pueden reportar tutores."
            );
        }

        return null;
    }
}
