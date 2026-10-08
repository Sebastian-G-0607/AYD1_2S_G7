namespace edu_connect_service.Api.Features.Estudiantes.ReportarTutor;

public record CategoriaReporteTutorResponseDto(
    int Id,
    string Nombre,
    string? Descripcion
);

public record ReportarTutorRequestDto(
    int CategoriaId,
    string? Motivo
);

public record ReportarTutorResponseDto(
    int Id,
    int SesionId,
    string Categoria,
    string Estado,
    string Mensaje
);
