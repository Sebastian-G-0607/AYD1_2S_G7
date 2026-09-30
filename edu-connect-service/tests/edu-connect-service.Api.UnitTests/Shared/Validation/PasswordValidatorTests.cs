using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class PasswordValidatorTests
{
    [Theory]
    [InlineData("Password123")]
    [InlineData("Pass1234")]
    [InlineData("A1b2c3d4")]
    [InlineData("ContraseñaSegura1")]
    [InlineData("Abcdefgh1!")]
    public void IsValid_WithValidPassword_ReturnsTrue(string password)
    {
        var result = PasswordValidator.IsValid(password);

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Pass1")]
    [InlineData("password123")]
    [InlineData("PASSWORD123")]
    [InlineData("Password")]
    [InlineData("12345678")]
    public void IsValid_WithInvalidPassword_ReturnsFalse(string? password)
    {
        var result = PasswordValidator.IsValid(password);

        Assert.False(result);
    }
}
