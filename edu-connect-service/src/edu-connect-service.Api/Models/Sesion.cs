namespace edu_connect_service.Api.Models;

public class Sesion
{
    public int Id { get; set; }

    public int EstudianteId { get; set; }

    public Estudiante Estudiante { get; set; } = null!;

    public int TutorId { get; set; }

    public Tutor Tutor { get; set; } = null!;

    public int MateriaId { get; set; }

    public Materia Materia { get; set; } = null!;

    public int EstadoId { get; set; }

    public EstadoSesion Estado { get; set; } = null!;

    public DateOnly FechaSesion { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public required string Motivo { get; set; }

    public string? Resumen { get; set; }

    public DateTime FechaCreacion { get; set; }
}
