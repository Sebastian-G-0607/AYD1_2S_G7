using edu_connect_service.Api.Shared.Authorization;

namespace edu_connect_service.Api.UnitTests.Shared.Authorization;

public class AppRolesTests
{
    [Fact]
    public void Constants_ShouldHaveExpectedValues()
    {
        Assert.Equal("Admin", AppRoles.Admin);
        Assert.Equal("Admin", AppRoles.Administrador);
        Assert.Equal("Estudiante", AppRoles.Estudiante);
        Assert.Equal("Tutor", AppRoles.Tutor);
    }

    [Fact]
    public void All_ShouldContainAllRoles()
    {
        Assert.Equal(3, AppRoles.All.Count);
        Assert.Contains(AppRoles.Admin, AppRoles.All);
        Assert.Contains(AppRoles.Estudiante, AppRoles.All);
        Assert.Contains(AppRoles.Tutor, AppRoles.All);
    }

    [Theory]
    [InlineData("Admin", true)]
    [InlineData("Estudiante", true)]
    [InlineData("Tutor", true)]
    [InlineData("SuperAdmin", false)]
    [InlineData("admin", false)]
    [InlineData("", false)]
    [InlineData("Invitado", false)]
    public void IsValidRole_ShouldValidateCorrectly(string role, bool expected)
    {
        var result = AppRoles.IsValidRole(role);

        Assert.Equal(expected, result);
    }
}
