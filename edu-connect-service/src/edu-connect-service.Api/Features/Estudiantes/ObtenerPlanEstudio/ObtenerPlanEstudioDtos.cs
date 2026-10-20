namespace edu_connect_service.Api.Features.Estudiantes.ObtenerPlanEstudio;

public sealed record PlanEstudioResponse(
    string FechaUltimaSesion,
    string TutorNombre,
    string TutorIdentificacion,
    string TutorEspecialidad,
    string DificultadesIdentificadas,
    List<RecursoRecomendadoResponse> Recursos
);

public sealed record RecursoRecomendadoResponse(
    string Nombre,
    string Tipo,
    string DescripcionUso
);