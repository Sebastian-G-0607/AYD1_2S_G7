using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Tutores.RegistrarTutor;

public class RegistrarTutorRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [RegularExpression(@"^\d{6,10}$", ErrorMessage = "El carnet debe ser numérico y contener entre 6 y 10 dígitos.")]
    public string CarnetId { get; set; } = string.Empty;

    [RegularExpression(@"^\d{6,10}$", ErrorMessage = "El carnet debe ser numérico y contener entre 6 y 10 dígitos.")]
    public string? Carnet { get; set; }

    [RegularExpression(@"^\d{13}$", ErrorMessage = "El DPI / documento de identificación debe ser numérico y contener exactamente 13 dígitos.")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [RegularExpression(@"^\d{13}$", ErrorMessage = "El DPI / documento de identificación debe ser numérico y contener exactamente 13 dígitos.")]
    public string? Dpi { get; set; }

    [Required(ErrorMessage = "El género es obligatorio.")]
    [RegularExpression(@"^(?i)(masculino|femenino|m|f)$", ErrorMessage = "El género debe ser 'masculino' ('m') o 'femenino' ('f').")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de residencia es obligatoria.")]
    [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres.")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe ser numérico y contener exactamente 8 dígitos.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateOnly FechaNacimiento { get; set; }

    [Required(ErrorMessage = "La fotografía de perfil es obligatoria.")]
    public IFormFile? Fotografia { get; set; }

    [Required(ErrorMessage = "La dirección de tutoría es obligatoria.")]
    [StringLength(255, ErrorMessage = "La dirección de tutoría no puede exceder 255 caracteres.")]
    public string DireccionTutoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año de inicio es obligatorio.")]
    [Range(1980, 2100, ErrorMessage = "El año de inicio debe ser un año numérico válido.")]
    public int AnioInicio { get; set; }

    [Required(ErrorMessage = "La universidad es obligatoria.")]
    [StringLength(150, ErrorMessage = "La universidad no puede exceder 150 caracteres.")]
    public string Universidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "La contraseña debe tener un mínimo de 8 caracteres, al menos 1 letra mayúscula, 1 minúscula y 1 número.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es obligatoria.")]
    [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden exactamente.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public List<int> MateriasIds { get; set; } = [];

    public List<int>? Materias { get; set; }

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
