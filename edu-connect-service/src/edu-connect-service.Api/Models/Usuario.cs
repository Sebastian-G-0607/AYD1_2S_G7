namespace edu_connect_service.Api.Models;

public class Usuario
{
    public int Id { get; set; }

    public required string Correo { get; set; }

    public required string PasswordHash { get; set; }

    public int RolId { get; set; }

    public Rol Rol { get; set; } = null!;

    public int EstadoId { get; set; }

    public EstadoUsuario Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaBaja { get; set; }

    public string? MotivoBaja { get; set; }

    public Administrador? Administrador { get; set; }

    public Estudiante? Estudiante { get; set; }

    public Tutor? Tutor { get; set; }
}
