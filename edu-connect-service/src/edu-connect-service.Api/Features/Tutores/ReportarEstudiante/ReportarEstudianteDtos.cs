namespace edu_connect_service.Api.Features.Tutores.ReportarEstudiante;

public sealed record ReportarEstudianteRequest(
    int CategoriaId,
    string Explicacion
);

public sealed record ReportarEstudianteResponse(
    int ReporteId,
    int SesionId,
    string Categoria,
    string Estado
);

public sealed record CategoriaReporteEstudianteResponse(
    int Id,
    string Nombre
);