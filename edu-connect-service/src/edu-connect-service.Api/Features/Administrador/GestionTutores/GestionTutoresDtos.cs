namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public record TutorPendienteResponseDto(
    int Id,
    string Nombre,
    string Apellido,
    string CarnetId,
    string NumeroIdentificacion,
    string Genero,
    DateOnly FechaNacimiento,
    string Correo,
    string FotografiaUrl,
    string Especialidad,
    List<string> Materias,
    string DireccionTutoria,
    int AnioInicio,
    string Universidad,
    string? Direccion = null,
    string? Telefono = null,
    DateTime? FechaRegistro = null
);

public record ActualizarEstadoTutorRequestDto(
    string Estado,
    string? Motivo = null
);

public record ActualizarEstadoTutorResponseDto(
    int Id,
    string Correo,
    string Estado,
    string Mensaje
);

public record TutorActivoResponseDto(
    int Id,
    string Nombre,
    string Apellido,
    string CarnetId,
    string NumeroIdentificacion,
    string Genero,
    DateOnly FechaNacimiento,
    string Correo,
    string FotografiaUrl,
    string Especialidad,
    List<string> Materias,
    string DireccionTutoria,
    int AnioInicio,
    string Universidad,
    string? Direccion = null,
    string? Telefono = null,
    DateTime? FechaRegistro = null,
    string Estado = "APROBADO"
);

public record DarBajaTutorRequestDto(
    string? Motivo = null
);

public record DarBajaTutorResponseDto(
    int Id,
    string Correo,
    string Estado,
    DateTime? FechaBaja,
    string? Motivo,
    string Mensaje
);

