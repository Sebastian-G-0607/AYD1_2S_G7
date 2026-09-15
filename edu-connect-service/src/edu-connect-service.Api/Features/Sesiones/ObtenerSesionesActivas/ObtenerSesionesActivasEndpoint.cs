using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Authorization;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.ObtenerSesionesActivas;

public static class ObtenerSesionesActivasEndpoint
{
    public static void MapObtenerSesionesActivas(this IEndpointRouteBuilder app)
    {
        app.MapGet("/activas", HandleAsync)
            .RequireAuthorization()
            .Produces<List<SesionActivaEstudianteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static void MapObtenerSesionesActivasEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapGet("/sesiones-activas", HandleAsync)
            .RequireAuthorization()
            .Produces<List<SesionActivaEstudianteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        IS3Service s3Service,
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
                detail: "Solo los estudiantes pueden consultar sus sesiones activas."
            );
        }

        var estudianteExiste = await dbContext.Estudiantes
            .AsNoTracking()
            .AnyAsync(
                e => e.UsuarioId == idUsuario,
                cancellationToken
            );

        if (!estudianteExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: "No existe información de estudiante asociada al usuario autenticado."
            );
        }

        var sesiones = await dbContext.Sesiones
            .AsNoTracking()
            .Include(s => s.Tutor)
            .Include(s => s.Materia)
            .Include(s => s.Estado)
            .Where(s =>
                s.EstudianteId == idUsuario &&
                s.Estado.Nombre == "PENDIENTE")
            .OrderBy(s => s.FechaSesion)
            .ThenBy(s => s.HoraInicio)
            .Select(s => new
            {
                s.Id,
                s.FechaSesion,
                s.HoraInicio,
                TutorNombre = s.Tutor.Nombre,
                TutorApellido = s.Tutor.Apellido,
                Materia = s.Materia.Nombre,
                s.Tutor.DireccionTutoria,
                s.Motivo,
                Estado = s.Estado.Nombre,
                s.Tutor.FotografiaUrl
            })
            .ToListAsync(cancellationToken);

        var response = sesiones.Select(s => new SesionActivaEstudianteDto(
            s.Id,
            s.FechaSesion,
            s.HoraInicio,
            $"{s.TutorNombre} {s.TutorApellido}".Trim(),
            s.Materia,
            s.DireccionTutoria,
            s.Motivo,
            s.Estado,
            !string.IsNullOrWhiteSpace(s.FotografiaUrl)
                ? s3Service.GeneratePresignedUrl(s.FotografiaUrl) ?? s.FotografiaUrl
                : null
        )).ToList();

        return Results.Ok(response);
    }
}
