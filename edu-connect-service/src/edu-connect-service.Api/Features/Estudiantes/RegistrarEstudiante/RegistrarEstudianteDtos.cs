using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;

public class RegistrarEstudianteRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El carnet es obligatorio.")]
    [RegularExpression(@"^\d{6,10}$", ErrorMessage = "El carnet debe ser numérico y contener entre 6 y 10 dígitos.")]
    public string Carnet { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio.")]
    [RegularExpression(@"^(?i)(masculino|femenino|m|f)$", ErrorMessage = "El género debe ser 'masculino' ('m') o 'femenino' ('f').")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres.")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe ser numérico y contener exactamente 8 dígitos.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "La contraseña debe tener un mínimo de 8 caracteres, al menos 1 letra mayúscula, 1 minúscula y 1 número.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden exactamente.")]
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
