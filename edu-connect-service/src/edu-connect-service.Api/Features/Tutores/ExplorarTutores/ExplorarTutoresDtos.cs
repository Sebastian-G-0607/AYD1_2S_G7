namespace edu_connect_service.Api.Features.Tutores.ExplorarTutores;

public record ExplorarTutoresRequestDto(
    string? Materia,
    string? Universidad,
    string? Genero,
    int? ExperienciaMinima,
    int? EdadMinima,
    int? EdadMaxima
);

public record TutorExploradoResponseDto(
    int TutorId,
    string NombreCompleto,
    List<string> Materias,
    string DireccionTutoria,
    string FotografiaUrl,
    string Universidad,
    string Genero,
    int AniosExperiencia,
    int Edad
);
