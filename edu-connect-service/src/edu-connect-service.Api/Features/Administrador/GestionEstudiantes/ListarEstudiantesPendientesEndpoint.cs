using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class ListarEstudiantesPendientesEndpoint
{
    public static void MapListarEstudiantesPendientes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/estudiantes/pendientes", HandleAsync)
            .Produces<List<EstudiantePendienteResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        app.MapGet("/estudiantes", HandleAsync)
            .Produces<List<EstudiantePendienteResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var estudiantesPendientes = await dbContext.Estudiantes
            .AsNoTracking()
            .Include(e => e.Usuario)
            .ThenInclude(u => u.Estado)
            .Where(e => e.Usuario.Estado.Nombre == "PENDIENTE")
            .OrderBy(e => e.Usuario.FechaRegistro)
            .Select(e => new EstudiantePendienteResponseDto(
                e.UsuarioId,
                e.Nombre,
                e.Apellido,
                e.Carnet,
                e.Genero,
                e.FechaNacimiento,
                e.Usuario.Correo,
                e.FotografiaUrl,
                e.Direccion,
                e.Telefono,
                e.Usuario.FechaRegistro
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(estudiantesPendientes);
    }
}

