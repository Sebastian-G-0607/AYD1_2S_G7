using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class CarnetValidatorTests
{
    // casos válidos
    [Theory]
    [InlineData("123456")]
    [InlineData("202012345")]
    [InlineData("1234567890")]
    public void IsValid_WithValidDigits_ReturnsTrue(string carnet)
    {
        // Act
        var result = CarnetValidator.IsValid(carnet);

        // Assert
        Assert.True(result);
    }

    // casos inválidos
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")] // Menos de 6 dígitos
    [InlineData("12345678901")] // Más de 10 dígitos
    [InlineData("2020A1234")] // Contiene letras
    [InlineData("123-456")] // Contiene guiones
    public void IsValid_WithInvalidCarnet_ReturnsFalse(string? carnet)
    {
        // Act
        var result = CarnetValidator.IsValid(carnet);

        // Assert
        Assert.False(result);
    }
}
