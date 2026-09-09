namespace edu_connect_service.Api.Features.Tutores.HistorialSesiones;

public record HistorialSesionResponseDto(
    int SesionId,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string Estudiante,
    string Estado,
    string? EstudianteAvatarUrl
);