using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class GeneroValidatorTests
{
    [Theory]
    [InlineData("m", "masculino")]
    [InlineData("M", "masculino")]
    [InlineData("masculino", "masculino")]
    [InlineData("MASCULINO", "masculino")]
    [InlineData("  Masculino  ", "masculino")]
    [InlineData("f", "femenino")]
    [InlineData("F", "femenino")]
    [InlineData("femenino", "femenino")]
    [InlineData("FEMENINO", "femenino")]
    [InlineData("  Femenino  ", "femenino")]
    public void TryNormalize_WithValidInput_ReturnsTrueAndNormalizedValue(string input, string expected)
    {
        var result = GeneroValidator.TryNormalize(input, out var normalized);

        Assert.True(result);
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("otro")]
    [InlineData("x")]
    [InlineData("masc")]
    [InlineData("fem")]
    [InlineData("hombre")]
    [InlineData("mujer")]
    public void TryNormalize_WithInvalidInput_ReturnsFalseAndEmptyString(string? input)
    {
        var result = GeneroValidator.TryNormalize(input, out var normalized);

        Assert.False(result);
        Assert.Equal(string.Empty, normalized);
    }
}
