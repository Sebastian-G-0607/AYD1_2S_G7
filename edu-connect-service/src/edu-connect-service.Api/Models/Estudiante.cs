namespace edu_connect_service.Api.Models;

public class Estudiante
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public required string Nombre { get; set; }

    public required string Apellido { get; set; }

    public required string Carnet { get; set; }

    public required string Genero { get; set; }

    public required string Direccion { get; set; }

    public required string Telefono { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public string? FotografiaUrl { get; set; }

    public ICollection<Sesion> Sesiones { get; set; } = [];
}
