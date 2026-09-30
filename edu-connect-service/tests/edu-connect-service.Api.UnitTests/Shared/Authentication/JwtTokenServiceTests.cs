using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using edu_connect_service.Api.Shared.Authentication;
using Microsoft.Extensions.Options;

namespace edu_connect_service.Api.UnitTests.Shared.Authentication;

public class JwtTokenServiceTests
{
    private readonly JwtOptions _jwtOptions = new()
    {
        Key = "super_secret_jwt_key_that_is_at_least_32_characters_long!",
        Issuer = "EduConnectIssuer",
        Audience = "EduConnectAudience",
        ExpirationMinutes = 60
    };

    [Fact]
    public void GenerateToken_WithValidParameters_ReturnsValidJwt()
    {
        var optionsMock = Options.Create(_jwtOptions);
        var service = new JwtTokenService(optionsMock);

        var tokenString = service.GenerateToken(10, "test@educonnect.com", "Estudiante");

        Assert.NotNull(tokenString);
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(tokenString));

        var jwt = handler.ReadJwtToken(tokenString);
        Assert.Equal(_jwtOptions.Issuer, jwt.Issuer);
        Assert.Contains(_jwtOptions.Audience, jwt.Audiences);

        var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "id_usuario");
        Assert.NotNull(idClaim);
        Assert.Equal("10", idClaim.Value);

        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "correo");
        Assert.NotNull(emailClaim);
        Assert.Equal("test@educonnect.com", emailClaim.Value);

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "rol");
        Assert.NotNull(roleClaim);
        Assert.Equal("Estudiante", roleClaim.Value);

        var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        Assert.NotNull(subClaim);
        Assert.Equal("10", subClaim.Value);

        var jtiClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.NotNull(jtiClaim);
        Assert.NotEmpty(jtiClaim.Value);
    }
}
