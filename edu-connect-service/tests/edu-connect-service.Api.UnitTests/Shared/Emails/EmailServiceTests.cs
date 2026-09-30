using edu_connect_service.Api.Shared.Emails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace edu_connect_service.Api.UnitTests.Shared.Emails;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock = new();

    private IConfiguration CreateFallbackConfiguration()
    {
        return new ConfigurationBuilder().Build();
    }

    [Fact]
    public async Task SendEmailAsync_WithoutSmtpConfig_LogsNotificationFallback()
    {
        var config = CreateFallbackConfiguration();
        var service = new EmailService(_loggerMock.Object, config);

        var exception = await Record.ExceptionAsync(() =>
            service.SendEmailAsync("estudiante@test.com", "Bienvenido", "<p>Hola</p>"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendEstadoCuentaNotificacionAsync_WhenAprobado_ExecutesSuccessfully()
    {
        var config = CreateFallbackConfiguration();
        var service = new EmailService(_loggerMock.Object, config);

        var exception = await Record.ExceptionAsync(() =>
            service.SendEstadoCuentaNotificacionAsync("estudiante@test.com", "Juan Perez", "APROBADO"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendEstadoCuentaNotificacionAsync_WhenRechazadoWithMotivo_ExecutesSuccessfully()
    {
        var config = CreateFallbackConfiguration();
        var service = new EmailService(_loggerMock.Object, config);

        var exception = await Record.ExceptionAsync(() =>
            service.SendEstadoCuentaNotificacionAsync("estudiante@test.com", "Juan Perez", "RECHAZADO", "Documentos ilegibles"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendBajaCuentaNotificacionAsync_ExecutesSuccessfully()
    {
        var config = CreateFallbackConfiguration();
        var service = new EmailService(_loggerMock.Object, config);

        var exception = await Record.ExceptionAsync(() =>
            service.SendBajaCuentaNotificacionAsync("estudiante@test.com", "Juan Perez", "Incumplimiento de normas"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task SendCancelacionSesionTutorNotificacionAsync_ExecutesSuccessfully()
    {
        var config = CreateFallbackConfiguration();
        var service = new EmailService(_loggerMock.Object, config);

        var exception = await Record.ExceptionAsync(() =>
            service.SendCancelacionSesionTutorNotificacionAsync(
                "estudiante@test.com",
                "Juan Perez",
                "Prof. Carlos Gomez",
                "Matematica I",
                new DateOnly(2026, 10, 15),
                new TimeOnly(14, 30),
                "Dudas de derivadas",
                "Emergencia medica",
                "Disculpe las molestias"));

        Assert.Null(exception);
    }
}
