namespace edu_connect_service.Api.Models;

public class Materia
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public ICollection<TutorMateria> TutorMaterias { get; set; } = [];

    public ICollection<Sesion> Sesiones { get; set; } = [];
}
