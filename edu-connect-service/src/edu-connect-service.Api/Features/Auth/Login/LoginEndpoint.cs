using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace edu_connect_service.Api.Features.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            [FromBody] LoginRequestDto request,
            edu_connect_serviceContext dbContext,
            IJwtTokenService jwtTokenService,
            IS3Service s3Service,
            IOptions<JwtOptions> jwtOptions,
            CancellationToken cancellationToken) =>
        {
            var user = await dbContext.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .Include(u => u.Estudiante)
                .Include(u => u.Tutor)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo, cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Credenciales inválidas",
                    detail: "El correo o la contraseña son incorrectos."
                );
            }

            if (user.Estado.Nombre != "APROBADO")
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Usuario no habilitado",
                    detail: $"El usuario no puede iniciar sesión porque su estado actual es '{user.Estado.Nombre}'."
                );
            }

            var rol = user.Rol.Nombre;
            var token = jwtTokenService.GenerateToken(user.Id, user.Correo, rol);

            var nombre = user.Estudiante?.Nombre ?? user.Tutor?.Nombre;
            var apellido = user.Estudiante?.Apellido ?? user.Tutor?.Apellido;
            var fotografiaKey = user.Estudiante?.FotografiaUrl ?? user.Tutor?.FotografiaUrl;
            var fotografiaUrl = !string.IsNullOrWhiteSpace(fotografiaKey)
                ? s3Service.GeneratePresignedUrl(fotografiaKey)
                : null;

            var response = new TokenResponseDto(
                token,
                "Bearer",
                jwtOptions.Value.ExpirationMinutes * 60,
                user.Id,
                user.Correo,
                rol,
                nombre,
                apellido,
                fotografiaUrl
            );

            return Results.Ok(response);
        })
        .Produces<TokenResponseDto>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}
