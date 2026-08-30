namespace edu_connect_service.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdmin", policy => policy.RequireRole(AppRoles.Admin))
            .AddPolicy("RequireEstudiante", policy => policy.RequireRole(AppRoles.Estudiante))
            .AddPolicy("RequireTutor", policy => policy.RequireRole(AppRoles.Tutor));

        return services;
    }
}
