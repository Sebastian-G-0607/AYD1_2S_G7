using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Estudiantes.ReportarTutor;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.UnitTests.Features.Estudiantes.ReportarTutor;

public class ReportarTutorEndpointTests
{
    private const int TutorId = 10;
    private const int EstudianteId = 11;
    private const int SesionAtendidaId = 20;
    private const int SesionPendienteId = 21;
    private const int SesionCanceladaId = 22;
    private const int CategoriaId = 1;

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
        var pendiente = new EstadoSesion { Id = 1, Nombre = "PENDIENTE" };
        var atendida = new EstadoSesion { Id = 2, Nombre = "ATENDIDA" };
        var cancelada = new EstadoSesion { Id = 3, Nombre = "CANCELADA_ESTUDIANTE" };

        Sesion NuevaSesion(int id, EstadoSesion estado) => new()
        {
            Id = id,
            TutorId = TutorId,
            Tutor = tutor,
            EstudianteId = EstudianteId,
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
            pendiente,
            atendida,
            cancelada,
            NuevaSesion(SesionAtendidaId, atendida),
            NuevaSesion(SesionPendienteId, pendiente),
            NuevaSesion(SesionCanceladaId, cancelada),
            new CategoriaReporteTutor { Id = CategoriaId, Nombre = "Negligencia académica" }
        );
        context.SaveChanges();
        return context;
    }

    private static ClaimsPrincipal CreateUser(int userId = EstudianteId, string role = "Estudiante") =>
        new(new ClaimsIdentity(
        [
            new Claim("id_usuario", userId.ToString()),
            new Claim("rol", role)
        ], "test"));

    private static ReportarTutorRequestDto ValidRequest() =>
        new(CategoriaId, "El tutor no se presentó a la hora acordada");

    private static void AssertStatus(IResult result, int expectedStatus) =>
        Assert.Equal(expectedStatus, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);

    [Fact]
    public async Task HandleAsync_SesionAtendidaPropia_CreaReportePendiente()
    {
        using var context = CreateContext();

        var result = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, ValidRequest(), CreateUser(), context, CancellationToken.None);

        var created = Assert.IsType<Created<ReportarTutorResponseDto>>(result);
        Assert.Equal("Negligencia académica", created.Value!.Categoria);
        var reporte = await context.ReportesTutores.SingleAsync();
        Assert.Equal(SesionAtendidaId, reporte.SesionId);
        Assert.Equal(CategoriaId, reporte.CategoriaId);
        Assert.Equal("PENDIENTE", reporte.Estado);
        Assert.Equal("El tutor no se presentó a la hora acordada", reporte.Motivo);
    }

    [Fact]
    public async Task HandleAsync_SesionNoAtendida_RetornaConflictSinCrearReporte()
    {
        using var contextPendiente = CreateContext();
        var pendiente = await ReportarTutorEndpoint.HandleAsync(
            SesionPendienteId, ValidRequest(), CreateUser(), contextPendiente, CancellationToken.None);

        AssertStatus(pendiente, StatusCodes.Status409Conflict);
        Assert.Empty(await contextPendiente.ReportesTutores.ToListAsync());

        using var contextCancelada = CreateContext();
        var cancelada = await ReportarTutorEndpoint.HandleAsync(
            SesionCanceladaId, ValidRequest(), CreateUser(), contextCancelada, CancellationToken.None);

        AssertStatus(cancelada, StatusCodes.Status409Conflict);
        Assert.Empty(await contextCancelada.ReportesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_MotivoVacioOCorto_RetornaBadRequest()
    {
        using var context = CreateContext();

        var vacio = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, new ReportarTutorRequestDto(CategoriaId, "   "), CreateUser(), context,
            CancellationToken.None);
        var corto = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, new ReportarTutorRequestDto(CategoriaId, "Malo"), CreateUser(), context,
            CancellationToken.None);

        AssertStatus(vacio, StatusCodes.Status400BadRequest);
        AssertStatus(corto, StatusCodes.Status400BadRequest);
        Assert.Empty(await context.ReportesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_CategoriaInexistenteOSinSeleccionar_RetornaBadRequest()
    {
        using var context = CreateContext();

        var sinCategoria = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, new ReportarTutorRequestDto(0, "Motivo suficientemente largo"), CreateUser(),
            context, CancellationToken.None);
        var inexistente = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, new ReportarTutorRequestDto(999, "Motivo suficientemente largo"), CreateUser(),
            context, CancellationToken.None);

        AssertStatus(sinCategoria, StatusCodes.Status400BadRequest);
        AssertStatus(inexistente, StatusCodes.Status400BadRequest);
        Assert.Empty(await context.ReportesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_SesionDeOtroEstudiante_RetornaForbidden()
    {
        using var context = CreateContext();

        var result = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, ValidRequest(), CreateUser(userId: 99), context, CancellationToken.None);

        AssertStatus(result, StatusCodes.Status403Forbidden);
        Assert.Empty(await context.ReportesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_UsuarioNoEstudiante_RetornaForbidden()
    {
        using var context = CreateContext();

        var result = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, ValidRequest(), CreateUser(TutorId, "Tutor"), context, CancellationToken.None);

        AssertStatus(result, StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task HandleAsync_SesionYaReportada_RetornaConflict()
    {
        using var context = CreateContext();
        await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, ValidRequest(), CreateUser(), context, CancellationToken.None);

        var segundo = await ReportarTutorEndpoint.HandleAsync(
            SesionAtendidaId, ValidRequest(), CreateUser(), context, CancellationToken.None);

        AssertStatus(segundo, StatusCodes.Status409Conflict);
        Assert.Single(await context.ReportesTutores.ToListAsync());
    }

    [Fact]
    public async Task HandleAsync_SesionInexistente_RetornaNotFound()
    {
        using var context = CreateContext();

        var result = await ReportarTutorEndpoint.HandleAsync(
            12345, ValidRequest(), CreateUser(), context, CancellationToken.None);

        AssertStatus(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task ListarCategoriasAsync_RetornaCategoriasOrdenadas()
    {
        using var context = CreateContext();
        context.CategoriasReportesTutores.Add(
            new CategoriaReporteTutor { Id = 2, Nombre = "Ética y profesionalismo" });
        await context.SaveChangesAsync();

        var result = await ReportarTutorEndpoint.ListarCategoriasAsync(
            CreateUser(), context, CancellationToken.None);

        var ok = Assert.IsType<Ok<List<CategoriaReporteTutorResponseDto>>>(result);
        Assert.Equal(new[] { 1, 2 }, ok.Value!.Select(c => c.Id).ToArray());
    }

    [Fact]
    public void SeedCategoriasReporteTutor_CreaAlMenosCuatroCategoriasDistintas()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new edu_connect_serviceContext(options);

        DataExtensions.SeedCategoriasReporteTutor(context);
        context.SaveChanges();

        var nombres = context.CategoriasReportesTutores.Select(c => c.Nombre).ToList();
        Assert.True(nombres.Count >= 4);
        Assert.Equal(nombres.Count, nombres.Distinct().Count());
    }
}
