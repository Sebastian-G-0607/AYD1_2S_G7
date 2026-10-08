using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class ActualizarEstudianteEndpoint
{
    public static void MapActualizarEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPut("/estudiantes/{id:int}", HandleAsync)
            .Produces<ActualizarEstudianteAdminResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        [FromBody] ActualizarEstudianteAdminRequestDto request,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var estudiante = await dbContext.Estudiantes
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

        if (estudiante.Usuario.Estado.Nombre != "APROBADO")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Operación no permitida",
                detail: $"Solo se puede editar a estudiantes activos. El estado actual del estudiante es '{estudiante.Usuario.Estado.Nombre}'."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nombre obligatorio",
                detail: "El nombre del estudiante es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Apellido obligatorio",
                detail: "El apellido del estudiante es obligatorio."
            );
        }

        if (!CarnetValidator.IsValid(request.Carnet))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet o ID debe ser numérico y contener entre 6 y 10 dígitos."
            );
        }

        if (!GeneroValidator.TryNormalize(request.Genero, out var generoNormalizado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Género inválido",
                detail: "El género debe ser 'masculino' ('m') o 'femenino' ('f')."
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.FechaNacimiento == default || request.FechaNacimiento > today)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento inválida",
                detail: "La fecha de nacimiento es obligatoria y no puede ser futura."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Direccion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección obligatoria",
                detail: "La dirección de residencia es obligatoria."
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Telefono) && !TelefonoValidator.IsValid(request.Telefono))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Teléfono inválido",
                detail: "El teléfono debe ser numérico y contener exactamente 8 dígitos."
            );
        }

        var carnetNormalizado = request.Carnet.Trim();
        var duplicateCarnet = await dbContext.Estudiantes
            .AnyAsync(e => e.UsuarioId != id && e.Carnet == carnetNormalizado, cancellationToken);

        if (duplicateCarnet)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "Ya existe otro estudiante registrado con el mismo carnet o ID."
            );
        }

        // Modificación de campos del perfil (el correo electrónico permanece inmutable en estudiante.Usuario.Correo)
        estudiante.Nombre = request.Nombre.Trim();
        estudiante.Apellido = request.Apellido.Trim();
        estudiante.Carnet = carnetNormalizado;
        estudiante.Genero = generoNormalizado;
        estudiante.FechaNacimiento = request.FechaNacimiento;
        estudiante.Direccion = request.Direccion.Trim();
        estudiante.Telefono = request.Telefono?.Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(request.FotografiaUrl))
        {
            estudiante.FotografiaUrl = request.FotografiaUrl.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.DocumentoCarnetUrl))
        {
            estudiante.DocumentoCarnetUrl = request.DocumentoCarnetUrl.Trim();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ActualizarEstudianteAdminResponseDto(
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
            "Estudiante actualizado exitosamente por el administrador.",
            estudiante.DocumentoCarnetUrl
        );

        return Results.Ok(response);
    }
}
