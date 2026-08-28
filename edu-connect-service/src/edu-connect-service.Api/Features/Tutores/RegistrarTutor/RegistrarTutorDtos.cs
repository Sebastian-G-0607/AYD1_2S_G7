using System.ComponentModel.DataAnnotations;

namespace edu_connect_service.Api.Features.Tutores.RegistrarTutor;

public record RegistrarTutorRequestDto(
    [Required] string Nombre,
    [Required] string Apellido,
    [Required] string CarnetId,
    [Required] string NumeroIdentificacion,
    [Required] string Genero,
    [Required] string Direccion,
    [Required] string Telefono,
    [Required] DateOnly FechaNacimiento,
    [Required] string FotografiaUrl,
    [Required] string DireccionTutoria,
    [Required] int AnioInicio,
    [Required] string Universidad,
    [Required][EmailAddress] string Correo,
    [Required] string Password,
    [Required] List<int> MateriasIds,
    TimeOnly? HoraInicio,
    TimeOnly? HoraFin,
    List<int>? DiasAtencion
);

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
