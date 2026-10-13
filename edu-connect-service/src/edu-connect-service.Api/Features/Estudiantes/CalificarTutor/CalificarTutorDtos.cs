namespace edu_connect_service.Api.Features.Estudiantes.CalificarTutor;

public sealed record CalificarTutorRequestDto(
    int? Estrellas,
    string? Comentario
);

public sealed record CalificarTutorResponseDto(
    int SesionId,
    int Estrellas,
    string? Comentario
);