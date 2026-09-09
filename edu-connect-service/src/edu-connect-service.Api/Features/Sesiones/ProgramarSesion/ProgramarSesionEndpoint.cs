using System.Security.Claims;
using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Sesiones.ProgramarSesion;

public static class ProgramarSesionEndpoint
{
    private static readonly TimeSpan DuracionSesion = TimeSpan.FromHours(1);

    public static void MapProgramarSesion(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .RequireAuthorization()
            .Accepts<ProgramarSesionRequestDto>("application/json")
            .Produces<ProgramarSesionResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> HandleAsync(
        ProgramarSesionRequestDto request,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var idUsuarioClaim =
            user.FindFirstValue("id_usuario")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idUsuarioClaim, out var idUsuario))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Usuario no autenticado",
                detail: "No fue posible identificar al usuario autenticado."
            );
        }

        var rol =
            user.FindFirstValue("rol")
            ?? user.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(rol, "Estudiante", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden programar sesiones de tutoría."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Motivo requerido",
                detail: "Debe indicar el motivo de la sesión."
            );
        }

        if (request.FechaSesion < DateOnly.FromDateTime(DateTime.Today))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha inválida",
                detail: "No se puede programar una sesión en una fecha pasada."
            );
        }

        var estudiante = await dbContext.Estudiantes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                estudiante => estudiante.UsuarioId == idUsuario,
                cancellationToken
            );

        if (estudiante is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Estudiante no encontrado",
                detail: "No existe información de estudiante asociada al usuario autenticado."
            );
        }

        var tutor = await dbContext.Tutores
            .AsNoTracking()
            .FirstOrDefaultAsync(
                tutor => tutor.UsuarioId == request.TutorId,
                cancellationToken
            );

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: "El tutor seleccionado no existe."
            );
        }

        var tutorImparteMateria = await dbContext.TutoresMaterias
            .AsNoTracking()
            .AnyAsync(
                tutorMateria =>
                    tutorMateria.TutorId == request.TutorId &&
                    tutorMateria.MateriaId == request.MateriaId,
                cancellationToken
            );

        if (!tutorImparteMateria)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Materia inválida",
                detail: "La materia seleccionada no es impartida por el tutor."
            );
        }

        if (!tutor.HoraInicio.HasValue || !tutor.HoraFin.HasValue)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Tutor sin horario configurado",
                detail: "El tutor seleccionado todavía no ha configurado su horario de atención."
            );
        }

        var diaSemana = request.FechaSesion.DayOfWeek == DayOfWeek.Sunday
            ? 7
            : (int)request.FechaSesion.DayOfWeek;

        var atiendeEseDia = await dbContext.TutoresDiasAtencion
            .AsNoTracking()
            .AnyAsync(
                dia =>
                    dia.TutorId == request.TutorId &&
                    dia.DiaSemana == diaSemana,
                cancellationToken
            );

        if (!atiendeEseDia)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Tutor no disponible",
                detail: "El tutor no atiende en la fecha seleccionada."
            );
        }

        // La duración utilizada por los bloques de disponibilidad del sistema es de 1 hora.
        var horaFinSesion = request.HoraInicio.Add(DuracionSesion);

        if (request.HoraInicio < tutor.HoraInicio.Value ||
            request.HoraInicio >= tutor.HoraFin.Value)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horario fuera del rango de atención",
                detail: "La hora seleccionada se encuentra fuera del horario de atención del tutor."
            );
        }

        // También se valida que la sesión completa termine dentro del horario del tutor.
        if (horaFinSesion > tutor.HoraFin.Value)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horario fuera del rango de atención",
                detail: "La sesión debe finalizar dentro del horario de atención del tutor."
            );
        }

        var estadoPendiente = await dbContext.EstadosSesiones
            .FirstOrDefaultAsync(
                estado => estado.Nombre == "PENDIENTE",
                cancellationToken
            );

        if (estadoPendiente is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Estado de sesión no configurado",
                detail: "No se encontró el estado PENDIENTE en el sistema."
            );
        }

        // El enunciado no permite más de una sesión activa con el mismo tutor.
        var sesionActivaMismoTutor = await dbContext.Sesiones
            .AsNoTracking()
            .AnyAsync(
                sesion =>
                    sesion.EstudianteId == idUsuario &&
                    sesion.TutorId == request.TutorId &&
                    sesion.EstadoId == estadoPendiente.Id,
                cancellationToken
            );

        if (sesionActivaMismoTutor)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Sesión activa existente",
                detail: "Ya tiene una sesión activa programada con este tutor."
            );
        }

        /*
         * Validamos traslapes con las sesiones del tutor.
         *
         * Algunas sesiones antiguas pueden tener HoraFin = null porque fueron
         * creadas antes de esta corrección. Para no dejar esos horarios libres
         * incorrectamente, se consideran también como sesiones de una hora.
         */
        var sesionesTutor = await dbContext.Sesiones
            .AsNoTracking()
            .Where(sesion =>
                sesion.TutorId == request.TutorId &&
                sesion.FechaSesion == request.FechaSesion &&
                sesion.EstadoId == estadoPendiente.Id)
            .ToListAsync(cancellationToken);

        var tutorOcupado = sesionesTutor.Any(sesion =>
        {
            var horaFinExistente =
                sesion.HoraFin ?? sesion.HoraInicio.Add(DuracionSesion);

            return request.HoraInicio < horaFinExistente &&
                   horaFinSesion > sesion.HoraInicio;
        });

        if (tutorOcupado)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horario no disponible",
                detail: "El tutor ya tiene una sesión programada que se traslapa con el horario seleccionado."
            );
        }

        /*
         * Validamos también traslapes del estudiante con cualquier otro tutor.
         * El estudiante sí puede tener varias sesiones activas con tutores distintos,
         * pero nunca en horarios que se traslapen.
         */
        var sesionesEstudiante = await dbContext.Sesiones
            .AsNoTracking()
            .Where(sesion =>
                sesion.EstudianteId == idUsuario &&
                sesion.FechaSesion == request.FechaSesion &&
                sesion.EstadoId == estadoPendiente.Id)
            .ToListAsync(cancellationToken);

        var estudianteOcupado = sesionesEstudiante.Any(sesion =>
        {
            var horaFinExistente =
                sesion.HoraFin ?? sesion.HoraInicio.Add(DuracionSesion);

            return request.HoraInicio < horaFinExistente &&
                   horaFinSesion > sesion.HoraInicio;
        });

        if (estudianteOcupado)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflicto de horario",
                detail: "Ya tiene otra sesión programada que se traslapa con el horario seleccionado."
            );
        }

        var materia = await dbContext.Materias
            .AsNoTracking()
            .FirstOrDefaultAsync(
                materia => materia.Id == request.MateriaId,
                cancellationToken
            );

        if (materia is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Materia no encontrada",
                detail: "La materia seleccionada no existe."
            );
        }

        var sesion = new Sesion
        {
            EstudianteId = idUsuario,
            TutorId = request.TutorId,
            MateriaId = request.MateriaId,
            EstadoId = estadoPendiente.Id,
            FechaSesion = request.FechaSesion,
            HoraInicio = request.HoraInicio,

            // Antes se guardaba null. Ahora la sesión completa dura 1 hora.
            HoraFin = horaFinSesion,

            Motivo = request.Motivo.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.Sesiones.Add(sesion);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ProgramarSesionResponseDto(
            sesion.Id,
            sesion.EstudianteId,
            sesion.TutorId,
            sesion.MateriaId,
            materia.Nombre,
            sesion.FechaSesion,
            sesion.HoraInicio,
            sesion.HoraFin,
            sesion.Motivo,
            estadoPendiente.Nombre
        );

        return Results.Created(
            $"/api/sesiones/{sesion.Id}",
            response
        );
    }
}