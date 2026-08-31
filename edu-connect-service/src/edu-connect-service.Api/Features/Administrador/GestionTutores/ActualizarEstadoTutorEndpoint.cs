using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class ActualizarEstadoTutorEndpoint
{
    public static void MapActualizarEstadoTutor(this IEndpointRouteBuilder app)
    {
        app.MapPut("/tutores/{id:int}/estado", HandleAsync)
            .Produces<ActualizarEstadoTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] ActualizarEstadoTutorRequestDto request,
        edu_connect_serviceContext dbContext,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Estado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Estado requerido",
                detail: "Debe especificar un estado para actualizar la solicitud."
            );
        }

        var nuevoEstado = request.Estado.Trim().ToUpperInvariant();
        if (nuevoEstado is not ("APROBADO" or "RECHAZADO"))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Estado inválido",
                detail: "El estado debe ser 'APROBADO' o 'RECHAZADO'."
            );
        }

        var tutor = await dbContext.Tutores
            .Include(t => t.Usuario)
            .ThenInclude(u => u.Estado)
            .FirstOrDefaultAsync(t => t.UsuarioId == id, cancellationToken);

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: $"No se encontró un tutor con ID {id}."
            );
        }

        var estadoEntidad = await dbContext.EstadosUsuarios
            .FirstOrDefaultAsync(e => e.Nombre == nuevoEstado, cancellationToken);

        if (estadoEntidad is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: $"El estado '{nuevoEstado}' no está configurado en el sistema."
            );
        }

        tutor.Usuario.EstadoId = estadoEntidad.Id;
        tutor.Usuario.Estado = estadoEntidad;

        if (nuevoEstado == "RECHAZADO")
        {
            tutor.Usuario.FechaBaja = DateTime.UtcNow;
            tutor.Usuario.MotivoBaja = string.IsNullOrWhiteSpace(request.Motivo)
                ? "Solicitud rechazada por el administrador"
                : request.Motivo.Trim();
        }
        else
        {
            tutor.Usuario.FechaBaja = null;
            tutor.Usuario.MotivoBaja = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreCompleto = $"{tutor.Nombre} {tutor.Apellido}".Trim();
        await emailService.SendEstadoCuentaNotificacionAsync(
            tutor.Usuario.Correo,
            nombreCompleto,
            nuevoEstado,
            request.Motivo,
            cancellationToken
        );

        var response = new ActualizarEstadoTutorResponseDto(
            tutor.UsuarioId,
            tutor.Usuario.Correo,
            nuevoEstado,
            $"El tutor ha sido {nuevoEstado.ToLowerInvariant()} exitosamente."
        );

        return Results.Ok(response);
    }
}

