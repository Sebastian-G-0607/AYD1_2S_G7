using Microsoft.AspNetCore.Authorization;

namespace edu_connect_service.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(ctx =>
                    !ctx.User.HasClaim(c => c.Type == "scope" && c.Value == "email_validation") &&
                    ctx.User.HasClaim(c => (c.Type == "rol" || c.Type == System.Security.Claims.ClaimTypes.Role) && AppRoles.IsValidRole(c.Value)))
                .Build())
            .AddPolicy("RequireEmailValidation", policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "email_validation"))
            .AddPolicy("RequireAdmin", policy => policy.RequireRole(AppRoles.Admin))
            .AddPolicy("RequireEstudiante", policy => policy.RequireRole(AppRoles.Estudiante))
            .AddPolicy("RequireTutor", policy => policy.RequireRole(AppRoles.Tutor));

        return services;
    }
}
