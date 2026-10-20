using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace edu_connect_service.Api.Features.Auth.Login;

public record LoginRequestDto(
    [Required][EmailAddress] string Correo,
    [Required] string Password
);

public record TokenResponseDto(
    [property: JsonPropertyName("correo_validado")] bool CorreoValidado,
    string Token,
    string TokenType,
    int ExpiresIn,
    int IdUsuario,
    string Correo,
    string Rol,
    string? Nombre,
    string? Apellido,
    string? FotografiaUrl
);
