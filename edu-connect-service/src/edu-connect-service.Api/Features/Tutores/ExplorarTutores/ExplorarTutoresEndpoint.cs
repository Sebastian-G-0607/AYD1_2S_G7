using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ExplorarTutores;

public static class ExplorarTutoresEndpoint
{
    public static void MapExplorarTutores(this IEndpointRouteBuilder app)
    {
        app.MapGet("/explorar", HandleAsync)
            .RequireAuthorization()
            .Produces<List<TutorExploradoResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] ExplorarTutoresRequestDto filtros,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var idUsuarioClaim =
            user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioClaim, out var idEstudiante))
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
                detail: "Solo los estudiantes pueden explorar tutores."
            );
        }

        // Tutores con los que el estudiante ya tiene una sesión (en cualquier estado)
        // se excluyen de la exploración, según HU-16.
        var tutoresConSesionExistente = dbContext.Sesiones
            .Where(sesion => sesion.EstudianteId == idEstudiante)
            .Select(sesion => sesion.TutorId);

        var query = dbContext.Tutores
            .AsNoTracking()
            .Include(tutor => tutor.Usuario)
            .Include(tutor => tutor.TutorMaterias)
                .ThenInclude(tutorMateria => tutorMateria.Materia)
            .Where(tutor => tutor.Usuario.Estado.Nombre == "APROBADO")
            .Where(tutor => !tutoresConSesionExistente.Contains(tutor.UsuarioId));

        if (!string.IsNullOrWhiteSpace(filtros.Materia))
        {
            query = query.Where(tutor => tutor.TutorMaterias
                .Any(tutorMateria => tutorMateria.Materia.Nombre.Contains(filtros.Materia)));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Universidad))
        {
            query = query.Where(tutor => tutor.Universidad.Contains(filtros.Universidad));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Genero))
        {
            query = query.Where(tutor => tutor.Genero == filtros.Genero);
        }

        var anioActual = DateTime.UtcNow.Year;

        if (filtros.ExperienciaMinima.HasValue)
        {
            query = query.Where(tutor => anioActual - tutor.AnioInicio >= filtros.ExperienciaMinima.Value);
        }

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        if (filtros.EdadMinima.HasValue)
        {
            var fechaNacimientoMaxima = hoy.AddYears(-filtros.EdadMinima.Value);
            query = query.Where(tutor => tutor.FechaNacimiento <= fechaNacimientoMaxima);
        }

        if (filtros.EdadMaxima.HasValue)
        {
            var fechaNacimientoMinima = hoy.AddYears(-filtros.EdadMaxima.Value - 1);
            query = query.Where(tutor => tutor.FechaNacimiento > fechaNacimientoMinima);
        }

        var tutores = await query.ToListAsync(cancellationToken);

        var response = tutores.Select(tutor => new TutorExploradoResponseDto(
            tutor.UsuarioId,
            $"{tutor.Nombre} {tutor.Apellido}",
            tutor.TutorMaterias.Select(tm => tm.Materia.Nombre).ToList(),
            tutor.DireccionTutoria,
            tutor.FotografiaUrl,
            tutor.Universidad,
            tutor.Genero,
            anioActual - tutor.AnioInicio,
            CalcularEdad(tutor.FechaNacimiento, hoy)
        )).ToList();

        return Results.Ok(response);
    }

    private static int CalcularEdad(DateOnly fechaNacimiento, DateOnly hoy)
    {
        var edad = hoy.Year - fechaNacimiento.Year;

        if (hoy < fechaNacimiento.AddYears(edad))
        {
            edad--;
        }

        return edad;
    }
}
