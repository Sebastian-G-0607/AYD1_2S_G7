using Microsoft.AspNetCore.Mvc;

namespace edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;

public record ConsultarDisponibilidadRequestDto(
    [property: FromQuery(Name = "fecha")] string? Fecha = null
);

public record BloqueHorarioDto(
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    bool Disponible
);

public record SesionOcupadaDto(
    TimeOnly HoraInicio,
    TimeOnly HoraFin
);

public record DisponibilidadTutorResponseDto(
    int TutorId,
    string NombreCompleto,
    List<int> DiasAtencion,
    TimeOnly? HoraInicioAtencion,
    TimeOnly? HoraFinAtencion,
    DateOnly Fecha,
    bool AtiendeEseDia,
    List<BloqueHorarioDto> Bloques,
    List<SesionOcupadaDto> SesionesOcupadas
);