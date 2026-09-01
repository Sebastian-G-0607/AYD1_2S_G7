using System.Net;
using System.Net.Mail;

namespace edu_connect_service.Api.Shared.Emails;

public class EmailService(
    ILogger<EmailService> logger,
    IConfiguration configuration) : IEmailService
{
    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var smtpHost = configuration["Smtp:Host"] ?? configuration["SMTP_HOST"];
        var smtpPortString = configuration["Smtp:Port"] ?? configuration["SMTP_PORT"];
        var smtpUser = configuration["Smtp:User"] ?? configuration["Smtp:Username"] ?? configuration["SMTP_USER"];
        var smtpPassword = configuration["Smtp:Password"] ?? configuration["SMTP_PASSWORD"];
        var fromEmail = configuration["Smtp:From"] ?? configuration["SMTP_FROM"] ?? "no-reply@educonnect.com";
        var fromName = configuration["Smtp:FromName"] ?? "EduConnect";

        if (!string.IsNullOrWhiteSpace(smtpHost) && int.TryParse(smtpPortString, out var smtpPort))
        {
            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = bool.TryParse(configuration["Smtp:EnableSsl"] ?? configuration["SMTP_ENABLE_SSL"], out var ssl) ? ssl : true
                };

                if (!string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPassword))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                }

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                logger.LogInformation("Enviando correo a {ToEmail} con asunto: '{Subject}' a través del servidor SMTP {Host}:{Port}", toEmail, subject, smtpHost, smtpPort);
                await client.SendMailAsync(mailMessage, cancellationToken);
                logger.LogInformation("Correo enviado exitosamente a {ToEmail}", toEmail);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al enviar correo electrónico a {ToEmail} vía SMTP. Registrando en logs...", toEmail);
            }
        }

        // Fallback cuando SMTP no está configurado
        logger.LogInformation(
            "=== [NOTIFICACIÓN DE CORREO EDUCONNECT] ===\n" +
            "Para: {ToEmail}\n" +
            "Asunto: {Subject}\n" +
            "Contenido:\n{Body}\n" +
            "============================================",
            toEmail, subject, htmlBody);
    }

    public async Task SendEstadoCuentaNotificacionAsync(
        string toEmail,
        string nombreUsuario,
        string nuevoEstado,
        string? motivo = null,
        CancellationToken cancellationToken = default)
    {
        var esAprobado = string.Equals(nuevoEstado, "APROBADO", StringComparison.OrdinalIgnoreCase);
        var subject = esAprobado
            ? "¡Tu cuenta en EduConnect ha sido aprobada!"
            : "Actualización sobre tu solicitud de registro en EduConnect";

        var titulo = esAprobado ? "¡Bienvenido/a a EduConnect!" : "Estado de tu solicitud de registro";
        var mensajePrincipal = esAprobado
            ? "Nos complace informarte que tu solicitud de registro ha sido <strong style='color: #16a34a;'>APROBADA</strong>. Ya puedes iniciar sesión con tus credenciales y disfrutar de todas las funcionalidades de la plataforma."
            : "Te informamos que tu solicitud de registro ha sido <strong style='color: #dc2626;'>RECHAZADA</strong> por el equipo de administración.";

        var motivoHtml = !string.IsNullOrWhiteSpace(motivo)
            ? $"<p style='background-color: #f3f4f6; padding: 12px; border-radius: 6px; border-left: 4px solid #ef4444;'><strong>Motivo / Observaciones:</strong> {WebUtility.HtmlEncode(motivo)}</p>"
            : string.Empty;

        var htmlBody = $$"""
            <!DOCTYPE html>
            <html lang="es">
            <head>
                <meta charset="UTF-8">
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f9fafb; margin: 0; padding: 20px; }
                    .container { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; }
                    .header { background-color: #1e3a8a; padding: 24px; text-align: center; color: white; }
                    .content { padding: 24px; color: #374151; line-height: 1.6; }
                    .footer { background-color: #f3f4f6; padding: 16px; text-align: center; font-size: 12px; color: #6b7280; }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="header">
                        <h1 style="margin:0; font-size: 24px;">EduConnect</h1>
                    </div>
                    <div class="content">
                        <h2>{{titulo}}</h2>
                        <p>Hola <strong>{{WebUtility.HtmlEncode(nombreUsuario)}}</strong>,</p>
                        <p>{{mensajePrincipal}}</p>
                        {{motivoHtml}}
                        <p>Si tienes alguna pregunta o requieres asistencia adicional, por favor responde a este correo o contacta a soporte.</p>
                        <p>Atentamente,<br><strong>Equipo de EduConnect</strong></p>
                    </div>
                    <div class="footer">
                        <p>&copy; {{DateTime.UtcNow.Year}} EduConnect. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>
            """;

        await SendEmailAsync(toEmail, subject, htmlBody, cancellationToken);
    }
}

