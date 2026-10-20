using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Estudiantes.CalificarTutor;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.UnitTests.Features.Estudiantes.CalificarTutor;

public class CalificarTutorEndpointTests
{
    private const int EstudianteId = 11;
    private const int SesionId = 20;

    private static edu_connect_serviceContext CreateContext(
        string estadoSesion = "ATENDIDA",
        bool yaCalificada = false)
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new edu_connect_serviceContext(options);

        var rolTutor = new Rol { Id = 1, Nombre = "Tutor" };
        var rolEstudiante = new Rol { Id = 2, Nombre = "Estudiante" };
        var estadoUsuario = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var tutorUsuario = new Usuario
        {
            Id = 10,
            Correo = "tutor@educonnect.test",
            PasswordHash = "hash",
            RolId = rolTutor.Id,
            Rol = rolTutor,
            EstadoId = estadoUsuario.Id,
            Estado = estadoUsuario,
            FechaRegistro = DateTime.UtcNow
        };
        var estudianteUsuario = new Usuario
        {
            Id = EstudianteId,
            Correo = "estudiante@educonnect.test",
            PasswordHash = "hash",
            RolId = rolEstudiante.Id,
            Rol = rolEstudiante,
            EstadoId = estadoUsuario.Id,
            Estado = estadoUsuario,
            FechaRegistro = DateTime.UtcNow
        };
        var tutor = new Tutor
        {
            UsuarioId = tutorUsuario.Id,
            Usuario = tutorUsuario,
            Nombre = "Tutor",
            Apellido = "Prueba",
            CarnetId = "T-10",
            NumeroIdentificacion = "1000000000001",
            Genero = "otro",
            Direccion = "Zona 1",
            Telefono = "55550000",
            FotografiaUrl = "tutor.png",
            DireccionTutoria = "USAC",
            Universidad = "USAC"
        };
        var estudiante = new Estudiante
        {
            UsuarioId = EstudianteId,
            Usuario = estudianteUsuario,
            Nombre = "Estudiante",
            Apellido = "Prueba",
            Carnet = "E-11",
            Genero = "otro",
            Direccion = "Zona 1",
            Telefono = "55551111"
        };
        var materia = new Materia { Id = 1, Nombre = "Matemática" };
        var estado = new EstadoSesion { Id = 1, Nombre = estadoSesion };
        var sesion = new Sesion
        {
            Id = SesionId,
            TutorId = tutor.UsuarioId,
            Tutor = tutor,
            EstudianteId = estudiante.UsuarioId,
            Estudiante = estudiante,
            MateriaId = materia.Id,
            Materia = materia,
            EstadoId = estado.Id,
            Estado = estado,
            FechaSesion = new DateOnly(2026, 10, 1),
            HoraInicio = new TimeOnly(10, 0),
            Motivo = "Repaso"
        };

        context.AddRange(
            rolTutor,
            rolEstudiante,
            estadoUsuario,
            tutorUsuario,
            estudianteUsuario,
            tutor,
            estudiante,
            materia,
            estado,
            sesion
        );

        if (yaCalificada)
        {
            sesion.CalificacionTutor = new CalificacionTutor
            {
                SesionId = sesion.Id,
                Sesion = sesion,
                Estrellas = 4,
                FechaCreacion = DateTime.UtcNow
            };
            context.CalificacionesTutores.Add(sesion.CalificacionTutor);
        }

        context.SaveChanges();
        return context;
    }

    private static ClaimsPrincipal CreateStudentUser(
        int estudianteId = EstudianteId,
        string role = "Estudiante") =>
        new(new ClaimsIdentity(
        [
            new Claim("id_usuario", estudianteId.ToString()),
            new Claim("rol", role)
        ], "test"));

    private static void AssertStatus(IResult result, int expectedStatus) =>
        Assert.Equal(expectedStatus, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);

    [Fact]
    public async Task HandleAsync_SesionAtendidaYCalificacionCero_GuardaCalificacion()
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(0, "  Atención deficiente  "),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        var ok = Assert.IsType<Ok<CalificarTutorResponseDto>>(result);
        Assert.Equal(0, ok.Value!.Estrellas);
        Assert.Equal("Atención deficiente", ok.Value.Comentario);

        var calificacion = await context.CalificacionesTutores.SingleAsync();
        Assert.Equal(0, calificacion.Estrellas);
        Assert.Equal("Atención deficiente", calificacion.Comentario);
    }

    [Fact]
    public async Task HandleAsync_ComentarioOpcionalVacio_GuardaNull()
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(5, "   "),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        Assert.IsType<Ok<CalificarTutorResponseDto>>(result);
        Assert.Null(await context.CalificacionesTutores.Select(item => item.Comentario).SingleAsync());
    }

    [Fact]
    public async Task HandleAsync_CalificacionNoSeleccionada_RetornaBadRequest()
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(null, null),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status400BadRequest);
        Assert.Empty(await context.CalificacionesTutores.ToListAsync());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public async Task HandleAsync_CalificacionFueraDeRango_RetornaBadRequest(int estrellas)
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(estrellas, null),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status400BadRequest);
        Assert.Empty(await context.CalificacionesTutores.ToListAsync());
    }

    [Theory]
    [InlineData("PENDIENTE")]
    [InlineData("CANCELADA_TUTOR")]
    [InlineData("CANCELADA_ESTUDIANTE")]
    public async Task HandleAsync_SesionNoAtendida_RetornaConflict(string estado)
    {
        using var context = CreateContext(estadoSesion: estado);

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(4, null),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status409Conflict);
        Assert.Empty(await context.CalificacionesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_SesionDeOtroEstudiante_RetornaForbidden()
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(4, null),
            CreateStudentUser(estudianteId: 99),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status403Forbidden);
        Assert.Empty(await context.CalificacionesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_SesionYaCalificada_RetornaConflict()
    {
        using var context = CreateContext(yaCalificada: true);

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(3, "Otra calificación"),
            CreateStudentUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status409Conflict);
        Assert.Equal(4, (await context.CalificacionesTutores.SingleAsync()).Estrellas);
    }

    [Fact]
    public async Task HandleAsync_UsuarioNoEsEstudiante_RetornaForbidden()
    {
        using var context = CreateContext();

        var result = await CalificarTutorEndpoint.HandleAsync(
            SesionId,
            new CalificarTutorRequestDto(4, null),
            CreateStudentUser(role: "Tutor"),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status403Forbidden);
        Assert.Empty(await context.CalificacionesTutores.ToListAsync());
    }
}