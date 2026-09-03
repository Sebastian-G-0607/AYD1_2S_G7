using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace edu_connect_service.Api.Features.Auth.AdminTwoFactor;

public static class AdminTwoFactorEndpoints
{
    public static void MapAdminTwoFactor(this IEndpointRouteBuilder app)
    {

        var authGroup = app.MapGroup("/auth");
        authGroup.MapPost("/admin-login", AdminInitialLogin);
        authGroup.MapPost("/admin-2fa", AdminUploadFile);

        var apiGroup = app.MapGroup("/api");
        apiGroup.MapPost("/admin-login", AdminInitialLogin);
        apiGroup.MapPost("/admin-2fa", AdminUploadFile);
    }

    private static IResult AdminInitialLogin(
        [FromBody] LoginRequestDto request,
        IConfiguration config,
        IOptions<JwtOptions> jwtOptions)
    {
        var adminSection = config.GetSection("AdminUser");
        var adminEmail = adminSection.GetValue<string>("Email");
        var adminPassword = adminSection.GetValue<string>("Password");
        var passwordFase2 = adminSection.GetValue<string>("PasswordFase2");

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(passwordFase2))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "AdminUser no está configurado en appsettings.json"
            );
        }

        if (request.Correo != adminEmail || request.Password != adminPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Credenciales inválidas",
                detail: "El correo o la contraseña son incorrectos."
            );
        }

        if (adminPassword == passwordFase2)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración inválida",
                detail: "La contraseña inicial y la de archivo deben ser diferentes."
            );
        }

        var tempToken = GenerateTemporaryToken(0, adminEmail, jwtOptions.Value, TimeSpan.FromMinutes(5));

        return Results.Ok(new { TempToken = tempToken });
    }

    private static async Task<IResult> AdminUploadFile(
        ClaimsPrincipal user,
        HttpRequest request,
        IConfiguration config,
        IOptions<JwtOptions> jwtOptions,
        IJwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        var rolClaim = user.FindFirst("rol")?.Value ?? user.FindFirst(ClaimTypes.Role)?.Value;
        if (rolClaim is null || rolClaim != "AdminPending2FA")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "No autorizado",
                detail: "Se requiere un token temporal válido para completar la segunda fase."
            );
        }

        if (!request.HasFormContentType)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Solicitud inválida",
                detail: "Se requiere multipart/form-data con el archivo 'auth2-ayd1.txt'."
            );
        }

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.FirstOrDefault();

        if (file is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Archivo faltante",
                detail: "No se recibió ningún archivo. Asegúrese de subir 'auth2-ayd1.txt'."
            );
        }

        string fileText;
        await using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, cancellationToken);
            fileText = Encoding.UTF8.GetString(ms.ToArray()).Trim();
        }

        // El archivo debe contener base64 de [iv(16 bytes)|ciphertext]
        byte[] cipherWithIv;
        try
        {
            cipherWithIv = Convert.FromBase64String(fileText);
        }
        catch
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Formato inválido",
                detail: "El archivo debe contener Base64 del contenido cifrado (iv + ciphertext)."
            );
        }

        if (cipherWithIv.Length < 17)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Archivo inválido",
                detail: "Contenido demasiado corto para contener IV y ciphertext."
            );
        }

        var key = DeriveKeyFromString(jwtOptions.Value.Key);
        var iv = cipherWithIv[..16];
        var cipher = cipherWithIv[16..];

        string decrypted;
        try
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            decrypted = Encoding.UTF8.GetString(plain);
        }
        catch
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Desencriptado fallido",
                detail: "No se pudo desencriptar el contenido del archivo con la clave del servidor."
            );
        }

        var adminSection = config.GetSection("AdminUser");
        var adminEmail = adminSection.GetValue<string>("Email");
        var adminPassword = adminSection.GetValue<string>("Password");
        var passwordFase2 = adminSection.GetValue<string>("PasswordFase2");

        if (decrypted != passwordFase2)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Código 2FA inválido",
                detail: "La contraseña proporcionada en el archivo no es válida."
            );
        }

        if (passwordFase2 == adminPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración inválida",
                detail: "La contraseña inicial y la de archivo deben ser diferentes."
            );
        }

        var finalToken = jwtTokenService.GenerateToken(0, adminEmail, AppRoles.Administrador);

        return Results.Ok(new { Token = finalToken, Role = AppRoles.Administrador });
    }

    private static byte[] DeriveKeyFromString(string keyString)
    {
        return SHA256.HashData(Encoding.UTF8.GetBytes(keyString ?? string.Empty));
    }

    private static string GenerateTemporaryToken(int idUsuario, string correo, JwtOptions jwtOptions, TimeSpan validity)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

        var claims = new List<System.Security.Claims.Claim>
        {
            new("id_usuario", idUsuario.ToString()),
            new("correo", correo),
            new("rol", "AdminPending2FA"),
            new(ClaimTypes.Role, "AdminPending2FA"),
            new(ClaimTypes.NameIdentifier, idUsuario.ToString()),
            new(ClaimTypes.Email, correo),
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(validity),
            IssuedAt = DateTime.UtcNow,
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
