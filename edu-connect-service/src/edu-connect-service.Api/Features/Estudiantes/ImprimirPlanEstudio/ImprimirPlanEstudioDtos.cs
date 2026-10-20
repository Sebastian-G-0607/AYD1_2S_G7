namespace edu_connect_service.Api.Features.Estudiantes.ImprimirPlanEstudio;

/// <summary>
/// ⚠️ TEMPORAL: mientras HU-29 no persiste el plan de estudio en base de datos,
/// el cliente envía el plan ya cargado (mismo shape que StudyPlan en el frontend).
/// Cuando exista la tabla real, este endpoint pasará a ser GET y a leer
/// el plan desde la base de datos según el usuario autenticado,
/// eliminando este request body.
/// </summary>
public sealed record ImprimirPlanEstudioRequest(
    string FechaUltimaSesion,
    string TutorNombre,
    string TutorIdentificacion,
    string TutorEspecialidad,
    string DificultadesIdentificadas,
    List<RecursoRecomendadoRequest> Recursos
);

public sealed record RecursoRecomendadoRequest(
    string Nombre,
    string Tipo,
    string DescripcionUso
);