using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace edu_connect_service.Api.Features.Auth.Me;

public static class MeEndpoint
{
    public static void MapMe(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var idUsuario = user.FindFirstValue("id_usuario") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
            var correo = user.FindFirstValue("correo") ?? user.FindFirstValue(ClaimTypes.Email);
            var rol = user.FindFirstValue("rol") ?? user.FindFirstValue(ClaimTypes.Role);

            return Results.Ok(new UserProfileDto(idUsuario, correo, rol));
        })
        .RequireAuthorization()
        .Produces<UserProfileDto>()
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
