using System.Globalization;
using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;

public static class ConsultarDisponibilidadEndpoint
{
    private static readonly TimeSpan DuracionBloque = TimeSpan.FromHours(1);
    private static readonly TimeSpan IntervaloPaso = TimeSpan.FromMinutes(30);

    public static void MapConsultarDisponibilidad(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{tutorId:int}/disponibilidad", HandleAsync)
            .RequireAuthorization()
            .Produces<DisponibilidadTutorResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        int tutorId,
        [AsParameters] ConsultarDisponibilidadRequestDto filtro,
        ClaimsPrincipal user,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var rol =
            user.FindFirstValue("rol")
            ?? user.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(rol, "Estudiante", StringComparison.OrdinalIgnoreCase))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Acceso denegado",
                detail: "Solo los estudiantes pueden consultar la disponibilidad de un tutor."
            );
        }

        var tutor = await dbContext.Tutores
            .AsNoTracking()
            .Include(t => t.DiasAtencion)
            .FirstOrDefaultAsync(t => t.UsuarioId == tutorId, cancellationToken);

        if (tutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Tutor no encontrado",
                detail: $"No existe un tutor con id {tutorId}."
            );
        }

        var diasAtencion = tutor.DiasAtencion
            .Select(d => d.DiaSemana)
            .OrderBy(d => d)
            .ToList();

        DateOnly fechaConsulta;
        var rawFecha = filtro.Fecha;

        if (!string.IsNullOrWhiteSpace(rawFecha) &&
            (DateOnly.TryParseExact(rawFecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dExact)
             || DateOnly.TryParse(rawFecha, CultureInfo.InvariantCulture, out dExact)
             || DateOnly.TryParse(rawFecha, out dExact)))
        {
            fechaConsulta = dExact;
        }
        else
        {
            fechaConsulta = DateOnly.FromDateTime(DateTime.Today);
        }

        var diaSemanaConsultado = fechaConsulta.DayOfWeek == DayOfWeek.Sunday
            ? 7
            : (int)fechaConsulta.DayOfWeek;

        var atiendeEseDia = diasAtencion.Contains(diaSemanaConsultado);

        if (!atiendeEseDia || tutor.HoraInicio is null || tutor.HoraFin is null)
        {
            return Results.Ok(new DisponibilidadTutorResponseDto(
                tutor.UsuarioId,
                $"{tutor.Nombre} {tutor.Apellido}",
                diasAtencion,
                tutor.HoraInicio,
                tutor.HoraFin,
                fechaConsulta,
                AtiendeEseDia: false,
                Bloques: [],
                SesionesOcupadas: []
            ));
        }

        var estadosOcupadosIds = await dbContext.EstadosSesiones
            .AsNoTracking()
            .Where(e => e.Nombre.ToUpper() == "PENDIENTE" || e.Nombre.ToUpper() == "ATENDIDA")
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var sesionesQuery = dbContext.Sesiones
            .AsNoTracking()
            .Where(s => s.TutorId == tutorId && s.FechaSesion == fechaConsulta);

        if (estadosOcupadosIds.Count > 0)
        {
            sesionesQuery = sesionesQuery.Where(s => estadosOcupadosIds.Contains(s.EstadoId));
        }
        else
        {
            sesionesQuery = sesionesQuery.Where(s => !s.Estado.Nombre.ToUpper().Contains("CANCEL"));
        }

        var sesionesOcupadasDb = await sesionesQuery
            .Select(s => new { s.HoraInicio, s.HoraFin })
            .OrderBy(s => s.HoraInicio)
            .ToListAsync(cancellationToken);

        var sesionesOcupadas = sesionesOcupadasDb
            .Select(s => new SesionOcupadaDto(
                s.HoraInicio,
                s.HoraFin ?? s.HoraInicio.Add(DuracionBloque)
            ))
            .ToList();

        var bloques = new List<BloqueHorarioDto>();
        var horaActual = tutor.HoraInicio.Value;

        while (horaActual.Add(DuracionBloque) <= tutor.HoraFin.Value
               || horaActual.Add(DuracionBloque) == TimeOnly.MinValue)
        {
            var horaFinBloque = horaActual.Add(DuracionBloque);

            var ocupado = sesionesOcupadas.Any(s =>
                horaActual < s.HoraFin && horaFinBloque > s.HoraInicio
            );

            var disponible = !ocupado;

            bloques.Add(new BloqueHorarioDto(horaActual, horaFinBloque, disponible));

            if (horaFinBloque == tutor.HoraFin.Value || horaFinBloque < horaActual)
            {
                break;
            }

            horaActual = horaActual.Add(IntervaloPaso);
        }

        return Results.Ok(new DisponibilidadTutorResponseDto(
            tutor.UsuarioId,
            $"{tutor.Nombre} {tutor.Apellido}",
            diasAtencion,
            tutor.HoraInicio,
            tutor.HoraFin,
            fechaConsulta,
            AtiendeEseDia: true,
            Bloques: bloques,
            SesionesOcupadas: sesionesOcupadas
        ));
    }
}