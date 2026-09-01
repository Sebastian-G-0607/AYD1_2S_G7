using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace edu_connect_service.Api.Shared.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(int idUsuario, string correo, string rol);
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
            new(ClaimTypes.Role, rol),
            new(ClaimTypes.NameIdentifier, idUsuario.ToString()),
            new(ClaimTypes.Email, correo),
            new(JwtRegisteredClaimNames.Sub, idUsuario.ToString()),
            new(JwtRegisteredClaimNames.Email, correo),
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
}
