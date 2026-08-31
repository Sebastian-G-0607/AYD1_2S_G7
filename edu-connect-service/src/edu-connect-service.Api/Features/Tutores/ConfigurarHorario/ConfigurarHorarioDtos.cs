namespace edu_connect_service.Api.Features.Tutores.ConfigurarHorario;

public record ConfigurarHorarioRequestDto(
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    List<int> DiasAtencion
);

public record ConfigurarHorarioResponseDto(
    int TutorId,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    List<int> DiasAtencion
);