using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Storage;
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
        IS3Service s3Service,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nombre obligatorio",
                detail: "El nombre es obligatorio y no puede estar vacío."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Apellido obligatorio",
                detail: "El apellido es obligatorio y no puede estar vacío."
            );
        }

        if (!CarnetValidator.IsValid(request.Carnet))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet debe ser numérico y contener entre 6 y 10 dígitos."
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

        if (!TelefonoValidator.IsValid(request.Telefono))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Teléfono inválido",
                detail: "El teléfono debe ser numérico y contener exactamente 8 dígitos."
            );
        }

        if (request.FechaNacimiento == default)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento obligatoria",
                detail: "La fecha de nacimiento es obligatoria y debe tener formato YYYY-MM-DD."
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.FechaNacimiento > today)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento inválida",
                detail: "La fecha de nacimiento no puede ser una fecha futura."
            );
        }

        var edad = today.Year - request.FechaNacimiento.Year;
        if (request.FechaNacimiento > today.AddYears(-edad))
        {
            edad--;
        }

        if (edad < 16)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Edad mínima no cumplida",
                detail: "El estudiante debe tener una edad mínima de 16 años cumplidos."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Direccion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección obligatoria",
                detail: "La dirección de residencia es obligatoria y no puede estar vacía."
            );
        }

        if (request.Fotografia is not null && request.Fotografia.Length > 0 && !ImageFileValidator.IsValidImage(request.Fotografia))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fotografía inválida",
                detail: "El archivo de fotografía debe ser una imagen válida (.jpg, .jpeg, .png, .webp)."
            );
        }

        if (!EmailValidator.IsValid(request.Correo))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Correo inválido",
                detail: "El formato del correo electrónico no es válido."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña obligatoria",
                detail: "La contraseña es obligatoria."
            );
        }

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword) || request.Password != request.ConfirmPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseñas no coinciden",
                detail: "La contraseña y la confirmación de contraseña no coinciden exactamente."
            );
        }

        if (!PasswordValidator.IsValid(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña inválida",
                detail: "La contraseña debe tener un mínimo de 8 caracteres, al menos 1 letra mayúscula, 1 letra minúscula y 1 número."
            );
        }

        var normalizedEmail = request.Correo.Trim().ToLowerInvariant();
        var normalizedCarnet = request.Carnet.Trim();

        var emailExists = await dbContext.Usuarios.AnyAsync(u => u.Correo == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Correo duplicado",
                detail: "El correo electrónico ya se encuentra registrado en el sistema."
            );
        }

        var carnetExists = await dbContext.Estudiantes.AnyAsync(e => e.Carnet == normalizedCarnet, cancellationToken);
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

        string? fotografiaKey = null;
        if (request.Fotografia is not null && request.Fotografia.Length > 0)
        {
            fotografiaKey = await s3Service.UploadImageAsync(request.Fotografia, "estudiantes", cancellationToken);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var usuario = new Usuario
        {
            Correo = normalizedEmail,
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
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            Carnet = normalizedCarnet,
            Genero = generoNormalizado,
            Direccion = request.Direccion.Trim(),
            Telefono = request.Telefono.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            FotografiaUrl = fotografiaKey
        };

        dbContext.Estudiantes.Add(estudiante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var fotografiaPresignedUrl = s3Service.GeneratePresignedUrl(estudiante.FotografiaUrl);

        var response = new EstudianteResponseDto(
            estudiante.UsuarioId,
            estudiante.Nombre,
            estudiante.Apellido,
            estudiante.Carnet,
            estudiante.Genero,
            estudiante.Direccion,
            estudiante.Telefono,
            estudiante.FechaNacimiento,
            fotografiaPresignedUrl,
            usuario.Correo,
            rolEstudiante.Nombre,
            estadoPendiente.Nombre,
            usuario.FechaRegistro
        );

        return Results.Created($"/api/estudiantes/{estudiante.UsuarioId}", response);
    }
}
