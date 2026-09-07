using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class ListarTutoresActivosEndpoint
{
    public static void MapListarTutoresActivos(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tutores/activos", HandleAsync)
            .Produces<List<TutorActivoResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var tutoresActivos = await dbContext.Tutores
            .AsNoTracking()
            .Include(t => t.Usuario)
            .ThenInclude(u => u.Estado)
            .Include(t => t.TutorMaterias)
            .ThenInclude(tm => tm.Materia)
            .Where(t => t.Usuario.Estado.Nombre == "APROBADO")
            .OrderBy(t => t.Nombre)
            .ThenBy(t => t.Apellido)
            .ToListAsync(cancellationToken);

        var response = tutoresActivos.Select(t =>
        {
            var materias = t.TutorMaterias.Select(tm => tm.Materia.Nombre).ToList();
            var especialidad = materias.Count > 0 ? string.Join(", ", materias) : "Sin especialidad registrada";

            return new TutorActivoResponseDto(
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
                t.Usuario.FechaRegistro,
                t.Usuario.Estado.Nombre
            );
        }).ToList();

        return Results.Ok(response);
    }
}

