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

    [Fact]
    public void GenerateEmailValidationToken_ReturnsValidJwtWithCorrectClaims()
    {
        var optionsMock = Options.Create(_jwtOptions);
        var service = new JwtTokenService(optionsMock);

        var tokenString = service.GenerateEmailValidationToken(261, "estudiante.calificacion@gmail.com", 2);

        Assert.NotNull(tokenString);
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(tokenString));

        var jwt = handler.ReadJwtToken(tokenString);
        Assert.Equal(_jwtOptions.Issuer, jwt.Issuer);
        Assert.Contains(_jwtOptions.Audience, jwt.Audiences);

        var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "id_usuario");
        Assert.NotNull(idClaim);
        Assert.Equal("261", idClaim.Value);

        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "correo");
        Assert.NotNull(emailClaim);
        Assert.Equal("estudiante.calificacion@gmail.com", emailClaim.Value);

        var scopeClaim = jwt.Claims.FirstOrDefault(c => c.Type == "scope");
        Assert.NotNull(scopeClaim);
        Assert.Equal("email_validation", scopeClaim.Value);

        var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        Assert.NotNull(subClaim);
        Assert.Equal("261", subClaim.Value);

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "rol");
        Assert.Null(roleClaim);

        Assert.True(jwt.ValidTo > DateTime.UtcNow);
        Assert.True(jwt.ValidTo <= DateTime.UtcNow.AddMinutes(2).AddSeconds(5));
    }
}
