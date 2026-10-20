using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador.GestionEstudiantes;
using edu_connect_service.Api.Features.Auth.Login;
using edu_connect_service.Api.Features.Sesiones.ProgramarSesion;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Authentication;
using edu_connect_service.Api.Shared.Emails;
using edu_connect_service.Api.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace edu_connect_service.Api.UnitTests.Features.Administrador.GestionEstudiantes;

/// <summary>
/// Pruebas unitarias para la HU-34: Ver, actualizar y dar de baja estudiantes.
/// </summary>
public class ActualizarEstudianteTests
{
    private static edu_connect_serviceContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new edu_connect_serviceContext(options);
        SeedBaseData(context);
        return context;
    }

    private static void SeedBaseData(edu_connect_serviceContext context)
    {
        var rolTutor = new Rol { Id = 1, Nombre = "Tutor" };
        var rolEstudiante = new Rol { Id = 2, Nombre = "Estudiante" };
        context.Roles.AddRange(rolTutor, rolEstudiante);

        var estadoAprobado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var estadoInactivo = new EstadoUsuario { Id = 2, Nombre = "INACTIVO" };
        var estadoPendiente = new EstadoUsuario { Id = 3, Nombre = "PENDIENTE" };
        context.EstadosUsuarios.AddRange(estadoAprobado, estadoInactivo, estadoPendiente);

        var materia1 = new Materia { Id = 1, Nombre = "Matemática Básica" };
        context.Materias.Add(materia1);

        context.SaveChanges();
    }

    private static Estudiante CrearEstudiantePrueba(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        string apellido,
        string carnet,
        string correo,
        string estadoNombre = "APROBADO",
        string? telefono = "12345678",
        string direccion = "Ciudad de Guatemala",
        string genero = "masculino")
    {
        var estado = context.EstadosUsuarios.First(e => e.Nombre == estadoNombre);
        var rolEstudiante = context.Roles.First(r => r.Nombre == "Estudiante");

        var usuario = new Usuario
        {
            Id = id,
            Correo = correo,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            RolId = rolEstudiante.Id,
            Rol = rolEstudiante,
            EstadoId = estado.Id,
            Estado = estado,
            CorreoValidado = true,
            FechaRegistro = DateTime.UtcNow
        };

        var estudiante = new Estudiante
        {
            UsuarioId = id,
            Usuario = usuario,
            Nombre = nombre,
            Apellido = apellido,
            Carnet = carnet,
            Genero = genero,
            Direccion = direccion,
            Telefono = telefono ?? string.Empty,
            FechaNacimiento = new DateOnly(2002, 3, 15),
            FotografiaUrl = "https://s3.amazonaws.com/estudiante.jpg",
            DocumentoCarnetUrl = "https://s3.amazonaws.com/carnet.pdf"
        };

        context.Usuarios.Add(usuario);
        context.Estudiantes.Add(estudiante);
        context.SaveChanges();

        return estudiante;
    }

    private static Tutor CrearTutorPrueba(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        string apellido,
        string carnet,
        string correo,
        string estadoNombre = "APROBADO")
    {
        var estado = context.EstadosUsuarios.First(e => e.Nombre == estadoNombre);
        var rolTutor = context.Roles.First(r => r.Nombre == "Tutor");

        var usuario = new Usuario
        {
            Id = id,
            Correo = correo,
            PasswordHash = "hash123",
            RolId = rolTutor.Id,
            Rol = rolTutor,
            EstadoId = estado.Id,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow
        };

        var tutor = new Tutor
        {
            UsuarioId = id,
            Usuario = usuario,
            Nombre = nombre,
            Apellido = apellido,
            CarnetId = carnet,
            NumeroIdentificacion = "1234567890101",
            Genero = "masculino",
            Direccion = "Guatemala",
            Telefono = "55551234",
            FechaNacimiento = new DateOnly(1995, 5, 20),
            FotografiaUrl = "https://s3.amazonaws.com/tutor.jpg",
            DireccionTutoria = "Edificio T3",
            AnioInicio = 2020,
            Universidad = "USAC",
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(17, 0)
        };

        var materia = context.Materias.First(m => m.Id == 1);
        tutor.TutorMaterias.Add(new TutorMateria
        {
            TutorId = id,
            MateriaId = materia.Id,
            Materia = materia
        });

        context.Usuarios.Add(usuario);
        context.Tutores.Add(tutor);
        context.SaveChanges();

        return tutor;
    }

    [Fact]
    public async Task ActualizarEstudiante_ConDatosValidos_DebeActualizarCamposYRetornarOk()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 10, "Juan", "Perez", "202201234", "juan.perez@educonnect.com");

        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Juan Carlos",
            Apellido: "Perez Gomez",
            Carnet: "202201234",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(2001, 7, 20),
            Direccion: "Zona 12, Ciudad Universitaria",
            Telefono: "87654321",
            FotografiaUrl: "https://s3.amazonaws.com/nueva_foto.jpg",
            DocumentoCarnetUrl: "https://s3.amazonaws.com/nuevo_carnet.pdf"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(10, request, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<ActualizarEstudianteAdminResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("Juan Carlos", okResult.Value.Nombre);
        Assert.Equal("Perez Gomez", okResult.Value.Apellido);
        Assert.Equal("202201234", okResult.Value.Carnet);
        Assert.Equal("masculino", okResult.Value.Genero);
        Assert.Equal(new DateOnly(2001, 7, 20), okResult.Value.FechaNacimiento);
        Assert.Equal("Zona 12, Ciudad Universitaria", okResult.Value.Direccion);
        Assert.Equal("87654321", okResult.Value.Telefono);
        Assert.Equal("https://s3.amazonaws.com/nueva_foto.jpg", okResult.Value.FotografiaUrl);
        Assert.Equal("https://s3.amazonaws.com/nuevo_carnet.pdf", okResult.Value.DocumentoCarnetUrl);

        // Verificar persistencia directa en DB
        var estudianteDb = await context.Estudiantes.FirstAsync(e => e.UsuarioId == 10);
        Assert.Equal("Juan Carlos", estudianteDb.Nombre);
        Assert.Equal("Perez Gomez", estudianteDb.Apellido);
        Assert.Equal("Zona 12, Ciudad Universitaria", estudianteDb.Direccion);
        Assert.Equal("87654321", estudianteDb.Telefono);
    }

    [Fact]
    public async Task ActualizarEstudiante_IntentoDeModificarCorreo_DebeConservarCorreoOriginal()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        const string correoOriginal = "estudiante.inmutable@educonnect.com";
        CrearEstudiantePrueba(context, 11, "Maria", "Lopez", "202209999", correoOriginal);

        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Maria Jose",
            Apellido: "Lopez Ruiz",
            Carnet: "202209999",
            Genero: "femenino",
            FechaNacimiento: new DateOnly(2002, 4, 10),
            Direccion: "Zona 1, Centro",
            Telefono: "44443333"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(11, request, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<ActualizarEstudianteAdminResponseDto>>(result);
        Assert.Equal(correoOriginal, okResult.Value!.Correo);

        var usuarioDb = await context.Usuarios.FirstAsync(u => u.Id == 11);
        Assert.Equal(correoOriginal, usuarioDb.Correo);
    }

    [Fact]
    public async Task ActualizarEstudiante_CarnetDuplicadoConOtroEstudiante_DebeRetornarConflicto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 12, "Estudiante Uno", "Alpha", "202200001", "uno@educonnect.com");
        CrearEstudiantePrueba(context, 13, "Estudiante Dos", "Beta", "202200002", "dos@educonnect.com");

        // Intentar actualizar Estudiante Dos usando el carnet del Estudiante Uno
        var requestCarnetDuplicado = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Estudiante Dos Modificado",
            Apellido: "Beta",
            Carnet: "202200001", // Duplicado
            Genero: "masculino",
            FechaNacimiento: new DateOnly(2000, 1, 1),
            Direccion: "Zona 10",
            Telefono: "77778888"
        );

        // Act
        var resultCarnet = await ActualizarEstudianteEndpoint.HandleAsync(13, requestCarnetDuplicado, context, CancellationToken.None);

        // Assert
        var problemCarnet = Assert.IsType<ProblemHttpResult>(resultCarnet);
        Assert.Equal(StatusCodes.Status409Conflict, problemCarnet.StatusCode);
        Assert.Equal("Carnet duplicado", problemCarnet.ProblemDetails.Title);
    }

    [Fact]
    public async Task ActualizarEstudiante_MismoCarnetDelMismoEstudiante_DebePermitirActualizacion()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 14, "Estudiante Mismo", "Gamma", "202200003", "mismo@educonnect.com");

        var requestMismoCarnet = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Estudiante Nombre Actualizado",
            Apellido: "Gamma",
            Carnet: "202200003", // Mismo carnet
            Genero: "masculino",
            FechaNacimiento: new DateOnly(2000, 1, 1),
            Direccion: "Zona 10",
            Telefono: "77778888"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(14, requestMismoCarnet, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<ActualizarEstudianteAdminResponseDto>>(result);
        Assert.Equal("Estudiante Nombre Actualizado", okResult.Value!.Nombre);
        Assert.Equal("202200003", okResult.Value.Carnet);
    }

    [Fact]
    public async Task ActualizarEstudiante_EstudianteInexistente_DebeRetornarNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Inexistente",
            Apellido: "Estudiante",
            Carnet: "202208888",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(2000, 1, 1),
            Direccion: "Zona 1",
            Telefono: "12345678"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(999, request, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
    }

    [Fact]
    public async Task ActualizarEstudiante_EstudianteNoActivo_DebeRetornarBadRequest()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 15, "Estudiante", "Inactivo", "202207777", "inactivo@educonnect.com", "INACTIVO");

        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Estudiante",
            Apellido: "Inactivo Mod",
            Carnet: "202207777",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(2000, 1, 1),
            Direccion: "Zona 1",
            Telefono: "12345678"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(15, request, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Operación no permitida", problem.ProblemDetails.Title);
    }

    [Theory]
    [InlineData("", "Perez", "202201234", "masculino", "Zona 1", "12345678", "Nombre obligatorio")]
    [InlineData("Juan", "", "202201234", "masculino", "Zona 1", "12345678", "Apellido obligatorio")]
    [InlineData("Juan", "Perez", "abc123", "masculino", "Zona 1", "12345678", "Carnet inválido")]
    [InlineData("Juan", "Perez", "123", "masculino", "Zona 1", "12345678", "Carnet inválido")]
    [InlineData("Juan", "Perez", "202201234", "otro_genero", "Zona 1", "12345678", "Género inválido")]
    [InlineData("Juan", "Perez", "202201234", "masculino", "", "12345678", "Dirección obligatoria")]
    [InlineData("Juan", "Perez", "202201234", "masculino", "Zona 1", "1234", "Teléfono inválido")]
    public async Task ActualizarEstudiante_ValidacionesCamposInvalidos_DebeRetornarBadRequest(
        string nombre,
        string apellido,
        string carnet,
        string genero,
        string direccion,
        string telefono,
        string expectedTitle)
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 16, "Juan", "Perez", "202201234", "juan.val@educonnect.com");

        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: nombre,
            Apellido: apellido,
            Carnet: carnet,
            Genero: genero,
            FechaNacimiento: new DateOnly(2001, 1, 1),
            Direccion: direccion,
            Telefono: telefono
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(16, request, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal(expectedTitle, problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task ActualizarEstudiante_FechaNacimientoFutura_DebeRetornarBadRequest()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 17, "Juan", "Perez", "202201234", "juan.fecha@educonnect.com");

        var request = new ActualizarEstudianteAdminRequestDto(
            Nombre: "Juan",
            Apellido: "Perez",
            Carnet: "202201234",
            Genero: "masculino",
            FechaNacimiento: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            Direccion: "Zona 1",
            Telefono: "12345678"
        );

        // Act
        var result = await ActualizarEstudianteEndpoint.HandleAsync(17, request, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Fecha de nacimiento inválida", problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task ObtenerEstudiantePorId_EstudianteExistente_DebeRetornarOk()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 18, "Lucia", "Morales", "202205555", "lucia@educonnect.com");

        // Act
        var result = await ObtenerEstudiantePorIdEndpoint.HandleAsync(18, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<EstudianteActivoResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(18, okResult.Value.Id);
        Assert.Equal("Lucia", okResult.Value.Nombre);
        Assert.Equal("Morales", okResult.Value.Apellido);
        Assert.Equal("202205555", okResult.Value.Carnet);
        Assert.Equal("lucia@educonnect.com", okResult.Value.Correo);
        Assert.Equal("APROBADO", okResult.Value.Estado);
    }

    [Fact]
    public async Task ObtenerEstudiantePorId_EstudianteInexistente_DebeRetornarNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act
        var result = await ObtenerEstudiantePorIdEndpoint.HandleAsync(999, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
    }

    [Fact]
    public async Task DarBajaEstudiante_EstudianteActivo_DebePasarAEstadoInactivoYRegistrarFecha()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 19, "Carlos", "Mendez", "202206666", "carlos.baja@educonnect.com");

        var mockEmail = new Mock<IEmailService>();
        mockEmail.Setup(e => e.SendBajaCuentaNotificacionAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        var request = new DarBajaEstudianteRequestDto("Faltas al reglamento disciplinario");

        // Act
        var result = await DarBajaEstudianteEndpoint.HandleAsync(19, request, context, mockEmail.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<DarBajaEstudianteResponseDto>>(result);
        Assert.Equal("INACTIVO", okResult.Value!.Estado);
        Assert.NotNull(okResult.Value.FechaBaja);
        Assert.Equal("Faltas al reglamento disciplinario", okResult.Value.Motivo);

        var estudianteDb = await context.Estudiantes
            .Include(e => e.Usuario)
            .ThenInclude(u => u.Estado)
            .FirstAsync(e => e.UsuarioId == 19);

        Assert.Equal("INACTIVO", estudianteDb.Usuario.Estado.Nombre);
        Assert.NotNull(estudianteDb.Usuario.FechaBaja);

        mockEmail.Verify(e => e.SendBajaCuentaNotificacionAsync(
            "carlos.baja@educonnect.com",
            "Carlos Mendez",
            "Faltas al reglamento disciplinario",
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task DarBajaEstudiante_EstudianteYaInactivo_DebeRetornarBadRequest()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 20, "Estudiante", "YaInactivo", "202207778", "ya.inactivo@educonnect.com", "INACTIVO");

        var mockEmail = new Mock<IEmailService>();
        var request = new DarBajaEstudianteRequestDto("Reintento de baja");

        // Act
        var result = await DarBajaEstudianteEndpoint.HandleAsync(20, request, context, mockEmail.Object, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Operación no permitida", problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task DarBajaEstudiante_EstudianteInexistente_DebeRetornarNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mockEmail = new Mock<IEmailService>();
        var request = new DarBajaEstudianteRequestDto("Motivo");

        // Act
        var result = await DarBajaEstudianteEndpoint.HandleAsync(999, request, context, mockEmail.Object, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
    }

    [Fact]
    public async Task ProgramarSesion_EstudianteDadoDeBaja_DebeRechazarReservaConError()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 21, "Tutor", "Activo", "201812345", "tutor.activo@educonnect.com");
        CrearEstudiantePrueba(context, 22, "Estudiante", "DadoDeBaja", "202209991", "estudiante.baja@educonnect.com", "INACTIVO");

        var request = new ProgramarSesionRequestDto(
            TutorId: 21,
            MateriaId: 1,
            FechaSesion: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            HoraInicio: new TimeOnly(10, 0),
            Motivo: "Consulta académica"
        );

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id_usuario", "22"),
            new Claim("rol", "Estudiante")
        }, "TestAuth"));

        // Act
        var result = await ProgramarSesionEndpoint.HandleAsync(request, claims, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, problem.StatusCode);
        Assert.Equal("Estudiante inactivo", problem.ProblemDetails.Title);
    }

    [Fact]
    public async Task Login_EstudianteDadoDeBaja_DebeRetornarForbidden()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 23, "Estudiante", "BajaLogin", "202209992", "login.baja@educonnect.com", "INACTIVO");

        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var s3ServiceMock = new Mock<IS3Service>();
        var jwtOptions = Options.Create(new JwtOptions { ExpirationMinutes = 60 });

        var request = new LoginRequestDto("login.baja@educonnect.com", "Password123!");

        // Act
        var result = await LoginEndpoint.HandleAsync(
            request,
            context,
            jwtTokenServiceMock.Object,
            s3ServiceMock.Object,
            jwtOptions,
            CancellationToken.None
        );

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, problem.StatusCode);
        Assert.Equal("Usuario no habilitado", problem.ProblemDetails.Title);
        Assert.Contains("INACTIVO", problem.ProblemDetails.Detail);
    }
}
