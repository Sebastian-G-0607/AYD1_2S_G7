namespace edu_connect_service.Api.Models;

public class TutorMateria
{
    public int TutorId { get; set; }

    public Tutor Tutor { get; set; } = null!;

    public int MateriaId { get; set; }

    public Materia Materia { get; set; } = null!;
}
