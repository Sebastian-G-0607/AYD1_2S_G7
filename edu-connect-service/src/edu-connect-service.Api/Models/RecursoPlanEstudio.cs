namespace edu_connect_service.Api.Models;

public class RecursoPlanEstudio
{
    public int Id { get; set; }

    public int PlanEstudioId { get; set; }

    public PlanEstudio PlanEstudio { get; set; } = null!;

    public required string Nombre { get; set; }

    public required string Tipo { get; set; }

    public string? DescripcionUso { get; set; }
}
