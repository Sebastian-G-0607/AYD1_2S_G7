using edu_connect_service.Api.Data;
using edu_connect_service.Api.Shared.Emails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionEstudiantes;

public static class DarBajaEstudianteEndpoint
{
    public static void MapDarBajaEstudiante(this IEndpointRouteBuilder app)
    {
        app.MapPut("/estudiantes/{id:int}/dar-baja", HandleAsync)
            .Produces<DarBajaEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPut("/estudiantes/{id:int}/baja", HandleAsync)
            .Produces<DarBajaEstudianteResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] DarBajaEstudianteRequestDto? request,
        edu_connect_serviceContext dbContext,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
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

        if (estudiante.Usuario.Estado.Nombre != "APROBADO")
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Operación no permitida",
                detail: $"Solo se puede dar de baja a usuarios activos. El estado actual del estudiante es '{estudiante.Usuario.Estado.Nombre}'."
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

        estudiante.Usuario.EstadoId = estadoInactivo.Id;
        estudiante.Usuario.Estado = estadoInactivo;
        estudiante.Usuario.FechaBaja = fechaBaja;
        estudiante.Usuario.MotivoBaja = motivo;

        await dbContext.SaveChangesAsync(cancellationToken);

        var nombreCompleto = $"{estudiante.Nombre} {estudiante.Apellido}".Trim();
        await emailService.SendBajaCuentaNotificacionAsync(
            estudiante.Usuario.Correo,
            nombreCompleto,
            motivo,
            cancellationToken
        );

        var response = new DarBajaEstudianteResponseDto(
            estudiante.UsuarioId,
            estudiante.Usuario.Correo,
            "INACTIVO",
            fechaBaja,
            motivo,
            "El estudiante ha sido dado de baja exitosamente."
        );

        return Results.Ok(response);
    }
}

