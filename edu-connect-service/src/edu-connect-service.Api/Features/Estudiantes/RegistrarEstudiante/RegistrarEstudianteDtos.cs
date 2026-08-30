using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;

public class RegistrarEstudianteRequestDto
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    public string Carnet { get; set; } = string.Empty;

    [Required]
    public string Genero { get; set; } = string.Empty;

    [Required]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    public DateOnly FechaNacimiento { get; set; }

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IFormFile? Fotografia { get; set; }
}

public record EstudianteResponseDto(
    int UsuarioId,
    string Nombre,
    string Apellido,
    string Carnet,
    string Genero,
    string Direccion,
    string Telefono,
    DateOnly FechaNacimiento,
    string? FotografiaUrl,
    string Correo,
    string Rol,
    string Estado,
    DateTime FechaRegistro
);
