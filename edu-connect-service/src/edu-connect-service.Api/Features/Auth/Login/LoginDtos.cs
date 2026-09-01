using System.ComponentModel.DataAnnotations;

namespace edu_connect_service.Api.Features.Auth.Login;

public record LoginRequestDto(
    [Required][EmailAddress] string Correo,
    [Required] string Password
);

public record TokenResponseDto(
    string Token,
    string TokenType,
    int ExpiresIn,
    int IdUsuario,
    string Correo,
    string Rol
);
