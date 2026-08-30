namespace edu_connect_service.Api.Features.Auth.Me;

public record UserProfileDto(
    string? IdUsuario,
    string? Correo,
    string? Rol
);
