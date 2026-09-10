namespace edu_connect_service.Api.Features.Sesiones.ObtenerSesionesActivas;

public record SesionActivaEstudianteDto(
    int Id,
    DateOnly FechaSesion,
    TimeOnly Hora,
    string NombreTutor,
    string Materia,
    string DireccionTutoria,
    string Motivo,
    string Estado,
    string? FotografiaTutorUrl
)
{
    public DateOnly Fecha => FechaSesion;
    public TimeOnly HoraInicio => Hora;
    public string? FotografiaUrl => FotografiaTutorUrl;
}
