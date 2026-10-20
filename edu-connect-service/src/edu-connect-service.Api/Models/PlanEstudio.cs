namespace edu_connect_service.Api.Models;

public class PlanEstudio
{
    public int Id { get; set; }

    public int SesionId { get; set; }

    public Sesion Sesion { get; set; } = null!;

    public required string DificultadesIdentificadas { get; set; }

    public DateTime FechaCreacion { get; set; }

    public ICollection<RecursoPlanEstudio> Recursos { get; set; } = [];
}
