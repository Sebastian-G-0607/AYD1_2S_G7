using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Features.Auth.ValidarEmail;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Emails;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Auth.ValidarEmail;

public class ValidarEmailEndpointTests
{
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IS3Service> _s3ServiceMock = new();
    private readonly Mock<ITokenCorreoService> _tokenCorreoServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly IOptions<JwtOptions> _jwtOptions = Options.Create(new JwtOptions
    {
        Key = "super_secret_jwt_key_that_is_at_least_32_characters_long!",
        Issuer = "EduConnectIssuer",
        Audience = "EduConnectAudience",
        ExpirationMinutes = 120
    });

    private static edu_connect_serviceContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: $"ValidarEmailTestDb_{Guid.NewGuid()}")
            .Options;
        return new edu_connect_serviceContext(options);
    }

    private static ClaimsPrincipal CreateUserPrincipal(int usuarioId)
    {
        var claims = new List<Claim>
        {
            new("id_usuario", usuarioId.ToString()),
            new("correo", "estudiante@educonnect.com"),
            new("scope", "email_validation"),
            new(ClaimTypes.NameIdentifier, usuarioId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("abcdef")]
    public async Task ValidarAsync_WithInvalidCodeFormat_ReturnsBadRequestProblem(string codigoInvalido)
    {
        using var db = CreateInMemoryDbContext();
        var principal = CreateUserPrincipal(10);
        var request = new ValidarEmailRequestDto(codigoInvalido, null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Código inválido", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public async Task ValidarAsync_WithMissingUserClaim_ReturnsUnauthorizedProblem()
    {
        using var db = CreateInMemoryDbContext();
        var emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var request = new ValidarEmailRequestDto("123456", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            emptyPrincipal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, problemResult.StatusCode);
    }

    [Fact]
    public async Task ValidarAsync_WithNonExistentUser_ReturnsNotFoundProblem()
    {
        using var db = CreateInMemoryDbContext();
        var principal = CreateUserPrincipal(999);
        var request = new ValidarEmailRequestDto("123456", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
    }

    [Fact]
    public async Task ValidarAsync_WithNoTokenInDatabase_ReturnsBadRequestProblem()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var user = new Usuario
        {
            Id = 15,
            Correo = "estudiante@educonnect.com",
            PasswordHash = "hash",
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        var principal = CreateUserPrincipal(15);
        var request = new ValidarEmailRequestDto("123456", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Código no encontrado", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public async Task ValidarAsync_WithRevokedToken_ReturnsBadRequestProblem()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var user = new Usuario
        {
            Id = 20,
            Correo = "revocado@educonnect.com",
            PasswordHash = "hash",
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false
        };
        var tokenCorreo = new TokenCorreo
        {
            Id = 1,
            UsuarioId = 20,
            Token = "123456",
            FechaGeneracion = DateTime.UtcNow.AddMinutes(-5),
            FechaExpiracion = DateTime.UtcNow.AddHours(24),
            Revocado = true
        };
        db.Usuarios.Add(user);
        db.TokensCorreo.Add(tokenCorreo);
        await db.SaveChangesAsync();

        var principal = CreateUserPrincipal(20);
        var request = new ValidarEmailRequestDto("123456", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Código revocado", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public async Task ValidarAsync_WithExpiredToken_ReturnsBadRequestProblem()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var user = new Usuario
        {
            Id = 25,
            Correo = "expirado@educonnect.com",
            PasswordHash = "hash",
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false
        };
        var tokenCorreo = new TokenCorreo
        {
            Id = 2,
            UsuarioId = 25,
            Token = "123456",
            FechaGeneracion = DateTime.UtcNow.AddHours(-25),
            FechaExpiracion = DateTime.UtcNow.AddHours(-1),
            Revocado = false
        };
        db.Usuarios.Add(user);
        db.TokensCorreo.Add(tokenCorreo);
        await db.SaveChangesAsync();

        var principal = CreateUserPrincipal(25);
        var request = new ValidarEmailRequestDto("123456", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Código expirado", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public async Task ValidarAsync_WithMismatchCode_ReturnsBadRequestProblem()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var user = new Usuario
        {
            Id = 30,
            Correo = "mismatch@educonnect.com",
            PasswordHash = "hash",
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false
        };
        var tokenCorreo = new TokenCorreo
        {
            Id = 3,
            UsuarioId = 30,
            Token = "654321",
            FechaGeneracion = DateTime.UtcNow.AddMinutes(-2),
            FechaExpiracion = DateTime.UtcNow.AddHours(24),
            Revocado = false
        };
        db.Usuarios.Add(user);
        db.TokensCorreo.Add(tokenCorreo);
        await db.SaveChangesAsync();

        var principal = CreateUserPrincipal(30);
        var request = new ValidarEmailRequestDto("111222", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        Assert.Equal("Código incorrecto", problemResult.ProblemDetails.Title);
    }

    [Fact]
    public async Task ValidarAsync_WithValidCode_MarksRevokedAndSetsCorreoValidadoAndReturnsTokenResponse()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var user = new Usuario
        {
            Id = 35,
            Correo = "valido@educonnect.com",
            PasswordHash = "hash",
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false,
            Estudiante = new Estudiante
            {
                UsuarioId = 35,
                Nombre = "Elena",
                Apellido = "Ruiz",
                Carnet = "202100001",
                Genero = "femenino",
                Direccion = "Guatemala",
                Telefono = "87654321",
                FotografiaUrl = "avatars/elena.png"
            }
        };
        var tokenCorreo = new TokenCorreo
        {
            Id = 4,
            UsuarioId = 35,
            Token = "998877",
            FechaGeneracion = DateTime.UtcNow.AddMinutes(-1),
            FechaExpiracion = DateTime.UtcNow.AddHours(24),
            Revocado = false
        };
        db.Usuarios.Add(user);
        db.TokensCorreo.Add(tokenCorreo);
        await db.SaveChangesAsync();

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(35, "valido@educonnect.com", "Estudiante"))
            .Returns("real_jwt_auth_token");

        _s3ServiceMock
            .Setup(s => s.GeneratePresignedUrl("avatars/elena.png", null))
            .Returns("https://s3.amazonaws.com/elena.png");

        var principal = CreateUserPrincipal(35);
        var request = new ValidarEmailRequestDto("998877", null);

        var result = await ValidarEmailEndpoint.ValidarAsync(
            request,
            principal,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var okResult = Assert.IsType<Ok<TokenResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(okResult.Value.CorreoValidado);
        Assert.Equal("real_jwt_auth_token", okResult.Value.Token);
        Assert.Equal(35, okResult.Value.IdUsuario);
        Assert.Equal("Elena", okResult.Value.Nombre);
        Assert.Equal("Ruiz", okResult.Value.Apellido);
        Assert.Equal("https://s3.amazonaws.com/elena.png", okResult.Value.FotografiaUrl);

        var updatedToken = await db.TokensCorreo.FindAsync(4);
        Assert.NotNull(updatedToken);
        Assert.True(updatedToken.Revocado);

        var updatedUser = await db.Usuarios.FindAsync(35);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.CorreoValidado);
    }
}
