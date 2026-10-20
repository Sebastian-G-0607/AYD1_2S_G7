namespace edu_connect_service.Api.Features.Administrador.Reportes;

public record CalificacionTutorReporteDto(
    int TutorId,
    string NombreCompleto,
    string Especialidad,
    double? PromedioCalificacion,
    int TotalCalificaciones
);
