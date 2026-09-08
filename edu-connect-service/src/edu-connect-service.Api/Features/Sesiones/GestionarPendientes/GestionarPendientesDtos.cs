namespace edu_connect_service.Api.Features.Sesiones.GestionarPendientes;

public record SesionPendienteDto(
    int Id,
    string Fecha,
    string Hora,
    string EstudianteNombre,
    string EstudianteId,
    string Materia,
    string Motivo,
    string Estado
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