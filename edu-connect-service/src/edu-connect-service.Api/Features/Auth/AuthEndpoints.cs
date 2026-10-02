using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Features.Auth.Me;
using edu_connect_service.Api.Features.Auth.AdminTwoFactor;
using edu_connect_service.Api.Features.Auth.ValidarEmail;
using edu_connect_service.Api.Features.Auth.ReenviarTokenEmail;

namespace edu_connect_service.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("/auth");
        authGroup.MapLogin();
        authGroup.MapMe();
        authGroup.MapValidarEmail();
        authGroup.MapReenviarTokenEmail();

        var apiGroup = app.MapGroup("/api");
        apiGroup.MapLogin();
        apiGroup.MapMe();
        apiGroup.MapValidarEmail();
        apiGroup.MapReenviarTokenEmail();

        app.MapAdminTwoFactor();
    }
}
