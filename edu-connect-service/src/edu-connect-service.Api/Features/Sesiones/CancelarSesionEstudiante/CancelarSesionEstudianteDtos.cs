namespace edu_connect_service.Api.Features.Sesiones.CancelarSesionEstudiante;

public record CancelarSesionEstudianteResponseDto(
    int Id,
    string Estado,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string Materia,
    string TutorNombre,
    string Mensaje
);
