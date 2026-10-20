using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador.GestionReportesUsuarios;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace edu_connect_service.Api.UnitTests.Features.Administrador.GestionReportesUsuarios;

public class GestionReportesUsuariosTests
{
    private const int TutorId = 10;
    private const int EstudianteId = 20;
    private const int ReporteTutorId = 101;
    private const int ReporteEstudianteId = 201;

    private static readonly DateOnly FechaSesion = new(2025, 4, 15);
    private static readonly DateTime FechaReporte = new(2025, 4, 16, 12, 30, 0, DateTimeKind.Utc);

    private static edu_connect_serviceContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new edu_connect_serviceContext(options);
        SeedData(context);
        return context;
    }

    private static void SeedData(edu_connect_serviceContext context)
    {
        context.Roles.AddRange(
            new Rol { Id = 1, Nombre = "Tutor" },
            new Rol { Id = 2, Nombre = "Estudiante" });
        context.EstadosUsuarios.AddRange(
            new EstadoUsuario { Id = 1, Nombre = "APROBADO" },
            new EstadoUsuario { Id = 2, Nombre = "INACTIVO" });
        context.EstadosSesiones.Add(new EstadoSesion { Id = 1, Nombre = "COMPLETADA" });
        context.Materias.Add(new Materia { Id = 1, Nombre = "Matemática" });
        context.SaveChanges();

        var rolTutor = context.Roles.Single(r => r.Id == 1);
        var rolEstudiante = context.Roles.Single(r => r.Id == 2);
        var estadoAprobado = context.EstadosUsuarios.Single(e => e.Nombre == "APROBADO");
        var tutorUsuario = new Usuario
        {
            Id = TutorId,
            Correo = "tutor@educonnect.com",
            PasswordHash = "hash",
            RolId = rolTutor.Id,
            Rol = rolTutor,
            EstadoId = estadoAprobado.Id,
            Estado = estadoAprobado,
            FechaRegistro = FechaReporte
        };
        var estudianteUsuario = new Usuario
        {
            Id = EstudianteId,
            Correo = "estudiante@educonnect.com",
            PasswordHash = "hash",
            RolId = rolEstudiante.Id,
            Rol = rolEstudiante,
            EstadoId = estadoAprobado.Id,
            Estado = estadoAprobado,
            FechaRegistro = FechaReporte
        };
        context.Usuarios.AddRange(tutorUsuario, estudianteUsuario);
        context.SaveChanges();

        var tutor = new Tutor
        {
            UsuarioId = TutorId,
            Usuario = tutorUsuario,
            Nombre = "Ana",
            Apellido = "Tutora",
            CarnetId = "T-100",
            NumeroIdentificacion = "1234567890101",
            Genero = "femenino",
            Direccion = "Zona 1",
            Telefono = "55550001",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            FotografiaUrl = "https://example.com/tutor.jpg",
            DireccionTutoria = "Edificio A",
            AnioInicio = 2010,
            Universidad = "Universidad",
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(17, 0)
        };
        var estudiante = new Estudiante
        {
            UsuarioId = EstudianteId,
            Usuario = estudianteUsuario,
            Nombre = "Luis",
            Apellido = "Estudiante",
            Carnet = "E-200",
            Genero = "masculino",
            Direccion = "Zona 2",
            Telefono = "55550002",
            FechaNacimiento = new DateOnly(2005, 2, 2)
        };
        context.Tutores.Add(tutor);
        context.Estudiantes.Add(estudiante);
        context.SaveChanges();

        var sesion = new Sesion
        {
            Id = 301,
            EstudianteId = EstudianteId,
            Estudiante = estudiante,
            TutorId = TutorId,
            Tutor = tutor,
            MateriaId = 1,
            Materia = context.Materias.Single(m => m.Id == 1),
            EstadoId = 1,
            Estado = context.EstadosSesiones.Single(e => e.Id == 1),
            FechaSesion = FechaSesion,
            HoraInicio = new TimeOnly(9, 0),
            HoraFin = new TimeOnly(10, 0),
            Motivo = "Repaso de álgebra",
            FechaCreacion = FechaReporte
        };
        context.Sesiones.Add(sesion);
        context.SaveChanges();

        var categoriaTutor = new CategoriaReporteTutor { Id = 1, Nombre = "Incumplimiento" };
        var categoriaEstudiante = new CategoriaReporteEstudiante { Id = 1, Nombre = "Conducta" };
        context.CategoriasReportesTutores.Add(categoriaTutor);
        context.CategoriasReportesEstudiantes.Add(categoriaEstudiante);
        context.SaveChanges();

        context.ReportesTutores.Add(new ReporteTutor
        {
            Id = ReporteTutorId,
            SesionId = sesion.Id,
            Sesion = sesion,
            CategoriaId = categoriaTutor.Id,
            Categoria = categoriaTutor,
            Motivo = "No asistió a la sesión",
            FechaReporte = FechaReporte,
            Estado = "PENDIENTE"
        });
        context.ReportesEstudiantes.Add(new ReporteEstudiante
        {
            Id = ReporteEstudianteId,
            SesionId = sesion.Id,
            Sesion = sesion,
            CategoriaId = categoriaEstudiante.Id,
            Categoria = categoriaEstudiante,
            Motivo = "Comportamiento inadecuado",
            FechaReporte = FechaReporte,
            Estado = "PENDIENTE"
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task ListarReportesTutores_DevuelveInformacionCompleta()
    {
        using var context = CreateInMemoryContext();

        var result = await ListarReportesTutoresEndpoint.HandleAsync(context, CancellationToken.None);

        var ok = Assert.IsType<Ok<List<ReporteTutorAdminResponseDto>>>(result);
        var reporte = Assert.Single(ok.Value!);
        Assert.Equal(ReporteTutorId, reporte.Id);
        Assert.Equal(301, reporte.SesionId);
        Assert.Equal("Incumplimiento", reporte.Categoria);
        Assert.Equal("No asistió a la sesión", reporte.Motivo);
        Assert.Equal(TutorId, reporte.TutorId);
        Assert.Equal("Ana Tutora", reporte.TutorNombreCompleto);
        Assert.Equal("tutor@educonnect.com", reporte.TutorCorreo);
        Assert.Equal(EstudianteId, reporte.EstudianteDenuncianteId);
        Assert.Equal("Luis Estudiante", reporte.EstudianteDenuncianteNombreCompleto);
        Assert.Equal("estudiante@educonnect.com", reporte.EstudianteDenuncianteCorreo);
        Assert.Equal(FechaSesion, reporte.FechaSesion);
        Assert.Equal(FechaReporte, reporte.FechaReporte);
        Assert.Equal("PENDIENTE", reporte.Estado);
    }

    [Fact]
    public async Task ListarReportesEstudiantes_DevuelveInformacionCompleta()
    {
        using var context = CreateInMemoryContext();

        var result = await ListarReportesEstudiantesEndpoint.HandleAsync(context, CancellationToken.None);

        var ok = Assert.IsType<Ok<List<ReporteEstudianteAdminResponseDto>>>(result);
        var reporte = Assert.Single(ok.Value!);
        Assert.Equal(ReporteEstudianteId, reporte.Id);
        Assert.Equal(301, reporte.SesionId);
        Assert.Equal("Conducta", reporte.Categoria);
        Assert.Equal("Comportamiento inadecuado", reporte.Motivo);
        Assert.Equal(EstudianteId, reporte.EstudianteId);
        Assert.Equal("Luis Estudiante", reporte.EstudianteNombreCompleto);
        Assert.Equal("estudiante@educonnect.com", reporte.EstudianteCorreo);
        Assert.Equal(TutorId, reporte.TutorDenuncianteId);
        Assert.Equal("Ana Tutora", reporte.TutorDenuncianteNombreCompleto);
        Assert.Equal("tutor@educonnect.com", reporte.TutorDenuncianteCorreo);
        Assert.Equal(FechaSesion, reporte.FechaSesion);
        Assert.Equal(FechaReporte, reporte.FechaReporte);
        Assert.Equal("PENDIENTE", reporte.Estado);
    }

    [Fact]
    public async Task RechazarReporteTutor_CambiaEstadoYConservaTutorAprobado()
    {
        using var context = CreateInMemoryContext();

        var result = await ResolverReporteTutorEndpoint.RechazarReporteAsync(
            ReporteTutorId, context, CancellationToken.None);

        var ok = Assert.IsType<Ok<ResolverReporteResponseDto>>(result);
        Assert.Equal("DESESTIMADO", ok.Value!.EstadoReporte);
        Assert.Equal("APROBADO", ok.Value.EstadoUsuario);
        Assert.Equal("DESESTIMADO", context.ReportesTutores.Single().Estado);
        Assert.Equal("APROBADO", context.Usuarios.Single(u => u.Id == TutorId).Estado.Nombre);
    }

    [Fact]
    public async Task RechazarReporteEstudiante_CambiaEstadoYConservaEstudianteAprobado()
    {
        using var context = CreateInMemoryContext();

        var result = await ResolverReporteEstudianteEndpoint.RechazarReporteAsync(
            ReporteEstudianteId, context, CancellationToken.None);

        var ok = Assert.IsType<Ok<ResolverReporteResponseDto>>(result);
        Assert.Equal("DESESTIMADO", ok.Value!.EstadoReporte);
        Assert.Equal("APROBADO", ok.Value.EstadoUsuario);
        Assert.Equal("DESESTIMADO", context.ReportesEstudiantes.Single().Estado);
        Assert.Equal("APROBADO", context.Usuarios.Single(u => u.Id == EstudianteId).Estado.Nombre);
    }

    [Fact]
    public async Task DarBajaTutor_PorReporteInactivaTutorResuelveReporteYEnviaCorreo()
    {
        using var context = CreateInMemoryContext();
        var emailMock = CreateEmailMock();

        var result = await ResolverReporteTutorEndpoint.DarBajaTutorAsync(
            ReporteTutorId, context, emailMock.Object, CancellationToken.None);

        var ok = Assert.IsType<Ok<ResolverReporteResponseDto>>(result);
        Assert.Equal("RESUELTO", ok.Value!.EstadoReporte);
        Assert.Equal("INACTIVO", ok.Value.EstadoUsuario);
        var usuario = context.Usuarios.Single(u => u.Id == TutorId);
        Assert.Equal("INACTIVO", usuario.Estado.Nombre);
        Assert.NotNull(usuario.FechaBaja);
        Assert.Contains($"reporte #{ReporteTutorId}", usuario.MotivoBaja);
        Assert.Equal("RESUELTO", context.ReportesTutores.Single().Estado);
        emailMock.Verify(
            e => e.SendBajaCuentaNotificacionAsync(
                "tutor@educonnect.com",
                "Ana Tutora",
                It.Is<string?>(motivo => motivo != null && motivo.Contains($"reporte #{ReporteTutorId}")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DarBajaEstudiante_PorReporteInactivaEstudianteResuelveReporteYEnviaCorreo()
    {
        using var context = CreateInMemoryContext();
        var emailMock = CreateEmailMock();

        var result = await ResolverReporteEstudianteEndpoint.DarBajaEstudianteAsync(
            ReporteEstudianteId, context, emailMock.Object, CancellationToken.None);

        var ok = Assert.IsType<Ok<ResolverReporteResponseDto>>(result);
        Assert.Equal("RESUELTO", ok.Value!.EstadoReporte);
        Assert.Equal("INACTIVO", ok.Value.EstadoUsuario);
        var usuario = context.Usuarios.Single(u => u.Id == EstudianteId);
        Assert.Equal("INACTIVO", usuario.Estado.Nombre);
        Assert.NotNull(usuario.FechaBaja);
        Assert.Contains($"reporte #{ReporteEstudianteId}", usuario.MotivoBaja);
        Assert.Equal("RESUELTO", context.ReportesEstudiantes.Single().Estado);
        emailMock.Verify(
            e => e.SendBajaCuentaNotificacionAsync(
                "estudiante@educonnect.com",
                "Luis Estudiante",
                It.Is<string?>(motivo => motivo != null && motivo.Contains($"reporte #{ReporteEstudianteId}")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Mock<IEmailService> CreateEmailMock()
    {
        var emailMock = new Mock<IEmailService>();
        emailMock
            .Setup(e => e.SendBajaCuentaNotificacionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return emailMock;
    }
}
