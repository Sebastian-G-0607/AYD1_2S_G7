namespace edu_connect_service.Api.Features.Estudiantes.HistorialSesiones;

public record HistorialSesionEstudianteResponseDto(
    int SesionId,
    DateOnly FechaSesion,
    string Tutor,
    string Materia,
    string DireccionTutoria,
    string Motivo,
    string? Resumen,
    string Estado
);