using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.Perfil;

public static class PerfilTutorEndpoint
{
    public static void MapPerfilTutor(this IEndpointRouteBuilder app)
    {
        app.MapGet("/perfil", ObtenerPerfilAsync)
            .RequireAuthorization()
            .Produces<PerfilTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPut("/perfil", ActualizarPerfilAsync)
            .RequireAuthorization()
            .DisableAntiforgery()
            .Accepts<ActualizarPerfilTutorRequestDto>("multipart/form-data")
            .Produces<PerfilTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapPut("/perfil/password", CambiarPasswordAsync)
            .RequireAuthorization()
            .Accepts<CambiarPasswordTutorRequestDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ObtenerPerfilAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var authError = ValidarTutorAutenticado(user, out var idUsuario);

        if (authError is not null)
        {
            return authError;
        }

        var tutor = await dbContext.Tutores
            .AsNoTracking()
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(
                t => t.UsuarioId == idUsuario,
                cancellationToken
            );

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "No se encontró información del tutor autenticado."
            );
        }

        return Results.Ok(CrearResponse(tutor));
    }

    private static async Task<IResult> ActualizarPerfilAsync(
        [FromForm] ActualizarPerfilTutorRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var authError = ValidarTutorAutenticado(user, out var idUsuario);

        if (authError is not null)
        {
            return authError;
        }

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nombre obligatorio",
                detail: "El nombre es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Apellido obligatorio",
                detail: "El apellido es obligatorio."
            );
        }

        if (!CarnetValidator.IsValid(request.CarnetId))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet debe ser numérico y contener entre 6 y 10 dígitos."
            );
        }

        if (!DpiValidator.IsValid(request.NumeroIdentificacion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Documento de identificación inválido",
                detail: "El DPI / documento de identificación debe contener exactamente 13 dígitos."
            );
        }

        if (!GeneroValidator.TryNormalize(
                request.Genero,
                out var generoNormalizado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Género inválido",
                detail: "El género debe ser masculino o femenino."
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

        if (!TelefonoValidator.IsValid(request.Telefono))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Teléfono inválido",
                detail: "El teléfono debe contener exactamente 8 dígitos."
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (request.FechaNacimiento == default ||
            request.FechaNacimiento > today)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento inválida",
                detail: "La fecha de nacimiento es obligatoria y no puede ser futura."
            );
        }

        if (string.IsNullOrWhiteSpace(request.DireccionTutoria))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección de tutoría obligatoria",
                detail: "Debe especificar la dirección donde imparte tutorías."
            );
        }

        var currentYear = DateTime.UtcNow.Year;

        if (request.AnioInicio < 1980 ||
            request.AnioInicio > currentYear)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Año de inicio inválido",
                detail: $"El año de inicio debe estar entre 1980 y {currentYear}."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Universidad))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Universidad obligatoria",
                detail: "La universidad es obligatoria."
            );
        }

        if (request.Fotografia is not null &&
            request.Fotografia.Length > 0 &&
            !ImageFileValidator.IsValidImage(request.Fotografia))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fotografía inválida",
                detail: "El archivo proporcionado debe ser una imagen válida."
            );
        }

        var tutor = await dbContext.Tutores
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(
                t => t.UsuarioId == idUsuario,
                cancellationToken
            );

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "No se encontró información del tutor autenticado."
            );
        }

        var carnetExiste = await dbContext.Tutores
            .AnyAsync(
                t =>
                    t.CarnetId == request.CarnetId.Trim() &&
                    t.UsuarioId != idUsuario,
                cancellationToken
            );

        if (carnetExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "El carnet ingresado pertenece a otro tutor."
            );
        }

        var identificacionExiste = await dbContext.Tutores
            .AnyAsync(
                t =>
                    t.NumeroIdentificacion == request.NumeroIdentificacion.Trim() &&
                    t.UsuarioId != idUsuario,
                cancellationToken
            );

        if (identificacionExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Identificación duplicada",
                detail: "El número de identificación pertenece a otro tutor."
            );
        }

        tutor.Nombre = request.Nombre.Trim();
        tutor.Apellido = request.Apellido.Trim();
        tutor.CarnetId = request.CarnetId.Trim();
        tutor.NumeroIdentificacion = request.NumeroIdentificacion.Trim();
        tutor.Genero = generoNormalizado;
        tutor.Direccion = request.Direccion.Trim();
        tutor.Telefono = request.Telefono.Trim();
        tutor.FechaNacimiento = request.FechaNacimiento;
        tutor.DireccionTutoria = request.DireccionTutoria.Trim();
        tutor.AnioInicio = request.AnioInicio;
        tutor.Universidad = request.Universidad.Trim();

        if (request.Fotografia is not null &&
            request.Fotografia.Length > 0)
        {
            var extension = Path
                .GetExtension(request.Fotografia.FileName)
                .ToLowerInvariant();

            tutor.FotografiaUrl =
                $"/uploads/tutores/{Guid.NewGuid():N}{extension}";
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(CrearResponse(tutor));
    }

    private static async Task<IResult> CambiarPasswordAsync(
        CambiarPasswordTutorRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var authError = ValidarTutorAutenticado(user, out var idUsuario);

        if (authError is not null)
        {
            return authError;
        }

        if (string.IsNullOrWhiteSpace(request.PasswordActual))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña actual requerida",
                detail: "Debe ingresar la contraseña actual para continuar."
            );
        }

        if (string.IsNullOrWhiteSpace(request.NuevaPassword))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nueva contraseña requerida",
                detail: "Debe ingresar una nueva contraseña."
            );
        }

        if (request.NuevaPassword != request.ConfirmarNuevaPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseñas no coinciden",
                detail: "La nueva contraseña y su confirmación no coinciden."
            );
        }

        if (!PasswordValidator.IsValid(request.NuevaPassword))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña inválida",
                detail:
                    "La contraseña debe tener mínimo 8 caracteres, " +
                    "al menos una mayúscula, una minúscula y un número."
            );
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(
                u => u.Id == idUsuario,
                cancellationToken
            );

        if (usuario is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Usuario no encontrado",
                detail: "No se encontró el usuario autenticado."
            );
        }

        var passwordCorrecta = BCrypt.Net.BCrypt.Verify(
            request.PasswordActual,
            usuario.PasswordHash
        );

        if (!passwordCorrecta)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña actual incorrecta",
                detail: "La contraseña actual ingresada no es correcta."
            );
        }

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NuevaPassword
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new
        {
            mensaje = "Contraseña actualizada correctamente."
        });
    }

    private static IResult? ValidarTutorAutenticado(
        ClaimsPrincipal user,
        out int idUsuario)
    {
        idUsuario = 0;

        var idUsuarioClaim =
            user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioClaim, out idUsuario))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Usuario no autenticado",
                detail: "No fue posible identificar al usuario autenticado."
            );
        }

        var rol =
            user.FindFirstValue("rol")
            ?? user.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(
                rol,
                "Tutor",
                StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los tutores pueden administrar su perfil."
            );
        }

        return null;
    }

    private static PerfilTutorResponseDto CrearResponse(Tutor tutor)
    {
        return new PerfilTutorResponseDto(
            tutor.UsuarioId,
            tutor.Nombre,
            tutor.Apellido,
            tutor.CarnetId,
            tutor.NumeroIdentificacion,
            tutor.Genero,
            tutor.Direccion,
            tutor.Telefono,
            tutor.FechaNacimiento,
            tutor.FotografiaUrl,
            tutor.DireccionTutoria,
            tutor.AnioInicio,
            tutor.Universidad,
            tutor.Usuario.Correo
        );
    }
}