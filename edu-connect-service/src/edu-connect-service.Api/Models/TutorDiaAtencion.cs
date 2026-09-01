namespace edu_connect_service.Api.Models;

public class TutorDiaAtencion
{
    public int Id { get; set; }

    public int TutorId { get; set; }

    public Tutor Tutor { get; set; } = null!;

    public int DiaSemana { get; set; }
}
