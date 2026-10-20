using System.Security.Cryptography;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Shared.Emails;

public class TokenCorreoService(edu_connect_serviceContext dbContext) : ITokenCorreoService
{
    public string GenerarToken()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }

    public async Task<string> GenerarYRegistrarTokenAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var token = GenerarToken();
        var ahora = DateTime.UtcNow;
        var fechaExpiracion = ahora.AddHours(24);

        var tokenExistente = await dbContext.TokensCorreo
            .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId, cancellationToken);

        if (tokenExistente is not null)
        {
            tokenExistente.Token = token;
            tokenExistente.FechaGeneracion = ahora;
            tokenExistente.FechaExpiracion = fechaExpiracion;
            tokenExistente.Revocado = false;
        }
        else
        {
            var nuevoToken = new TokenCorreo
            {
                UsuarioId = usuarioId,
                Token = token,
                FechaGeneracion = ahora,
                FechaExpiracion = fechaExpiracion,
                Revocado = false
            };
            dbContext.TokensCorreo.Add(nuevoToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return token;
    }
}
