namespace edu_connect_service.Api.Shared.Emails;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);

    Task SendEstadoCuentaNotificacionAsync(
        string toEmail,
        string nombreUsuario,
        string nuevoEstado,
        string? motivo = null,
        CancellationToken cancellationToken = default);

    Task SendBajaCuentaNotificacionAsync(
        string toEmail,
        string nombreUsuario,
        string? motivo = null,
        CancellationToken cancellationToken = default);

    Task SendCancelacionSesionTutorNotificacionAsync(
        string toEmail,
        string nombreEstudiante,
        string nombreTutor,
        string materia,
        DateOnly fecha,
        TimeOnly hora,
        string motivoOriginal,
        string motivoCancelacion,
        string? mensajeDisculpa = null,
        CancellationToken cancellationToken = default);
}

