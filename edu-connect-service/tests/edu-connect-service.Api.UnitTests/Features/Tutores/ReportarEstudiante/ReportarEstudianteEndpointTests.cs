using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Tutores.ReportarEstudiante;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.UnitTests.Features.Tutores.ReportarEstudiante;

public class ReportarEstudianteEndpointTests
{
    private const int TutorId = 10;
    private const int SesionId = 20;
    private const int CategoriaId = 30;

    private static edu_connect_serviceContext CreateContext(string estadoSesion = "ATENDIDA")
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
        var estado = new EstadoSesion { Id = 1, Nombre = estadoSesion };
        var categoria = new CategoriaReporteEstudiante { Id = CategoriaId, Nombre = "Conducta inapropiada" };
        var sesion = new Sesion
        {
            Id = SesionId,
            TutorId = TutorId,
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
            rolTutor, rolEstudiante, estadoUsuario,
            tutorUsuario, estudianteUsuario, tutor, estudiante,
            materia, estado, categoria, sesion
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

    private static void AssertStatus(IResult result, int expectedStatus) =>
        Assert.Equal(expectedStatus, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);

    [Fact]
    public async Task HandleAsync_SesionAtendidaYDatosValidos_GuardaReportePendiente()
    {
        using var context = CreateContext();
        var request = new ReportarEstudianteRequest(CategoriaId, "El estudiante llegó tarde reiteradamente");

        var result = await ReportarEstudianteEndpoint.HandleAsync(
            SesionId, request, CreateTutorUser(), context, CancellationToken.None);

        Assert.IsType<Ok<ReportarEstudianteResponse>>(result);
        var reporte = await context.ReportesEstudiantes.SingleAsync();
        Assert.Equal("PENDIENTE", reporte.Estado);
        Assert.Equal("El estudiante llegó tarde reiteradamente", reporte.Motivo);
    }

    [Fact]
    public async Task HandleAsync_SesionNoAtendida_RetornaConflictSinGuardar()
    {
        using var context = CreateContext(estadoSesion: "PENDIENTE");
        var request = new ReportarEstudianteRequest(CategoriaId, "Motivo de prueba");

        var result = await ReportarEstudianteEndpoint.HandleAsync(
            SesionId, request, CreateTutorUser(), context, CancellationToken.None);

        AssertStatus(result, StatusCodes.Status409Conflict);
        Assert.Empty(await context.ReportesEstudiantes.ToListAsync());
    }
}