using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class ObtenerEstudiantePorIdEndpoint
{
    public static void MapObtenerEstudiantePorId(this IEndpointRouteBuilder app)
    {
        app.MapGet("/estudiantes/{id:int}", HandleAsync)
            .Produces<EstudianteActivoResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var estudiante = await dbContext.Estudiantes
            .AsNoTracking()
            .Include(e => e.Usuario)
                .ThenInclude(u => u.Estado)
            .FirstOrDefaultAsync(e => e.UsuarioId == id, cancellationToken);

        if (estudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: $"No se encontró un estudiante con ID {id}."
            );
        }

        var response = new EstudianteActivoResponseDto(
            estudiante.UsuarioId,
            estudiante.Nombre,
            estudiante.Apellido,
            estudiante.Carnet,
            estudiante.Genero,
            estudiante.FechaNacimiento,
            estudiante.Usuario.Correo,
            estudiante.FotografiaUrl,
            estudiante.Direccion,
            estudiante.Telefono,
            estudiante.Usuario.FechaRegistro,
            estudiante.Usuario.Estado.Nombre,
            estudiante.DocumentoCarnetUrl
        );

        return Results.Ok(response);
    }
}
