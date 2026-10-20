using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.BackgroundTasks;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Auth.ReenviarTokenEmail;

public static class ReenviarTokenEmailEndpoint
{
    public static void MapReenviarTokenEmail(this IEndpointRouteBuilder app)
    {
        app.MapPost("/email-validation-tokens", ReenviarAsync)
            .RequireAuthorization("RequireEmailValidation")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> ReenviarAsync(
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        ITokenCorreoService tokenCorreoService,
        IBackgroundTaskQueue taskQueue,
        CancellationToken cancellationToken)
    {
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

        if (usuario.CorreoValidado)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Correo ya validado",
                detail: "El correo electrónico de este usuario ya ha sido verificado."
            );
        }

        var ultimoToken = await dbContext.TokensCorreo
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.FechaGeneracion)
            .FirstOrDefaultAsync(cancellationToken);

        if (ultimoToken is not null)
        {
            ultimoToken.Revocado = true;
        }

        var nuevoCodigo = tokenCorreoService.GenerarToken();
        var ahora = DateTime.UtcNow;

        var nuevoToken = new TokenCorreo
        {
            UsuarioId = usuarioId,
            Token = nuevoCodigo,
            FechaGeneracion = ahora,
            FechaExpiracion = ahora.AddHours(24),
            Revocado = false
        };

        dbContext.TokensCorreo.Add(nuevoToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var correoClaim = user.FindFirstValue("correo") ?? user.FindFirstValue(ClaimTypes.Email);
        var destinoCorreo = !string.IsNullOrWhiteSpace(correoClaim) ? correoClaim : usuario.Correo;
        var nombre = usuario.Estudiante?.Nombre ?? usuario.Tutor?.Nombre ?? "Usuario";

        await taskQueue.QueueBackgroundWorkItemAsync(async (serviceProvider, ct) =>
        {
            var emailService = serviceProvider.GetRequiredService<IEmailService>();
            await emailService.SendReenvioTokenVerificacionAsync(destinoCorreo, nombre, nuevoCodigo, ct);
        });

        return Results.Accepted();
    }
}
