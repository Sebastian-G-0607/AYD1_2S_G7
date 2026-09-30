using edu_connect_service.Api.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace edu_connect_service.Api.UnitTests.Shared.Authorization;

public class AuthorizationExtensionsTests
{
    [Fact]
    public void AddCustomAuthorization_ShouldRegisterAuthorizationAndPolicies()
    {
        var services = new ServiceCollection();

        services.AddCustomAuthorization();

        var serviceProvider = services.BuildServiceProvider();
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        var requireAdmin = authOptions.GetPolicy("RequireAdmin");
        Assert.NotNull(requireAdmin);
        Assert.Contains(requireAdmin.Requirements, r => r is Microsoft.AspNetCore.Authorization.Infrastructure.RolesAuthorizationRequirement rolesReq && rolesReq.AllowedRoles.Contains(AppRoles.Admin));

        var requireEstudiante = authOptions.GetPolicy("RequireEstudiante");
        Assert.NotNull(requireEstudiante);
        Assert.Contains(requireEstudiante.Requirements, r => r is Microsoft.AspNetCore.Authorization.Infrastructure.RolesAuthorizationRequirement rolesReq && rolesReq.AllowedRoles.Contains(AppRoles.Estudiante));

        var requireTutor = authOptions.GetPolicy("RequireTutor");
        Assert.NotNull(requireTutor);
        Assert.Contains(requireTutor.Requirements, r => r is Microsoft.AspNetCore.Authorization.Infrastructure.RolesAuthorizationRequirement rolesReq && rolesReq.AllowedRoles.Contains(AppRoles.Tutor));
    }
}
