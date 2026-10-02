namespace edu_connect_service.Api.Shared.Emails;

public interface ITokenCorreoService
{
    string GenerarToken();

    Task<string> GenerarYRegistrarTokenAsync(int usuarioId, CancellationToken cancellationToken = default);
}
