namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public record EstudiantePendienteResponseDto(
    int Id,
    string Nombre,
    string Apellido,
    string Carnet,
    string Genero,
    DateOnly FechaNacimiento,
    string Correo,
    string? FotografiaUrl,
    string? Direccion = null,
    string? Telefono = null,
    DateTime? FechaRegistro = null
);

public record ActualizarEstadoEstudianteRequestDto(
    string Estado,
    string? Motivo = null
);

public record ActualizarEstadoEstudianteResponseDto(
    int Id,
    string Correo,
    string Estado,
    string Mensaje
);

