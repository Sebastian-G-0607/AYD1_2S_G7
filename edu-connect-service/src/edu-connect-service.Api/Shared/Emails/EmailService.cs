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

    public async Task SendBajaCuentaNotificacionAsync(
        string toEmail,
        string nombreUsuario,
        string? motivo = null,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Notificación importante: Tu cuenta en EduConnect ha sido dada de baja";
        const string titulo = "Baja de cuenta en EduConnect";
        const string mensajePrincipal = "Te informamos que tu cuenta en la plataforma EduConnect ha sido dada de baja por el equipo de administración y su estado ha pasado a <strong style='color: #dc2626;'>INACTIVO</strong>.";

        var motivoHtml = !string.IsNullOrWhiteSpace(motivo)
            ? $"<p style='background-color: #fef2f2; padding: 12px; border-radius: 6px; border-left: 4px solid #ef4444;'><strong>Motivo de la baja:</strong> {WebUtility.HtmlEncode(motivo)}</p>"
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
                        <p>Estimado/a <strong>{{WebUtility.HtmlEncode(nombreUsuario)}}</strong>,</p>
                        <p>{{mensajePrincipal}}</p>
                        {{motivoHtml}}
                        <p>A partir de este momento, el acceso a las funcionalidades de tu cuenta queda deshabilitado y no podrás iniciar sesión en el sistema.</p>
                        <p>Si consideras que esto se debe a un error o necesitas más información, por favor comunícate con la administración de la plataforma.</p>
                        <p>Atentamente,<br><strong>Equipo de Administración de EduConnect</strong></p>
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

    public async Task SendCancelacionSesionTutorNotificacionAsync(
        string toEmail,
        string nombreEstudiante,
        string nombreTutor,
        string materia,
        DateOnly fecha,
        TimeOnly hora,
        string motivoOriginal,
        string motivoCancelacion,
        string? mensajeDisculpa = null,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Cancelación de tutoría: {materia} con {nombreTutor}";
        var disculpa = string.IsNullOrWhiteSpace(mensajeDisculpa)
            ? "Lamentamos profundamente los inconvenientes que esta cancelación imprevista pueda ocasionarte en tu planificación académica. Te invitamos a consultar la disponibilidad de la plataforma para reprogramar una nueva sesión."
            : mensajeDisculpa.Trim();

        var htmlBody = $$"""
            <!DOCTYPE html>
            <html lang="es">
            <head>
                <meta charset="UTF-8">
                <style>
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f9fafb; margin: 0; padding: 20px; }
                    .container { max-width: 600px; margin: 0 auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; }
                    .header { background-color: #dc2626; padding: 24px; text-align: center; color: white; }
                    .content { padding: 24px; color: #374151; line-height: 1.6; }
                    .session-details { background-color: #f3f4f6; padding: 16px; border-radius: 6px; margin: 16px 0; }
                    .session-details table { width: 100%; border-collapse: collapse; }
                    .session-details td { padding: 6px 0; vertical-align: top; }
                    .session-details td.label { font-weight: bold; width: 40%; color: #4b5563; }
                    .apology-box { background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 14px; border-radius: 4px; margin: 16px 0; font-style: italic; color: #991b1b; }
                    .footer { background-color: #f3f4f6; padding: 16px; text-align: center; font-size: 12px; color: #6b7280; }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="header">
                        <h1 style="margin:0; font-size: 24px;">EduConnect</h1>
                        <p style="margin: 4px 0 0 0; font-size: 14px; opacity: 0.9;">Notificación de Cancelación de Sesión</p>
                    </div>
                    <div class="content">
                        <h2>Sesión de Tutoría Cancelada</h2>
                        <p>Hola <strong>{{WebUtility.HtmlEncode(nombreEstudiante)}}</strong>,</p>
                        <p>Te informamos que tu tutor <strong>{{WebUtility.HtmlEncode(nombreTutor)}}</strong> ha cancelado la siguiente sesión de tutoría debido a un inconveniente:</p>

                        <div class="session-details">
                            <table>
                                <tr>
                                    <td class="label">Materia:</td>
                                    <td><strong>{{WebUtility.HtmlEncode(materia)}}</strong></td>
                                </tr>
                                <tr>
                                    <td class="label">Tutor:</td>
                                    <td>{{WebUtility.HtmlEncode(nombreTutor)}}</td>
                                </tr>
                                <tr>
                                    <td class="label">Fecha:</td>
                                    <td>{{fecha.ToString("dd/MM/yyyy")}}</td>
                                </tr>
                                <tr>
                                    <td class="label">Hora:</td>
                                    <td>{{hora.ToString("hh:mm tt")}}</td>
                                </tr>
                                <tr>
                                    <td class="label">Motivo de la sesión:</td>
                                    <td>{{WebUtility.HtmlEncode(motivoOriginal)}}</td>
                                </tr>
                                <tr>
                                    <td class="label">Motivo de cancelación:</td>
                                    <td><strong style="color: #dc2626;">{{WebUtility.HtmlEncode(motivoCancelacion)}}</strong></td>
                                </tr>
                            </table>
                        </div>

                        <div class="apology-box">
                            <p style="margin: 0;"><strong>Mensaje:</strong></p>
                            <p style="margin: 6px 0 0 0;">{{WebUtility.HtmlEncode(disculpa)}}</p>
                        </div>

                        <p>El horario ha sido liberado en el sistema y puedes ingresar a la plataforma cuando desees para agendar una nueva tutoría.</p>
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

