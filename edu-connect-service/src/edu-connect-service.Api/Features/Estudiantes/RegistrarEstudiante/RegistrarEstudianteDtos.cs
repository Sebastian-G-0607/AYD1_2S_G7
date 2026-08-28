using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace edu_connect_service.Api.Features.Estudiantes.RegistrarEstudiante;

public record RegistrarEstudianteRequestDto(
    [Required] string Nombre,
    [Required] string Apellido,
    [Required] string Carnet,
    [Required] string Genero,
    [Required] string Direccion,
    [Required] string Telefono,
    [Required] DateOnly FechaNacimiento,
    [Required][EmailAddress] string Correo,
    [Required] string Password,
    IFormFile? Fotografia
);

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
