namespace edu_connect_service.Api.Models;

public class CategoriaReporteTutor
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public string? Descripcion { get; set; }

    public ICollection<ReporteTutor> Reportes { get; set; } = [];
}
