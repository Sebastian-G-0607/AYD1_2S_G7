using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class ListarEstudiantesActivosEndpoint
{
    public static void MapListarEstudiantesActivos(this IEndpointRouteBuilder app)
    {
        app.MapGet("/estudiantes/activos", HandleAsync)
            .Produces<List<EstudianteActivoResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var estudiantesActivos = await dbContext.Estudiantes
            .AsNoTracking()
            .Include(e => e.Usuario)
            .ThenInclude(u => u.Estado)
            .Where(e => e.Usuario.Estado.Nombre == "APROBADO")
            .OrderBy(e => e.Nombre)
            .ThenBy(e => e.Apellido)
            .Select(e => new EstudianteActivoResponseDto(
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
                e.Usuario.FechaRegistro,
                e.Usuario.Estado.Nombre
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(estudiantesActivos);
    }
}

