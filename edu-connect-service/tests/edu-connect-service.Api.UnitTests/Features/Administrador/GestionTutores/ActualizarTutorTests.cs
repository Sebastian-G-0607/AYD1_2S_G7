using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador.GestionTutores;
using edu_connect_service.Api.Features.Sesiones.ProgramarSesion;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace edu_connect_service.Api.UnitTests.Features.Administrador.GestionTutores;

/// <summary>
/// Pruebas unitarias para la HU-35: Ver, actualizar y dar de baja tutores.
/// Cada prueba documenta su caso de prueba, resultado esperado y verificaciones.
/// </summary>
public class ActualizarTutorTests
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
        var materia2 = new Materia { Id = 2, Nombre = "Física 1" };
        var materia3 = new Materia { Id = 3, Nombre = "Química General" };
        context.Materias.AddRange(materia1, materia2, materia3);

        context.SaveChanges();
    }

    private static Tutor CrearTutorPrueba(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        string apellido,
        string carnet,
        string dpi,
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
            NumeroIdentificacion = dpi,
            Genero = "masculino",
            Direccion = "Ciudad de Guatemala",
            Telefono = "55551234",
            FechaNacimiento = new DateOnly(1995, 5, 20),
            FotografiaUrl = "https://s3.amazonaws.com/perfil.jpg",
            DireccionTutoria = "Edificio T3, Salón 201",
            AnioInicio = 2020,
            Universidad = "Universidad de San Carlos de Guatemala",
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

    /// <summary>
    /// CASO DE PRUEBA 1:
    /// El administrador actualiza la información académica y personal de un tutor activo con datos válidos.
    /// RESULTADO ESPERADO:
    /// Retorna 200 OK con mensaje de éxito y los cambios se persisten correctamente en la base de datos.
    /// </summary>
    [Fact]
    public async Task ActualizarTutor_ConDatosValidos_DebeActualizarCamposYRetornarOk()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 10, "Carlos", "Perez", "202012345", "1234567890101", "carlos.perez@educonnect.com");

        var request = new ActualizarTutorAdminRequestDto(
            Nombre: "Carlos Alberto",
            Apellido: "Perez Lopez",
            CarnetId: "202012345",
            NumeroIdentificacion: "1234567890101",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(1994, 8, 15),
            Direccion: "Zona 11, Las Charcas",
            Telefono: "44445555",
            DireccionTutoria: "Laboratorio de Cómputo 3",
            AnioInicio: 2018,
            Universidad: "USAC - Facultad de Ingeniería",
            Materias: new List<string> { "Física 1", "Química General" }
        );

        // Act
        var result = await ActualizarTutorEndpoint.HandleAsync(10, request, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<ActualizarTutorAdminResponseDto>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("Carlos Alberto", okResult.Value.Nombre);
        Assert.Equal("Perez Lopez", okResult.Value.Apellido);
        Assert.Equal("USAC - Facultad de Ingeniería", okResult.Value.Universidad);
        Assert.Equal("44445555", okResult.Value.Telefono);
        Assert.Contains("Física 1", okResult.Value.Materias);

        // Verificar persistencia directa en DB
        var tutorDb = await context.Tutores
            .Include(t => t.TutorMaterias)
            .ThenInclude(tm => tm.Materia)
            .FirstAsync(t => t.UsuarioId == 10);

        Assert.Equal("Carlos Alberto", tutorDb.Nombre);
        Assert.Equal(2018, tutorDb.AnioInicio);
        Assert.Equal(2, tutorDb.TutorMaterias.Count);
    }

    /// <summary>
    /// CASO DE PRUEBA 2:
    /// Se valida la invariante de negocio de que el correo electrónico no puede ser modificado por la edición.
    /// RESULTADO ESPERADO:
    /// El correo en la entidad Usuario permanece inmutable y exactamente igual al original.
    /// </summary>
    [Fact]
    public async Task ActualizarTutor_IntentoDeModificarCorreo_DebeConservarCorreoOriginal()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        const string correoOriginal = "tutor.inmutable@educonnect.com";
        CrearTutorPrueba(context, 11, "Maria", "Lopez", "201999888", "9876543210101", correoOriginal);

        var request = new ActualizarTutorAdminRequestDto(
            Nombre: "Maria Elena",
            Apellido: "Lopez Ruiz",
            CarnetId: "201999888",
            NumeroIdentificacion: "9876543210101",
            Genero: "femenino",
            FechaNacimiento: new DateOnly(1996, 3, 10),
            Direccion: "Zona 1, Centro",
            Telefono: "55558888",
            DireccionTutoria: "Biblioteca Central",
            AnioInicio: 2021,
            Universidad: "USAC"
        );

        // Act
        var result = await ActualizarTutorEndpoint.HandleAsync(11, request, context, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<ActualizarTutorAdminResponseDto>>(result);
        Assert.Equal(correoOriginal, okResult.Value!.Correo);

        var usuarioDb = await context.Usuarios.FirstAsync(u => u.Id == 11);
        Assert.Equal(correoOriginal, usuarioDb.Correo);
    }

    /// <summary>
    /// CASO DE PRUEBA 3:
    /// El administrador intenta asignar un Carnet o DPI que ya le pertenece a otro tutor.
    /// RESULTADO ESPERADO:
    /// Retorna 409 Conflict indicando duplicidad de credencial.
    /// </summary>
    [Fact]
    public async Task ActualizarTutor_CarnetODpiDuplicadoConOtroTutor_DebeRetornarConflicto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 12, "Tutor Uno", "Alpha", "201800001", "1111111110101", "uno@educonnect.com");
        CrearTutorPrueba(context, 13, "Tutor Dos", "Beta", "201800002", "2222222220101", "dos@educonnect.com");

        // Intentar actualizar Tutor Dos usando el Carnet del Tutor Uno
        var requestCarnetDuplicado = new ActualizarTutorAdminRequestDto(
            Nombre: "Tutor Dos Modificado",
            Apellido: "Beta",
            CarnetId: "201800001", // Duplicado con Tutor Uno
            NumeroIdentificacion: "2222222220101",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(1993, 1, 1),
            Direccion: "Zona 10",
            Telefono: "77778888",
            DireccionTutoria: "Sala A",
            AnioInicio: 2019,
            Universidad: "USAC"
        );

        // Act
        var resultCarnet = await ActualizarTutorEndpoint.HandleAsync(13, requestCarnetDuplicado, context, CancellationToken.None);

        // Assert
        var problemCarnet = Assert.IsType<ProblemHttpResult>(resultCarnet);
        Assert.Equal(StatusCodes.Status409Conflict, problemCarnet.StatusCode);
        Assert.Equal("Carnet duplicado", problemCarnet.ProblemDetails.Title);

        // Intentar actualizar Tutor Dos usando el DPI del Tutor Uno
        var requestDpiDuplicado = new ActualizarTutorAdminRequestDto(
            Nombre: "Tutor Dos Modificado",
            Apellido: "Beta",
            CarnetId: "201800002",
            NumeroIdentificacion: "1111111110101", // Duplicado con Tutor Uno
            Genero: "masculino",
            FechaNacimiento: new DateOnly(1993, 1, 1),
            Direccion: "Zona 10",
            Telefono: "77778888",
            DireccionTutoria: "Sala A",
            AnioInicio: 2019,
            Universidad: "USAC"
        );

        var resultDpi = await ActualizarTutorEndpoint.HandleAsync(13, requestDpiDuplicado, context, CancellationToken.None);
        var problemDpi = Assert.IsType<ProblemHttpResult>(resultDpi);
        Assert.Equal(StatusCodes.Status409Conflict, problemDpi.StatusCode);
        Assert.Equal("Documento de identificación duplicado", problemDpi.ProblemDetails.Title);
    }

    /// <summary>
    /// CASO DE PRUEBA 4:
    /// El administrador intenta actualizar un tutor con un ID inexistente.
    /// RESULTADO ESPERADO:
    /// Retorna 404 NotFound.
    /// </summary>
    [Fact]
    public async Task ActualizarTutor_TutorInexistente_DebeRetornarNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var request = new ActualizarTutorAdminRequestDto(
            Nombre: "Fantasma",
            Apellido: "Inexistente",
            CarnetId: "202099999",
            NumeroIdentificacion: "9999999990101",
            Genero: "masculino",
            FechaNacimiento: new DateOnly(1990, 1, 1),
            Direccion: "Zona 1",
            Telefono: "12345678",
            DireccionTutoria: "Online",
            AnioInicio: 2020,
            Universidad: "USAC"
        );

        // Act
        var result = await ActualizarTutorEndpoint.HandleAsync(999, request, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problem.StatusCode);
    }

    /// <summary>
    /// CASO DE PRUEBA 5:
    /// El administrador da de baja a un tutor activo (HU-35 CA-03).
    /// RESULTADO ESPERADO:
    /// Estado del usuario cambia a 'INACTIVO', se registra fecha y motivo de baja, y se envía el correo.
    /// </summary>
    [Fact]
    public async Task DarBajaTutor_TutorActivo_DebePasarAEstadoInactivoYRegistrarFecha()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 14, "Fernando", "Ruiz", "201712345", "3333333330101", "fernando@educonnect.com");

        var mockEmail = new Mock<IEmailService>();
        mockEmail.Setup(e => e.SendBajaCuentaNotificacionAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        var request = new DarBajaTutorRequestDto("Incumplimiento de normativas de tutoría");

        // Act
        var result = await DarBajaTutorEndpoint.HandleAsync(14, request, context, mockEmail.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<DarBajaTutorResponseDto>>(result);
        Assert.Equal("INACTIVO", okResult.Value!.Estado);
        Assert.NotNull(okResult.Value.FechaBaja);
        Assert.Equal("Incumplimiento de normativas de tutoría", okResult.Value.Motivo);

        var tutorDb = await context.Tutores
            .Include(t => t.Usuario)
            .ThenInclude(u => u.Estado)
            .FirstAsync(t => t.UsuarioId == 14);

        Assert.Equal("INACTIVO", tutorDb.Usuario.Estado.Nombre);
        Assert.NotNull(tutorDb.Usuario.FechaBaja);

        mockEmail.Verify(e => e.SendBajaCuentaNotificacionAsync(
            "fernando@educonnect.com",
            "Fernando Ruiz",
            "Incumplimiento de normativas de tutoría",
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    /// <summary>
    /// CASO DE PRUEBA 6:
    /// Un estudiante intenta programar sesión con un tutor dado de baja (HU-35 CA-04).
    /// RESULTADO ESPERADO:
    /// Retorna 400 BadRequest indicando que el tutor no está disponible para recibir reservas.
    /// </summary>
    [Fact]
    public async Task ProgramarSesion_TutorDadoDeBaja_DebeRechazarReservaConError()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        // Tutor con estado INACTIVO
        CrearTutorPrueba(context, 20, "Tutor", "Inactivo", "201612345", "4444444440101", "inactivo@educonnect.com", "INACTIVO");

        // Crear estudiante activo
        var rolEstudiante = context.Roles.First(r => r.Nombre == "Estudiante");
        var estadoAprobado = context.EstadosUsuarios.First(e => e.Nombre == "APROBADO");
        var usuarioEst = new Usuario
        {
            Id = 30,
            Correo = "estudiante@educonnect.com",
            PasswordHash = "hash123",
            RolId = rolEstudiante.Id,
            Rol = rolEstudiante,
            EstadoId = estadoAprobado.Id,
            Estado = estadoAprobado
        };
        var estudiante = new Estudiante
        {
            UsuarioId = 30,
            Usuario = usuarioEst,
            Nombre = "Juan",
            Apellido = "Estudiante",
            Carnet = "202200001",
            Genero = "masculino",
            Direccion = "Guatemala",
            Telefono = "11223344",
            FechaNacimiento = new DateOnly(2002, 1, 1),
            FotografiaUrl = ""
        };
        context.Usuarios.Add(usuarioEst);
        context.Estudiantes.Add(estudiante);
        await context.SaveChangesAsync();

        var request = new ProgramarSesionRequestDto(
            TutorId: 20,
            MateriaId: 1,
            FechaSesion: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            HoraInicio: new TimeOnly(10, 0),
            Motivo: "Dudas sobre derivadas parciales"
        );

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id_usuario", "30"),
            new Claim("rol", "Estudiante")
        }, "TestAuth"));

        // Act
        var result = await ProgramarSesionEndpoint.HandleAsync(request, claims, context, CancellationToken.None);

        // Assert
        var problem = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("Tutor no disponible", problem.ProblemDetails.Title);
    }
}
