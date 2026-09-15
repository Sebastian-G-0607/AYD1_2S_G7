using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.HistorialSesiones;

public static class HistorialSesionesEndpoint
{
    public static void MapHistorialSesiones(this IEndpointRouteBuilder app)
    {
        app.MapGet("/historial", HandleAsync)
            .RequireAuthorization()
            .Produces<List<HistorialSesionResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] HistorialSesionesRequestDto filtros,
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

        if (!string.Equals(rol, "Tutor", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los tutores pueden consultar su historial de sesiones."
            );
        }

        var tutorExiste = await dbContext.Tutores
            .AsNoTracking()
            .AnyAsync(
                tutor => tutor.UsuarioId == idUsuario,
                cancellationToken
            );

        if (!tutorExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "No se encontró información de tutor asociada al usuario autenticado."
            );
        }

        var query = dbContext.Sesiones
            .AsNoTracking()
            .Where(sesion =>
                sesion.TutorId == idUsuario &&
                sesion.Estado.Nombre != "PENDIENTE"
            );

        if (filtros.Fecha.HasValue)
        {
            query = query.Where(
                sesion => sesion.FechaSesion == filtros.Fecha.Value
            );
        }

        if (!string.IsNullOrWhiteSpace(filtros.Estudiante))
        {
            var filtroEstudiante = $"%{filtros.Estudiante.Trim().ToLower()}%";

            query = query.Where(sesion =>
                EF.Functions.Like((sesion.Estudiante.Nombre + " " + sesion.Estudiante.Apellido).ToLower(), filtroEstudiante) ||
                EF.Functions.Like(sesion.Estudiante.Usuario.Correo.ToLower(), filtroEstudiante)
            );
        }

        if (!string.IsNullOrWhiteSpace(filtros.Correo))
        {
            var filtroCorreo = $"%{filtros.Correo.Trim().ToLower()}%";

            query = query.Where(sesion =>
                EF.Functions.Like(sesion.Estudiante.Usuario.Correo.ToLower(), filtroCorreo)
            );
        }

        var sesiones = await query
            .OrderByDescending(sesion => sesion.FechaSesion)
            .ThenByDescending(sesion => sesion.HoraInicio)
            .Select(sesion => new
            {
                sesion.Id,
                sesion.FechaSesion,
                sesion.HoraInicio,
                Estudiante = sesion.Estudiante.Nombre + " " + sesion.Estudiante.Apellido,
                EstudianteEmail = sesion.Estudiante.Usuario.Correo,
                Estado = sesion.Estado.Nombre,
                FotografiaUrl = sesion.Estudiante.FotografiaUrl
            })
            .ToListAsync(cancellationToken);

        var historial = sesiones
            .Select(sesion => new HistorialSesionResponseDto(
                sesion.Id,
                sesion.FechaSesion,
                sesion.HoraInicio,
                sesion.Estudiante,
                sesion.EstudianteEmail,
                sesion.Estado,
                s3Service.GeneratePresignedUrl(sesion.FotografiaUrl)
            ))
            .ToList();

        return Results.Ok(historial);
    }
}