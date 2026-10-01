namespace edu_connect_service.Api.Models;

public class ReporteEstudiante
{
    public int Id { get; set; }

    public int SesionId { get; set; }

    public Sesion Sesion { get; set; } = null!;

    public int CategoriaId { get; set; }

    public CategoriaReporteEstudiante Categoria { get; set; } = null!;

    public required string Motivo { get; set; }

    public DateTime FechaReporte { get; set; }

    public required string Estado { get; set; }
}
