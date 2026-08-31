using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class ActualizarEstadoEstudianteEndpoint
{
    public static void MapActualizarEstadoEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPut("/estudiantes/{id:int}/estado", HandleAsync)
            .Produces<ActualizarEstadoEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] ActualizarEstadoEstudianteRequestDto request,
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

        var estudiante = await dbContext.Estudiantes
            .Include(e => e.Usuario)
            .ThenInclude(u => u.Estado)
            .FirstOrDefaultAsync(e => e.UsuarioId == id, cancellationToken);

        if (estudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: $"No se encontró un estudiante con ID {id}."
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

        estudiante.Usuario.EstadoId = estadoEntidad.Id;
        estudiante.Usuario.Estado = estadoEntidad;

        if (nuevoEstado == "RECHAZADO")
        {
            estudiante.Usuario.FechaBaja = DateTime.UtcNow;
            estudiante.Usuario.MotivoBaja = string.IsNullOrWhiteSpace(request.Motivo)
                ? "Solicitud rechazada por el administrador"
                : request.Motivo.Trim();
        }
        else
        {
            estudiante.Usuario.FechaBaja = null;
            estudiante.Usuario.MotivoBaja = null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreCompleto = $"{estudiante.Nombre} {estudiante.Apellido}".Trim();
        await emailService.SendEstadoCuentaNotificacionAsync(
            estudiante.Usuario.Correo,
            nombreCompleto,
            nuevoEstado,
            request.Motivo,
            cancellationToken
        );

        var response = new ActualizarEstadoEstudianteResponseDto(
            estudiante.UsuarioId,
            estudiante.Usuario.Correo,
            nuevoEstado,
            $"El estudiante ha sido {nuevoEstado.ToLowerInvariant()} exitosamente."
        );

        return Results.Ok(response);
    }
}

