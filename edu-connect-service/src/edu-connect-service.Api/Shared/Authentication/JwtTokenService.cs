using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace edu_connect_service.Api.Shared.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(int idUsuario, string correo, string rol);
    string GenerateEmailValidationToken(int idUsuario, string correo, int minutes);
}

public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(int idUsuario, string correo, string rol)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        var claims = new List<Claim>
        {
            new("id_usuario", idUsuario.ToString()),
            new("correo", correo),
            new("rol", rol),
            new(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            IssuedAt = DateTime.UtcNow,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateEmailValidationToken(int idUsuario, string correo, int minutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var now = DateTime.UtcNow;

        var issuer = !string.IsNullOrWhiteSpace(_jwtOptions.Issuer) ? _jwtOptions.Issuer : "edu-connect-service";
        var audience = !string.IsNullOrWhiteSpace(_jwtOptions.Audience) ? _jwtOptions.Audience : "edu-connect-client";

        var claims = new List<Claim>
        {
            new("id_usuario", idUsuario.ToString()),
            new("correo", correo),
            new(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
            new("scope", "email_validation"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddMinutes(minutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
