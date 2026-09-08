namespace edu_connect_service.Api.Features.Sesiones.GestionarPendientes;

public record SesionPendienteDto(
    int Id,
    DateOnly FechaSesion,
    TimeOnly HoraInicio,
    string EstudianteNombre,
    string EstudianteCarnet,
    string Materia,
    string Motivo
);

public record AtenderSesionRequestDto(
    string Resumen,
    string? Recomendaciones
);

public record AtenderSesionResponseDto(
    int Id,
    string Estado,
    string Resumen
);