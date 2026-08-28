namespace edu_connect_service.Api.Models;

public class EstadoUsuario
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = [];
}
