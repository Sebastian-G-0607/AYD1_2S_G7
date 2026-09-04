namespace edu_connect_service.Api.Features.Sesiones.ProgramarSesion;

public record ProgramarSesionRequestDto(
    int TutorId,
    int MateriaId,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string Motivo
);

public record ProgramarSesionResponseDto(
    int Id,
    int EstudianteId,
    int TutorId,
    int MateriaId,
    string Materia,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    TimeOnly? HoraFin,
    string Motivo,
    string Estado
);