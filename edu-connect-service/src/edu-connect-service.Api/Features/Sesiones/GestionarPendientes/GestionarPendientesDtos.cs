namespace edu_connect_service.Api.Features.Sesiones.GestionarPendientes;

public record SesionPendienteDto(
    int Id,
    string Fecha,
    string Hora,
    string EstudianteNombre,
    string EstudianteId,
    string Materia,
    string Motivo,
    string Estado,
    string? EstudianteAvatarUrl
);

public record AtenderSesionRequestDto(
    string? DificultadesIdentificadas,
    IReadOnlyList<RecursoPlanEstudioRequestDto>? Recursos,
    string? Resumen = null,
    string? Recomendaciones = null
);

public record RecursoPlanEstudioRequestDto(
    string? Nombre,
    string? Tipo,
    string? DescripcionUso
);

public record AtenderSesionResponseDto(
    int Id,
    string Estado,
    string Resumen
);