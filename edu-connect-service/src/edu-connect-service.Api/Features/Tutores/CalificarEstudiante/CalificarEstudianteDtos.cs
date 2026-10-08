namespace edu_connect_service.Api.Features.Tutores.CalificarEstudiante;

public sealed record CalificarEstudianteRequest(
    int Estrellas,
    string? Comentario
);

public sealed record CalificarEstudianteResponse(
    int SesionId,
    int Estrellas,
    string? Comentario
);