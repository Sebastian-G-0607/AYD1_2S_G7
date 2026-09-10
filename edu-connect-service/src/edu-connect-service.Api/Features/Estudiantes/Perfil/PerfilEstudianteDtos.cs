using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Estudiantes.Perfil;

public class ActualizarPerfilEstudianteRequestDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Carnet { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public DateOnly FechaNacimiento { get; set; }

    public IFormFile? Fotografia { get; set; }
}

public record CambiarPasswordEstudianteRequestDto(
    string PasswordActual,
    string NuevaPassword,
    string ConfirmarNuevaPassword
);

public record PerfilEstudianteResponseDto(
    int UsuarioId,
    string Nombre,
    string Apellido,
    string Carnet,
    string Genero,
    string Direccion,
    string Telefono,
    DateOnly FechaNacimiento,
    string? FotografiaUrl,
    string Correo
);