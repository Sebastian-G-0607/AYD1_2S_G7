using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;

public static class RegistrarEstudianteEndpoint
{
    public static void MapRegistrarEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPost("/registro", HandleAsync)
            .DisableAntiforgery()
            .Accepts<RegistrarEstudianteRequestDto>("multipart/form-data")
            .Produces<EstudianteResponseDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("/registros", HandleAsync)
            .DisableAntiforgery()
            .Accepts<RegistrarEstudianteRequestDto>("multipart/form-data")
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] RegistrarEstudianteRequestDto request,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseñas no coinciden",
                detail: "La contraseña y la confirmación de contraseña no coinciden."
            );
        }

        if (!PasswordValidator.IsValid(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña inválida",
                detail: "La contraseña debe tener un mínimo de 8 caracteres, incluyendo al menos una letra minúscula, una mayúscula y un número."
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

        var emailExists = await dbContext.Usuarios.AnyAsync(u => u.Correo == request.Correo, cancellationToken);
        if (emailExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Correo duplicado",
                detail: "El correo electrónico ya se encuentra registrado en el sistema."
            );
        }

        var carnetExists = await dbContext.Estudiantes.AnyAsync(e => e.Carnet == request.Carnet, cancellationToken);
        if (carnetExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "El carnet universitario ya se encuentra registrado."
            );
        }

        var rolEstudiante = await dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == "Estudiante", cancellationToken);
        if (rolEstudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "El rol 'Estudiante' no está configurado en el sistema."
            );
        }

        var estadoPendiente = await dbContext.EstadosUsuarios.FirstOrDefaultAsync(e => e.Nombre == "PENDIENTE", cancellationToken);
        if (estadoPendiente is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "El estado de usuario 'PENDIENTE' no está configurado en el sistema."
            );
        }

        string? fotografiaUrl = null;
        if (request.Fotografia is not null && request.Fotografia.Length > 0)
        {
            // TODO: Implementar lógica de guardado en almacenamiento de objetos (S3 / Oracle Object Storage).
            fotografiaUrl = $"/uploads/estudiantes/{Guid.NewGuid():N}.jpg";
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var usuario = new Usuario
        {
            Correo = request.Correo,
            PasswordHash = passwordHash,
            RolId = rolEstudiante.Id,
            EstadoId = estadoPendiente.Id,
            FechaRegistro = DateTime.UtcNow
        };

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var estudiante = new Estudiante
        {
            UsuarioId = usuario.Id,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Carnet = request.Carnet,
            Genero = generoNormalizado,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            FechaNacimiento = request.FechaNacimiento,
            FotografiaUrl = fotografiaUrl
        };

        dbContext.Estudiantes.Add(estudiante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new EstudianteResponseDto(
            estudiante.UsuarioId,
            estudiante.Nombre,
            estudiante.Apellido,
            estudiante.Carnet,
            estudiante.Genero,
            estudiante.Direccion,
            estudiante.Telefono,
            estudiante.FechaNacimiento,
            estudiante.FotografiaUrl,
            usuario.Correo,
            rolEstudiante.Nombre,
            estadoPendiente.Nombre,
            usuario.FechaRegistro
        );

        return Results.Created($"/api/estudiantes/{estudiante.UsuarioId}", response);
    }
}
