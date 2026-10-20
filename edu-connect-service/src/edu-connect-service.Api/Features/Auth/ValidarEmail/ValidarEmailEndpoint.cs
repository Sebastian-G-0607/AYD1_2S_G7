using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Emails;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace edu_connect_service.Api.Features.Auth.ValidarEmail;

public static class ValidarEmailEndpoint
{
    public static void MapValidarEmail(this IEndpointRouteBuilder app)
    {
        app.MapPost("/email-validations", ValidarAsync)
            .RequireAuthorization("RequireEmailValidation")
            .Produces<TokenResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost("/email-verifications", ValidarAsync)
            .RequireAuthorization("RequireEmailValidation")
            .Produces<TokenResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> ValidarAsync(
        [FromBody] ValidarEmailRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        IJwtTokenService jwtTokenService,
        IS3Service s3Service,
        IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken)
    {
        var codigo = request.ObtenerCodigo();

        if (string.IsNullOrWhiteSpace(codigo) || !Regex.IsMatch(codigo, @"^\d{6}$"))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Código inválido",
                detail: "El código de verificación debe ser numérico y contener exactamente 6 dígitos."
            );
        }

        var idUsuarioClaim = user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(idUsuarioClaim) || !int.TryParse(idUsuarioClaim, out var usuarioId))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "No autorizado",
                detail: "El token de validación es inválido o no contiene un identificador de usuario válido."
            );
        }

        var usuario = await dbContext.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Estado)
            .Include(u => u.Estudiante)
            .Include(u => u.Tutor)
            .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

        if (usuario is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Usuario no encontrado",
                detail: "No se encontró el usuario asociado a la sesión de validación."
            );
        }

        var tokenRegistro = await dbContext.TokensCorreo
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.FechaGeneracion)
            .FirstOrDefaultAsync(cancellationToken);

        if (tokenRegistro is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Código no encontrado",
                detail: "No se encontró ningún código de verificación emitido para esta cuenta."
            );
        }

        if (tokenRegistro.Revocado)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Código revocado",
                detail: "El código de verificación ya ha sido utilizado o ha sido revocado."
            );
        }

        if (DateTime.UtcNow > tokenRegistro.FechaExpiracion)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Código expirado",
                detail: "El código de verificación ha expirado. Por favor, solicita uno nuevo."
            );
        }

        if (!string.Equals(tokenRegistro.Token.Trim(), codigo, StringComparison.Ordinal))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Código incorrecto",
                detail: "El código de verificación ingresado no es válido."
            );
        }

        tokenRegistro.Revocado = true;
        usuario.CorreoValidado = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        var rol = usuario.Rol.Nombre;
        var authToken = jwtTokenService.GenerateToken(usuario.Id, usuario.Correo, rol);
        var nombre = usuario.Estudiante?.Nombre ?? usuario.Tutor?.Nombre;
        var apellido = usuario.Estudiante?.Apellido ?? usuario.Tutor?.Apellido;
        var fotografiaKey = usuario.Estudiante?.FotografiaUrl ?? usuario.Tutor?.FotografiaUrl;
        var fotografiaUrl = !string.IsNullOrWhiteSpace(fotografiaKey)
            ? s3Service.GeneratePresignedUrl(fotografiaKey)
            : null;

        var response = new TokenResponseDto(
            true,
            authToken,
            "Bearer",
            jwtOptions.Value.ExpirationMinutes * 60,
            usuario.Id,
            usuario.Correo,
            rol,
            nombre,
            apellido,
            fotografiaUrl
        );

        return Results.Ok(response);
    }
}
