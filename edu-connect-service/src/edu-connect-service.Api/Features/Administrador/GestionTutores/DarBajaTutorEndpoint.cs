using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class DarBajaTutorEndpoint
{
    public static void MapDarBajaTutor(this IEndpointRouteBuilder app)
    {
        app.MapPut("/tutores/{id:int}/dar-baja", HandleAsync)
            .Produces<DarBajaTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPut("/tutores/{id:int}/baja", HandleAsync)
            .Produces<DarBajaTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] DarBajaTutorRequestDto? request,
        edu_connect_serviceContext dbContext,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
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

        if (tutor.Usuario.Estado.Nombre != "APROBADO")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Operación no permitida",
                detail: $"Solo se puede dar de baja a usuarios activos. El estado actual del tutor es '{tutor.Usuario.Estado.Nombre}'."
            );
        }

        var estadoInactivo = await dbContext.EstadosUsuarios
            .FirstOrDefaultAsync(e => e.Nombre == "INACTIVO", cancellationToken);

        if (estadoInactivo is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "El estado 'INACTIVO' no está configurado en el sistema."
            );
        }

        var motivo = string.IsNullOrWhiteSpace(request?.Motivo)
            ? "Cuenta dada de baja por el administrador"
            : request.Motivo.Trim();

        var fechaBaja = DateTime.UtcNow;

        tutor.Usuario.EstadoId = estadoInactivo.Id;
        tutor.Usuario.Estado = estadoInactivo;
        tutor.Usuario.FechaBaja = fechaBaja;
        tutor.Usuario.MotivoBaja = motivo;

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreCompleto = $"{tutor.Nombre} {tutor.Apellido}".Trim();
        await emailService.SendBajaCuentaNotificacionAsync(
            tutor.Usuario.Correo,
            nombreCompleto,
            motivo,
            cancellationToken
        );

        var response = new DarBajaTutorResponseDto(
            tutor.UsuarioId,
            tutor.Usuario.Correo,
            "INACTIVO",
            fechaBaja,
            motivo,
            "El tutor ha sido dado de baja exitosamente."
        );

        return Results.Ok(response);
    }
}

