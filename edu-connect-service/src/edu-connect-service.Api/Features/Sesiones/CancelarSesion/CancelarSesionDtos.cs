namespace edu_connect_service.Api.Features.Sesiones.CancelarSesion;

public record CancelarSesionRequestDto(
    string Motivo,
    string? MensajeDisculpa = null
);

public record CancelarSesionResponseDto(
    int Id,
    string Estado,
    string MotivoCancelacion,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string Materia,
    string EstudianteNombre,
    string Mensaje
);
