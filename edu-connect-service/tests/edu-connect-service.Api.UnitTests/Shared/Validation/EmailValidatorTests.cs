using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class EmailValidatorTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("estudiante@universidad.edu.gt")]
    [InlineData("user.name+tag@domain.co")]
    [InlineData("  test@example.com  ")]
    public void IsValid_WithValidEmail_ReturnsTrue(string email)
    {
        var result = EmailValidator.IsValid(email);

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("username@.com")]
    [InlineData("username@domain")]
    [InlineData("username@domain.")]
    [InlineData("username@domain..com")]
    [InlineData("username space@domain.com")]
    public void IsValid_WithInvalidEmail_ReturnsFalse(string? email)
    {
        var result = EmailValidator.IsValid(email);

        Assert.False(result);
    }
}
