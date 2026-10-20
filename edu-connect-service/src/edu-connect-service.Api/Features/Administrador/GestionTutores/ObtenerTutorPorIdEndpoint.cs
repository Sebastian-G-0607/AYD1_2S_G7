using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class ObtenerTutorPorIdEndpoint
{
    public static void MapObtenerTutorPorId(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tutores/{id:int}", HandleAsync)
            .Produces<TutorActivoResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var tutor = await dbContext.Tutores
            .AsNoTracking()
            .Include(t => t.Usuario)
                .ThenInclude(u => u.Estado)
            .Include(t => t.TutorMaterias)
                .ThenInclude(tm => tm.Materia)
            .FirstOrDefaultAsync(t => t.UsuarioId == id, cancellationToken);

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: $"No se encontró un tutor con ID {id}."
            );
        }

        var materias = tutor.TutorMaterias
            .Select(tm => tm.Materia?.Nombre ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        var especialidad = materias.Count > 0
            ? string.Join(", ", materias)
            : "Sin especialidad registrada";

        var response = new TutorActivoResponseDto(
            tutor.UsuarioId,
            tutor.Nombre,
            tutor.Apellido,
            tutor.CarnetId,
            tutor.NumeroIdentificacion,
            tutor.Genero,
            tutor.FechaNacimiento,
            tutor.Usuario.Correo,
            tutor.FotografiaUrl,
            especialidad,
            materias,
            tutor.DireccionTutoria,
            tutor.AnioInicio,
            tutor.Universidad,
            tutor.Direccion,
            tutor.Telefono,
            tutor.Usuario.FechaRegistro,
            tutor.Usuario.Estado.Nombre
        );

        return Results.Ok(response);
    }
}

