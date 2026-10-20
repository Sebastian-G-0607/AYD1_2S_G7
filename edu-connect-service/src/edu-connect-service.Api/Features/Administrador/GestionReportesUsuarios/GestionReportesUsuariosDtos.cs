namespace edu_connect_service.Api.Features.Administrador.GestionReportesUsuarios;

public record ReporteTutorAdminResponseDto(
    int Id,
    int SesionId,
    string Categoria,
    string Motivo,
    int TutorId,
    string TutorNombreCompleto,
    string TutorCorreo,
    int EstudianteDenuncianteId,
    string EstudianteDenuncianteNombreCompleto,
    string EstudianteDenuncianteCorreo,
    DateOnly FechaSesion,
    DateTime FechaReporte,
    string Estado
);

public record ReporteEstudianteAdminResponseDto(
    int Id,
    int SesionId,
    string Categoria,
    string Motivo,
    int EstudianteId,
    string EstudianteNombreCompleto,
    string EstudianteCorreo,
    int TutorDenuncianteId,
    string TutorDenuncianteNombreCompleto,
    string TutorDenuncianteCorreo,
    DateOnly FechaSesion,
    DateTime FechaReporte,
    string Estado
);

public record ResolverReporteResponseDto(
    int ReporteId,
    string EstadoReporte,
    int UsuarioId,
    string EstadoUsuario,
    string Mensaje
);