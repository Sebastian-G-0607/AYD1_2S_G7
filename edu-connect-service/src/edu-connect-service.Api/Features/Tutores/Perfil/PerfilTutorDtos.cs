using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Tutores.Perfil;

public class ActualizarPerfilTutorRequestDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string CarnetId { get; set; } = string.Empty;

    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public DateOnly FechaNacimiento { get; set; }

    public IFormFile? Fotografia { get; set; }

    public string DireccionTutoria { get; set; } = string.Empty;

    public int AnioInicio { get; set; }

    public string Universidad { get; set; } = string.Empty;
}

public record CambiarPasswordTutorRequestDto(
    string PasswordActual,
    string NuevaPassword,
    string ConfirmarNuevaPassword
);

public record PerfilTutorResponseDto(
    int UsuarioId,
    string Nombre,
    string Apellido,
    string CarnetId,
    string NumeroIdentificacion,
    string Genero,
    string Direccion,
    string Telefono,
    DateOnly FechaNacimiento,
    string FotografiaUrl,
    string DireccionTutoria,
    int AnioInicio,
    string Universidad,
    string Correo
);