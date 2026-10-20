using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Auth.ReenviarTokenEmail;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.BackgroundTasks;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Auth.ReenviarTokenEmail;

public class ReenviarTokenEmailEndpointTests
{
    private readonly Mock<ITokenCorreoService> _tokenCorreoServiceMock = new();
    private readonly Mock<IBackgroundTaskQueue> _taskQueueMock = new();

    private static edu_connect_serviceContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: $"ReenviarTokenTestDb_{Guid.NewGuid()}")
            .Options;
        return new edu_connect_serviceContext(options);
    }

    private static ClaimsPrincipal CreateUserPrincipal(int? usuarioId, string? correo = "estudiante@educonnect.com")
    {
        var claims = new List<Claim>();
        if (usuarioId.HasValue)
        {
            claims.Add(new("id_usuario", usuarioId.Value.ToString()));
            claims.Add(new(ClaimTypes.NameIdentifier, usuarioId.Value.ToString()));
        }
        if (!string.IsNullOrWhiteSpace(correo))
        {
            claims.Add(new("correo", correo));
        }
        claims.Add(new("scope", "email_validation"));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task ReenviarAsync_WithoutUserIdClaim_ReturnsUnauthorizedProblem()
    {
        using var db = CreateInMemoryDbContext();
        var principal = CreateUserPrincipal(null);

        var result = await ReenviarTokenEmailEndpoint.ReenviarAsync(
            principal,
            db,
            _tokenCorreoServiceMock.Object,
            _taskQueueMock.Object,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, problemResult.StatusCode);
    }

    [Fact]
    public async Task ReenviarAsync_WhenUserNotFound_ReturnsNotFoundProblem()
    {
        using var db = CreateInMemoryDbContext();
        var principal = CreateUserPrincipal(999);

        var result = await ReenviarTokenEmailEndpoint.ReenviarAsync(
            principal,
            db,
            _tokenCorreoServiceMock.Object,
            _taskQueueMock.Object,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
    }

    [Fact]
    public async Task ReenviarAsync_WhenEmailAlreadyValidated_ReturnsBadRequestProblem()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "Activo" };
        db.Roles.Add(rol);
        db.EstadosUsuarios.Add(estado);

        var usuario = new Usuario
        {
            Id = 15,
            Correo = "validado@educonnect.com",
            PasswordHash = "hash123",
            RolId = 1,
            Rol = rol,
            EstadoId = 1,
            Estado = estado,
            CorreoValidado = true
        };
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var principal = CreateUserPrincipal(15, "validado@educonnect.com");

        var result = await ReenviarTokenEmailEndpoint.ReenviarAsync(
            principal,
            db,
            _tokenCorreoServiceMock.Object,
            _taskQueueMock.Object,
            CancellationToken.None
        );

        var problemResult = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task ReenviarAsync_WithExistingToken_RevokesPreviousAndCreatesNewTokenAndEnqueuesTask()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "Pendiente" };
        db.Roles.Add(rol);
        db.EstadosUsuarios.Add(estado);

        var usuario = new Usuario
        {
            Id = 20,
            Correo = "estudiante.nuevo@educonnect.com",
            PasswordHash = "hash123",
            RolId = 1,
            Rol = rol,
            EstadoId = 1,
            Estado = estado,
            CorreoValidado = false,
            Estudiante = new Estudiante
            {
                UsuarioId = 20,
                Nombre = "Carlos",
                Apellido = "Ruiz",
                Carnet = "202012345",
                Genero = "M",
                Direccion = "Ciudad",
                Telefono = "12345678",
                FechaNacimiento = new DateOnly(2000, 1, 1),
                FotografiaUrl = "foto.jpg",
                DocumentoCarnetUrl = "carnet.pdf"
            }
        };
        db.Usuarios.Add(usuario);

        var tokenAnterior = new TokenCorreo
        {
            Id = 100,
            UsuarioId = 20,
            Token = "111111",
            FechaGeneracion = DateTime.UtcNow.AddHours(-2),
            FechaExpiracion = DateTime.UtcNow.AddHours(22),
            Revocado = false
        };
        db.TokensCorreo.Add(tokenAnterior);
        await db.SaveChangesAsync();

        _tokenCorreoServiceMock.Setup(s => s.GenerarToken()).Returns("654321");

        Func<IServiceProvider, CancellationToken, ValueTask>? capturedWorkItem = null;
        _taskQueueMock.Setup(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>()))
            .Callback<Func<IServiceProvider, CancellationToken, ValueTask>>(wi => capturedWorkItem = wi)
            .Returns(ValueTask.CompletedTask);

        var principal = CreateUserPrincipal(20, "estudiante.nuevo@educonnect.com");

        var result = await ReenviarTokenEmailEndpoint.ReenviarAsync(
            principal,
            db,
            _tokenCorreoServiceMock.Object,
            _taskQueueMock.Object,
            CancellationToken.None
        );

        var acceptedResult = Assert.IsType<Accepted>(result);
        Assert.Equal(StatusCodes.Status202Accepted, acceptedResult.StatusCode);

        var tokenViejoActualizado = await db.TokensCorreo.FirstAsync(t => t.Id == 100);
        Assert.True(tokenViejoActualizado.Revocado);

        var nuevoToken = await db.TokensCorreo
            .OrderByDescending(t => t.FechaGeneracion)
            .FirstAsync(t => t.UsuarioId == 20 && t.Id != 100);

        Assert.Equal("654321", nuevoToken.Token);
        Assert.False(nuevoToken.Revocado);
        Assert.True(nuevoToken.FechaExpiracion > DateTime.UtcNow);

        Assert.NotNull(capturedWorkItem);
        _taskQueueMock.Verify(q => q.QueueBackgroundWorkItemAsync(It.IsAny<Func<IServiceProvider, CancellationToken, ValueTask>>()), Times.Once);
    }
}
