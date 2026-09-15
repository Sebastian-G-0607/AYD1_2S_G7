using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Storage;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Estudiantes.Perfil;

public static class PerfilEstudianteEndpoint
{
    public static void MapPerfilEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapGet("/perfil", ObtenerPerfilAsync)
            .RequireAuthorization()
            .Produces<PerfilEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPut("/perfil", ActualizarPerfilAsync)
            .RequireAuthorization()
            .DisableAntiforgery()
            .Accepts<ActualizarPerfilEstudianteRequestDto>("multipart/form-data")
            .Produces<PerfilEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapPut("/perfil/password", CambiarPasswordAsync)
            .RequireAuthorization()
            .Accepts<CambiarPasswordEstudianteRequestDto>("application/json")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ObtenerPerfilAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        IS3Service s3Service,
        CancellationToken cancellationToken)
    {
        var authError = ValidarEstudianteAutenticado(user, out var idUsuario);

        if (authError is not null)
        {
            return authError;
        }

        var estudiante = await dbContext.Estudiantes
            .AsNoTracking()
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(
                e => e.UsuarioId == idUsuario,
                cancellationToken
            );

        if (estudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: "No se encontró información del estudiante autenticado."
            );
        }

        return Results.Ok(CrearResponse(estudiante, s3Service));
    }

    private static async Task<IResult> ActualizarPerfilAsync(
        [FromForm] ActualizarPerfilEstudianteRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        IS3Service s3Service,
        CancellationToken cancellationToken)
    {
        var authError = ValidarEstudianteAutenticado(user, out var idUsuario);

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

        if (!CarnetValidator.IsValid(request.Carnet))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet debe ser numérico y contener entre 6 y 10 dígitos."
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

        var estudiante = await dbContext.Estudiantes
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(
                e => e.UsuarioId == idUsuario,
                cancellationToken
            );

        if (estudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: "No se encontró información del estudiante autenticado."
            );
        }

        var carnetNormalizado = request.Carnet.Trim();

        var carnetExiste = await dbContext.Estudiantes
            .AnyAsync(
                e =>
                    e.Carnet == carnetNormalizado &&
                    e.UsuarioId != idUsuario,
                cancellationToken
            );

        if (carnetExiste)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "El carnet ingresado pertenece a otro estudiante."
            );
        }

        string? nuevaFotografiaKey = null;
        var fotografiaAnteriorKey = estudiante.FotografiaUrl;

        if (request.Fotografia is not null &&
            request.Fotografia.Length > 0)
        {
            try
            {
                nuevaFotografiaKey = await s3Service.UploadImageAsync(
                    request.Fotografia,
                    "estudiantes",
                    cancellationToken
                );
            }
            catch (Exception)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Error al almacenar fotografía",
                    detail: "No se pudo actualizar la fotografía del perfil."
                );
            }
        }

        estudiante.Nombre = request.Nombre.Trim();
        estudiante.Apellido = request.Apellido.Trim();
        estudiante.Carnet = carnetNormalizado;
        estudiante.Genero = generoNormalizado;
        estudiante.Direccion = request.Direccion.Trim();
        estudiante.Telefono = request.Telefono.Trim();
        estudiante.FechaNacimiento = request.FechaNacimiento;

        if (!string.IsNullOrWhiteSpace(nuevaFotografiaKey))
        {
            estudiante.FotografiaUrl = nuevaFotografiaKey;
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            if (!string.IsNullOrWhiteSpace(nuevaFotografiaKey))
            {
                await s3Service.DeleteImageAsync(
                    nuevaFotografiaKey,
                    CancellationToken.None
                );
            }

            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al actualizar perfil",
                detail: "Ocurrió un error al guardar los cambios del perfil."
            );
        }

        if (!string.IsNullOrWhiteSpace(nuevaFotografiaKey) &&
            !string.IsNullOrWhiteSpace(fotografiaAnteriorKey) &&
            fotografiaAnteriorKey != nuevaFotografiaKey)
        {
            try
            {
                await s3Service.DeleteImageAsync(
                    fotografiaAnteriorKey,
                    CancellationToken.None
                );
            }
            catch
            {
                // El perfil ya fue actualizado correctamente.
                // Una falla al eliminar la imagen anterior no debe revertirlo.
            }
        }

        return Results.Ok(CrearResponse(estudiante, s3Service));
    }

    private static async Task<IResult> CambiarPasswordAsync(
        CambiarPasswordEstudianteRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var authError = ValidarEstudianteAutenticado(user, out var idUsuario);

        if (authError is not null)
        {
            return authError;
        }

        if (string.IsNullOrWhiteSpace(request.PasswordActual))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña actual requerida",
                detail: "Debe ingresar la contraseña actual para cambiarla."
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

        if (string.IsNullOrWhiteSpace(request.ConfirmarNuevaPassword) ||
            request.NuevaPassword != request.ConfirmarNuevaPassword)
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

    private static IResult? ValidarEstudianteAutenticado(
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
                "Estudiante",
                StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden administrar su perfil."
            );
        }

        return null;
    }

    private static PerfilEstudianteResponseDto CrearResponse(
        Estudiante estudiante,
        IS3Service s3Service)
    {
        var fotografiaUrl =
            s3Service.GeneratePresignedUrl(estudiante.FotografiaUrl)
            ?? estudiante.FotografiaUrl;

        return new PerfilEstudianteResponseDto(
            estudiante.UsuarioId,
            estudiante.Nombre,
            estudiante.Apellido,
            estudiante.Carnet,
            estudiante.Genero,
            estudiante.Direccion,
            estudiante.Telefono,
            estudiante.FechaNacimiento,
            fotografiaUrl,
            estudiante.Usuario.Correo
        );
    }
}