using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class TelefonoValidatorTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("55554444")]
    [InlineData("00000000")]
    [InlineData(" 12345678 ")]
    public void IsValid_WithValid8Digits_ReturnsTrue(string telefono)
    {
        var result = TelefonoValidator.IsValid(telefono);

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234abcd")]
    [InlineData("1234-5678")]
    [InlineData("+50212345678")]
    public void IsValid_WithInvalidTelefono_ReturnsFalse(string? telefono)
    {
        var result = TelefonoValidator.IsValid(telefono);

        Assert.False(result);
    }
}
