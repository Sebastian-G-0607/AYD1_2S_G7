using edu_connect_service.Api.Shared.Validation;

namespace edu_connect_service.Api.UnitTests.Shared.Validation;

public class DpiValidatorTests
{
    [Theory]
    [InlineData("1234567890123")]
    [InlineData("2998123450101")]
    [InlineData("0000000000000")]
    [InlineData(" 1234567890123 ")]
    public void IsValid_WithValid13Digits_ReturnsTrue(string dpi)
    {
        var result = DpiValidator.IsValid(dpi);

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123456789012")]
    [InlineData("12345678901234")]
    [InlineData("123456789012a")]
    [InlineData("1234 56789 0101")]
    [InlineData("1234-56789-0101")]
    public void IsValid_WithInvalidDpi_ReturnsFalse(string? dpi)
    {
        var result = DpiValidator.IsValid(dpi);

        Assert.False(result);
    }
}
