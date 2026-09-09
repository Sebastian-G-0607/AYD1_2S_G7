using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionUsuarios;

public static class ListarUsuariosDadosDeBajaEndpoint
{
    public static void MapListarUsuariosDadosDeBaja(this IEndpointRouteBuilder app)
    {
        app.MapGet("/usuarios/dados-de-baja", HandleAsync)
            .Produces<List<UsuarioDadoDeBajaResponseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var usuariosDadosDeBaja = await dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.Rol)
            .Include(u => u.Estado)
            .Include(u => u.Estudiante)
            .Include(u => u.Tutor)
            .Include(u => u.Administrador)
            .Where(u => u.Estado.Nombre == "INACTIVO")
            .OrderBy(u => u.FechaBaja ?? u.FechaRegistro)
            .Select(u => new UsuarioDadoDeBajaResponseDto(
                u.Id,
                u.Correo,
                u.Rol.Nombre,
                u.Estado.Nombre,
                u.Rol.Nombre == "Estudiante"
                    ? "Estudiante"
                    : u.Rol.Nombre == "Tutor"
                        ? "Tutor"
                        : u.Rol.Nombre == "Admin"
                            ? "Administrador"
                            : "Usuario",
                u.Rol.Nombre == "Estudiante" && u.Estudiante != null
                    ? u.Estudiante.Nombre + " " + u.Estudiante.Apellido
                    : u.Rol.Nombre == "Tutor" && u.Tutor != null
                        ? u.Tutor.Nombre + " " + u.Tutor.Apellido
                        : u.Rol.Nombre == "Admin" && u.Administrador != null
                            ? "Administrador del sistema"
                            : null,
                u.Rol.Nombre == "Estudiante" && u.Estudiante != null
                    ? u.Estudiante.Carnet
                    : u.Rol.Nombre == "Tutor" && u.Tutor != null
                        ? u.Tutor.CarnetId
                        : null,
                u.FechaRegistro,
                u.FechaBaja,
                u.MotivoBaja
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(usuariosDadosDeBaja);
    }
}
