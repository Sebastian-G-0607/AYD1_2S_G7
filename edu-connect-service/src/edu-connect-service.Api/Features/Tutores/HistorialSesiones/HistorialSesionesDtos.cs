namespace edu_connect_service.Api.Features.Tutores.HistorialSesiones;

public record HistorialSesionesRequestDto(
    DateOnly? Fecha = null,
    string? Estudiante = null,
    string? Correo = null
);

public record HistorialSesionResponseDto(
    int SesionId,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string Estudiante,
    string EstudianteEmail,
    string Estado,
    string? EstudianteAvatarUrl
);