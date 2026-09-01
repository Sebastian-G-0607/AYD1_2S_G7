using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class ListarTutoresPendientesEndpoint
{
    public static void MapListarTutoresPendientes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tutores/pendientes", HandleAsync)
            .Produces<List<TutorPendienteResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        app.MapGet("/tutores", HandleAsync)
            .Produces<List<TutorPendienteResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var tutoresPendientes = await dbContext.Tutores
            .AsNoTracking()
            .Include(t => t.Usuario)
            .ThenInclude(u => u.Estado)
            .Include(t => t.TutorMaterias)
            .ThenInclude(tm => tm.Materia)
            .Where(t => t.Usuario.Estado.Nombre == "PENDIENTE")
            .OrderBy(t => t.Usuario.FechaRegistro)
            .ToListAsync(cancellationToken);

        var response = tutoresPendientes.Select(t =>
        {
            var materias = t.TutorMaterias.Select(tm => tm.Materia.Nombre).ToList();
            var especialidad = materias.Count > 0 ? string.Join(", ", materias) : "Sin especialidad registrada";

            return new TutorPendienteResponseDto(
                t.UsuarioId,
                t.Nombre,
                t.Apellido,
                t.CarnetId,
                t.NumeroIdentificacion,
                t.Genero,
                t.FechaNacimiento,
                t.Usuario.Correo,
                t.FotografiaUrl,
                especialidad,
                materias,
                t.DireccionTutoria,
                t.AnioInicio,
                t.Universidad,
                t.Direccion,
                t.Telefono,
                t.Usuario.FechaRegistro
            );
        }).ToList();

        return Results.Ok(response);
    }
}

