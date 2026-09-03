namespace edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;

public record ConsultarDisponibilidadRequestDto(
    DateOnly Fecha
);

public record BloqueHorarioDto(
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    bool Disponible
);

public record DisponibilidadTutorResponseDto(
    int TutorId,
    string NombreCompleto,
    List<int> DiasAtencion,
    TimeOnly? HoraInicioAtencion,
    TimeOnly? HoraFinAtencion,
    DateOnly Fecha,
    bool AtiendeEseDia,
    List<BloqueHorarioDto> Bloques
);