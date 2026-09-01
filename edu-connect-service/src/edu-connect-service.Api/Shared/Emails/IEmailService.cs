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
}

