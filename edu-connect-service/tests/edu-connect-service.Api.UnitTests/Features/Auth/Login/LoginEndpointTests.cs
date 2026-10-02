using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Auth.Login;

public class LoginEndpointTests
{
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IS3Service> _s3ServiceMock = new();
    private readonly IOptions<JwtOptions> _jwtOptions = Options.Create(new JwtOptions
    {
        Key = "very_long_secret_key_used_for_testing_purposes_123456",
        Issuer = "EduConnect",
        Audience = "EduConnectUsers",
        ExpirationMinutes = 120
    });

    private edu_connect_serviceContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: "LoginEndpointTests_" + Guid.NewGuid())
            .Options;

        return new edu_connect_serviceContext(options);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUser_ReturnsProblemDetails401()
    {
        using var db = CreateInMemoryDbContext();
        var request = new LoginRequestDto("inexistente@educonnect.com", "Password123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.StatusCode);
        Assert.Equal("Credenciales inválidas", problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task HandleAsync_WithIncorrectPassword_ReturnsProblemDetails401()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };

        var user = new Usuario
        {
            Correo = "usuario@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto("usuario@educonnect.com", "WrongPassword123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, problem.StatusCode);
        Assert.Equal("Credenciales inválidas", problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotAprobado_ReturnsProblemDetails403()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 2, Nombre = "PENDIENTE" };

        var user = new Usuario
        {
            Correo = "pendiente@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        var request = new LoginRequestDto("pendiente@educonnect.com", "Password123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, problem.StatusCode);
        Assert.Equal("Usuario no habilitado", problem.ProblemDetails.Title);
        Assert.Contains("PENDIENTE", problem.ProblemDetails.Detail);
    }

    [Fact]
    public async Task HandleAsync_WithValidEstudiante_ReturnsOkWithTokenAndInfo()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };

        var user = new Usuario
        {
            Id = 50,
            Correo = "estudiante@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = true,
            Estudiante = new Estudiante
            {
                UsuarioId = 50,
                Nombre = "Carlos",
                Apellido = "Lopez",
                Carnet = "202012345",
                Genero = "masculino",
                Direccion = "Ciudad",
                Telefono = "12345678",
                FotografiaUrl = "avatars/carlos.jpg"
            }
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(user.Id, user.Correo, rol.Nombre))
            .Returns("token_jwt_valido");

        _s3ServiceMock
            .Setup(s => s.GeneratePresignedUrl("avatars/carlos.jpg", null))
            .Returns("https://s3.amazonaws.com/carlos.jpg?token=abc");

        var request = new LoginRequestDto("estudiante@educonnect.com", "CorrectPassword123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var okResult = Assert.IsType<Ok<TokenResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(okResult.Value.CorreoValidado);
        Assert.Equal("token_jwt_valido", okResult.Value.Token);
        Assert.Equal("Bearer", okResult.Value.TokenType);
        Assert.Equal(7200, okResult.Value.ExpiresIn);
        Assert.Equal(50, okResult.Value.IdUsuario);
        Assert.Equal("estudiante@educonnect.com", okResult.Value.Correo);
        Assert.Equal("Estudiante", okResult.Value.Rol);
        Assert.Equal("Carlos", okResult.Value.Nombre);
        Assert.Equal("Lopez", okResult.Value.Apellido);
        Assert.Equal("https://s3.amazonaws.com/carlos.jpg?token=abc", okResult.Value.FotografiaUrl);
    }

    [Fact]
    public async Task HandleAsync_WithValidTutorWithoutPhoto_ReturnsOkWithNullPhotoUrl()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 2, Nombre = "Tutor" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };

        var user = new Usuario
        {
            Id = 60,
            Correo = "tutor@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = true,
            Tutor = new Tutor
            {
                UsuarioId = 60,
                Nombre = "Ana",
                Apellido = "Martinez",
                CarnetId = "987654",
                NumeroIdentificacion = "1234567890123",
                Telefono = "87654321",
                Genero = "femenino",
                Direccion = "Antigua",
                DireccionTutoria = "Virtual",
                Universidad = "USAC",
                FotografiaUrl = ""
            }
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(user.Id, user.Correo, rol.Nombre))
            .Returns("token_jwt_tutor");

        var request = new LoginRequestDto("tutor@educonnect.com", "CorrectPassword123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var okResult = Assert.IsType<Ok<TokenResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(okResult.Value.CorreoValidado);
        Assert.Equal("token_jwt_tutor", okResult.Value.Token);
        Assert.Equal(60, okResult.Value.IdUsuario);
        Assert.Equal("Ana", okResult.Value.Nombre);
        Assert.Equal("Martinez", okResult.Value.Apellido);
        Assert.Null(okResult.Value.FotografiaUrl);
    }

    [Fact]
    public async Task HandleAsync_WithUnverifiedEmail_ReturnsOkWithTemporaryToken()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 1, Nombre = "Estudiante" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };

        var user = new Usuario
        {
            Id = 70,
            Correo = "no_validado@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false,
            Estudiante = new Estudiante
            {
                UsuarioId = 70,
                Nombre = "Pedro",
                Apellido = "Gomez",
                Carnet = "202099999",
                Genero = "masculino",
                Direccion = "Ciudad",
                Telefono = "12345678"
            }
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        _jwtTokenServiceMock
            .Setup(j => j.GenerateEmailValidationToken(user.Id, user.Correo))
            .Returns("temp_validation_token");

        var request = new LoginRequestDto("no_validado@educonnect.com", "CorrectPassword123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var okResult = Assert.IsType<Ok<TokenResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.False(okResult.Value.CorreoValidado);
        Assert.Equal("temp_validation_token", okResult.Value.Token);
        Assert.Equal("Bearer", okResult.Value.TokenType);
        Assert.Equal(120, okResult.Value.ExpiresIn);
        Assert.Equal(70, okResult.Value.IdUsuario);
        Assert.Null(okResult.Value.FotografiaUrl);
    }

    [Fact]
    public async Task HandleAsync_WithAdminRole_ReturnsOkWithAuthTokenAndVerifiedEmail()
    {
        using var db = CreateInMemoryDbContext();
        var rol = new Rol { Id = 3, Nombre = "Administrador" };
        var estado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };

        var user = new Usuario
        {
            Id = 80,
            Correo = "admin@educonnect.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Rol = rol,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow,
            CorreoValidado = false
        };
        db.Usuarios.Add(user);
        await db.SaveChangesAsync();

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(user.Id, user.Correo, rol.Nombre))
            .Returns("admin_auth_token");

        var request = new LoginRequestDto("admin@educonnect.com", "CorrectPassword123!");

        var result = await LoginEndpoint.HandleAsync(
            request,
            db,
            _jwtTokenServiceMock.Object,
            _s3ServiceMock.Object,
            _jwtOptions,
            CancellationToken.None
        );

        var okResult = Assert.IsType<Ok<TokenResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.True(okResult.Value.CorreoValidado);
        Assert.Equal("admin_auth_token", okResult.Value.Token);
        Assert.Equal(7200, okResult.Value.ExpiresIn);
    }
}
