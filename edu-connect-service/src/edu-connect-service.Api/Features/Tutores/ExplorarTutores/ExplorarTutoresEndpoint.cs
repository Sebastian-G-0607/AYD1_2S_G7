using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Storage;
using edu_connect_service.Api.Shared.Validation;
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
        IS3Service s3Service,
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

        var query = dbContext.Tutores
            .AsNoTracking()
            .Include(tutor => tutor.Usuario)
            .Include(tutor => tutor.TutorMaterias)
                .ThenInclude(tutorMateria => tutorMateria.Materia)
            .Where(tutor => tutor.Usuario.Estado.Nombre == "APROBADO");

        if (!string.IsNullOrWhiteSpace(filtros.Materia))
        {
            var materiaFiltro = filtros.Materia.Trim().ToLower();
            query = query.Where(tutor => tutor.TutorMaterias
                .Any(tutorMateria => EF.Functions.Like(tutorMateria.Materia.Nombre.ToLower(), $"%{materiaFiltro}%")));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Universidad))
        {
            var universidadFiltro = filtros.Universidad.Trim().ToLower();
            query = query.Where(tutor => EF.Functions.Like(tutor.Universidad.ToLower(), $"%{universidadFiltro}%"));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Genero))
        {
            var generoFiltro = GeneroValidator.TryNormalize(filtros.Genero, out var generoNormalizado)
                ? generoNormalizado
                : filtros.Genero.Trim().ToLower();

            query = query.Where(tutor => tutor.Genero == generoFiltro);
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
            s3Service.GeneratePresignedUrl(tutor.FotografiaUrl) ?? tutor.FotografiaUrl,
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
