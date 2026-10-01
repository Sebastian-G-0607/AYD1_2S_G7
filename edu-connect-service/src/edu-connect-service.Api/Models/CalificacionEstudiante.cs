namespace edu_connect_service.Api.Models;

public class CalificacionEstudiante
{
    public int Id { get; set; }

    public int SesionId { get; set; }

    public Sesion Sesion { get; set; } = null!;

    public int Estrellas { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaCreacion { get; set; }
}
