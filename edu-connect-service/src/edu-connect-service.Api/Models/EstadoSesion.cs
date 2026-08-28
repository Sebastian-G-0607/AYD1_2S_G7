namespace edu_connect_service.Api.Models;

public class EstadoSesion
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public string? Descripcion { get; set; }

    public ICollection<Sesion> Sesiones { get; set; } = [];
}
