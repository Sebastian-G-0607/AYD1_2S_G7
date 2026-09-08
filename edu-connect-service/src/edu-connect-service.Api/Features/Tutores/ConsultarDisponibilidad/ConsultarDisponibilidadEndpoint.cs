using System.Security.Claims;
using edu_connect_service.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.ConsultarDisponibilidad;

public static class ConsultarDisponibilidadEndpoint
{
    private static readonly TimeSpan DuracionBloque = TimeSpan.FromHours(1);

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

        var diaSemanaConsultado = filtro.Fecha.DayOfWeek == DayOfWeek.Sunday
            ? 7
            : (int)filtro.Fecha.DayOfWeek;

        var atiendeEseDia = diasAtencion.Contains(diaSemanaConsultado);

        if (!atiendeEseDia || tutor.HoraInicio is null || tutor.HoraFin is null)
        {
            return Results.Ok(new DisponibilidadTutorResponseDto(
                tutor.UsuarioId,
                $"{tutor.Nombre} {tutor.Apellido}",
                diasAtencion,
                tutor.HoraInicio,
                tutor.HoraFin,
                filtro.Fecha,
                AtiendeEseDia: false,
                Bloques: []
            ));
        }

        var estadosQueOcupan = new[] { "PENDIENTE", "ATENDIDA" };

        var horariosOcupados = await dbContext.Sesiones
            .AsNoTracking()
            .Where(s => s.TutorId == tutorId)
            .Where(s => s.FechaSesion == filtro.Fecha)
            .Where(s => estadosQueOcupan.Contains(s.Estado.Nombre))
            .Select(s => s.HoraInicio)
            .ToListAsync(cancellationToken);

        var bloques = new List<BloqueHorarioDto>();
        var horaActual = tutor.HoraInicio.Value;

        while (horaActual.Add(DuracionBloque) <= tutor.HoraFin.Value
               || horaActual.Add(DuracionBloque) == TimeOnly.MinValue)
        {
            var horaFinBloque = horaActual.Add(DuracionBloque);

            var disponible = !horariosOcupados.Contains(horaActual);

            bloques.Add(new BloqueHorarioDto(horaActual, horaFinBloque, disponible));

            if (horaFinBloque == tutor.HoraFin.Value || horaFinBloque < horaActual)
            {
                break;
            }

            horaActual = horaFinBloque;
        }

        return Results.Ok(new DisponibilidadTutorResponseDto(
            tutor.UsuarioId,
            $"{tutor.Nombre} {tutor.Apellido}",
            diasAtencion,
            tutor.HoraInicio,
            tutor.HoraFin,
            filtro.Fecha,
            AtiendeEseDia: true,
            Bloques: bloques
        ));
    }
}