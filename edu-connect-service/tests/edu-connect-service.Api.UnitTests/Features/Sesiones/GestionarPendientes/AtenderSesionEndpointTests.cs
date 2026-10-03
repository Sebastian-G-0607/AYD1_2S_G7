using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Sesiones.GestionarPendientes;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.UnitTests.Features.Sesiones.GestionarPendientes;

public class AtenderSesionEndpointTests
{
    private const int TutorId = 10;
    private const int SesionId = 20;

    private static edu_connect_serviceContext CreateContext()
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
            Id = TutorId,
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
            Id = 11,
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
            UsuarioId = TutorId,
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
            UsuarioId = estudianteUsuario.Id,
            Usuario = estudianteUsuario,
            Nombre = "Estudiante",
            Apellido = "Prueba",
            Carnet = "E-11",
            Genero = "otro",
            Direccion = "Zona 1",
            Telefono = "55551111"
        };
        var materia = new Materia { Id = 1, Nombre = "Matemática" };
        var pendiente = new EstadoSesion { Id = 1, Nombre = "PENDIENTE" };
        var atendida = new EstadoSesion { Id = 2, Nombre = "ATENDIDA" };
        var sesion = new Sesion
        {
            Id = SesionId,
            TutorId = TutorId,
            Tutor = tutor,
            EstudianteId = estudiante.UsuarioId,
            Estudiante = estudiante,
            MateriaId = materia.Id,
            Materia = materia,
            EstadoId = pendiente.Id,
            Estado = pendiente,
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
            pendiente,
            atendida,
            sesion
        );
        context.SaveChanges();
        return context;
    }

    private static ClaimsPrincipal CreateTutorUser(int tutorId = TutorId, string role = "Tutor") =>
        new(new ClaimsIdentity(
        [
            new Claim("id_usuario", tutorId.ToString()),
            new Claim("rol", role)
        ], "test"));

    private static AtenderSesionRequestDto ValidRequest() => new(
        "Dificultad con ecuaciones lineales",
        [new RecursoPlanEstudioRequestDto("Guía de ejercicios", "Documento", "Resolver ejercicios 1 al 5")]
    );

    private static void AssertStatus(IResult result, int expectedStatus) =>
        Assert.Equal(expectedStatus, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);

    [Fact]
    public async Task HandleAsync_PlanValido_GuardaPlanRecursosYMarcaSesionAtendida()
    {
        using var context = CreateContext();

        var result = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            ValidRequest(),
            CreateTutorUser(),
            context,
            CancellationToken.None);

        Assert.IsType<Ok<AtenderSesionResponseDto>>(result);
        var sesion = await context.Sesiones
            .Include(item => item.PlanEstudio)
            .ThenInclude(plan => plan!.Recursos)
            .SingleAsync(item => item.Id == SesionId);
        Assert.Equal(2, sesion.EstadoId);
        Assert.Equal("Dificultad con ecuaciones lineales", sesion.PlanEstudio!.DificultadesIdentificadas);
        var recurso = Assert.Single(sesion.PlanEstudio.Recursos);
        Assert.Equal("Guía de ejercicios", recurso.Nombre);
        Assert.Equal("Documento", recurso.Tipo);
        Assert.Equal("Resolver ejercicios 1 al 5", recurso.DescripcionUso);
    }

    [Fact]
    public async Task HandleAsync_DificultadesVacias_RetornaBadRequestSinAtender()
    {
        using var context = CreateContext();
        var request = ValidRequest() with { DificultadesIdentificadas = "  " };

        var result = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            request,
            CreateTutorUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status400BadRequest);
        Assert.Empty(await context.PlanesEstudio.ToListAsync());
        Assert.Equal(1, (await context.Sesiones.SingleAsync()).EstadoId);
    }

    [Fact]
    public async Task HandleAsync_RecursosVaciosOIncompletos_RetornaBadRequest()
    {
        using var contextSinRecursos = CreateContext();
        var requestSinRecursos = ValidRequest() with { Recursos = [] };
        var resultSinRecursos = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            requestSinRecursos,
            CreateTutorUser(),
            contextSinRecursos,
            CancellationToken.None);

        AssertStatus(resultSinRecursos, StatusCodes.Status400BadRequest);

        using var contextIncompleto = CreateContext();
        var requestIncompleto = ValidRequest() with
        {
            Recursos = [new RecursoPlanEstudioRequestDto("Libro", "", "Leer capítulo 2")]
        };
        var resultIncompleto = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            requestIncompleto,
            CreateTutorUser(),
            contextIncompleto,
            CancellationToken.None);

        AssertStatus(resultIncompleto, StatusCodes.Status400BadRequest);
        Assert.Empty(await contextIncompleto.PlanesEstudio.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_UsuarioNoEsTutorPropietario_RetornaForbiddenSinCambios()
    {
        using var context = CreateContext();

        var result = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            ValidRequest(),
            CreateTutorUser(tutorId: 99),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status403Forbidden);
        Assert.Empty(await context.PlanesEstudio.ToListAsync());
        Assert.Equal(1, (await context.Sesiones.SingleAsync()).EstadoId);
    }

    [Fact]
    public async Task HandleAsync_SesionNoPendiente_RetornaConflictSinCambios()
    {
        using var context = CreateContext();
        var sesion = await context.Sesiones.SingleAsync();
        sesion.EstadoId = 2;
        await context.SaveChangesAsync();

        var result = await AtenderSesionEndpoint.HandleAsync(
            SesionId,
            ValidRequest(),
            CreateTutorUser(),
            context,
            CancellationToken.None);

        AssertStatus(result, StatusCodes.Status409Conflict);
        Assert.Empty(await context.PlanesEstudio.ToListAsync());
    }
}