using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Tutores.RegistrarTutor;

public class RegistrarTutorRequestDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    public string CarnetId { get; set; } = string.Empty;

    [Required]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required]
    public string Genero { get; set; } = string.Empty;

    [Required]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    public DateOnly FechaNacimiento { get; set; }

    [Required]
    public IFormFile Fotografia { get; set; } = null!;

    [Required]
    public string DireccionTutoria { get; set; } = string.Empty;

    [Required]
    public int AnioInicio { get; set; }

    [Required]
    public string Universidad { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public List<int> MateriasIds { get; set; } = [];

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public List<int>? DiasAtencion { get; set; }
}

public record TutorResponseDto(
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
    TimeOnly? HoraInicio,
    TimeOnly? HoraFin,
    string Correo,
    string Rol,
    string Estado,
    DateTime FechaRegistro,
    List<int> DiasAtencion,
    List<int> MateriasIds
);
