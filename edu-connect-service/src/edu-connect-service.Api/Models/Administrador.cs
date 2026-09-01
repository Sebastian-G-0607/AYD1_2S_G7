namespace edu_connect_service.Api.Models;

public class Administrador
{
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public required string PasswordFase2Hash { get; set; }
}
