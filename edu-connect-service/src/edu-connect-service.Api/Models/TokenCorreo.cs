namespace edu_connect_service.Api.Models;

public class TokenCorreo
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public required string Token { get; set; }

    public DateTime FechaGeneracion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public bool Revocado { get; set; }
}
