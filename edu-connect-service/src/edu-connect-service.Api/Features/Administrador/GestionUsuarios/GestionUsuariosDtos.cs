namespace edu_connect_service.Api.Features.Administrador.GestionUsuarios;

public record UsuarioDadoDeBajaResponseDto(
    int Id,
    string Correo,
    string Rol,
    string Estado,
    string TipoUsuario,
    string? NombreCompleto,
    string? Identificador,
    DateTime? FechaRegistro,
    DateTime? FechaBaja,
    string? MotivoBaja
);
