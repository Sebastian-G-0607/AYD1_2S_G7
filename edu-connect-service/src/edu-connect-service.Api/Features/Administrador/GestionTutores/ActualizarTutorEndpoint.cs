using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Administrador.GestionTutores;

public static class ActualizarTutorEndpoint
{
    public static void MapActualizarTutor(this IEndpointRouteBuilder app)
    {
        app.MapPut("/tutores/{id:int}", HandleAsync)
            .Produces<ActualizarTutorAdminResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public static async Task<IResult> HandleAsync(
        int id,
        [FromBody] ActualizarTutorAdminRequestDto request,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        var tutor = await dbContext.Tutores
            .Include(t => t.Usuario)
                .ThenInclude(u => u.Estado)
            .Include(t => t.TutorMaterias)
                .ThenInclude(tm => tm.Materia)
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
                detail: $"Solo se puede editar a tutores activos. El estado actual del tutor es '{tutor.Usuario.Estado.Nombre}'."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nombre obligatorio",
                detail: "El nombre del tutor es obligatorio."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Apellido obligatorio",
                detail: "El apellido del tutor es obligatorio."
            );
        }

        if (!CarnetValidator.IsValid(request.CarnetId))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet o ID debe ser numérico y contener entre 6 y 10 dígitos."
            );
        }

        if (!DpiValidator.IsValid(request.NumeroIdentificacion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Documento de identificación inválido",
                detail: "El documento de identificación / colegiado debe contener exactamente 13 dígitos numéricos."
            );
        }

        if (!GeneroValidator.TryNormalize(request.Genero, out var generoNormalizado))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Género inválido",
                detail: "El género debe ser 'masculino' ('m') o 'femenino' ('f')."
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.FechaNacimiento == default || request.FechaNacimiento > today)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento inválida",
                detail: "La fecha de nacimiento es obligatoria y no puede ser futura."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Direccion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección obligatoria",
                detail: "La dirección de residencia es obligatoria."
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Telefono) && !TelefonoValidator.IsValid(request.Telefono))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Teléfono inválido",
                detail: "El teléfono debe ser numérico y contener exactamente 8 dígitos."
            );
        }

        if (string.IsNullOrWhiteSpace(request.DireccionTutoria))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección de tutoría obligatoria",
                detail: "La dirección de tutoría (física o enlace online) es obligatoria."
            );
        }

        var currentYear = DateTime.UtcNow.Year;
        if (request.AnioInicio < 1980 || request.AnioInicio > currentYear)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Año de inicio inválido",
                detail: $"El año de inicio debe estar comprendido entre 1980 y el año actual ({currentYear})."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Universidad))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Universidad obligatoria",
                detail: "La universidad es obligatoria."
            );
        }

        var carnetNormalizado = request.CarnetId.Trim();
        var duplicateCarnet = await dbContext.Tutores
            .AnyAsync(t => t.UsuarioId != id && t.CarnetId == carnetNormalizado, cancellationToken);

        if (duplicateCarnet)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "Ya existe otro tutor registrado con el mismo carnet o ID."
            );
        }

        var idDocNormalizado = request.NumeroIdentificacion.Trim();
        var duplicateDoc = await dbContext.Tutores
            .AnyAsync(t => t.UsuarioId != id && t.NumeroIdentificacion == idDocNormalizado, cancellationToken);

        if (duplicateDoc)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Documento de identificación duplicado",
                detail: "Ya existe otro tutor registrado con el mismo número de identificación."
            );
        }

        // Sincronización de materias
        if (request.MateriasIds is not null && request.MateriasIds.Count > 0)
        {
            var validMaterias = await dbContext.Materias
                .Where(m => request.MateriasIds.Contains(m.Id))
                .ToListAsync(cancellationToken);

            tutor.TutorMaterias.Clear();
            foreach (var materia in validMaterias)
            {
                tutor.TutorMaterias.Add(new TutorMateria
                {
                    TutorId = tutor.UsuarioId,
                    MateriaId = materia.Id
                });
            }
        }
        else if (request.Materias is not null && request.Materias.Count > 0)
        {
            var requestedNames = request.Materias
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim().ToLower())
                .ToList();

            var validMaterias = await dbContext.Materias
                .Where(m => requestedNames.Contains(m.Nombre.ToLower()))
                .ToListAsync(cancellationToken);

            tutor.TutorMaterias.Clear();
            foreach (var materia in validMaterias)
            {
                tutor.TutorMaterias.Add(new TutorMateria
                {
                    TutorId = tutor.UsuarioId,
                    MateriaId = materia.Id
                });
            }
        }

        // Modificación de campos del tutor (el correo electrónico permanece intacto en tutor.Usuario.Correo)
        tutor.Nombre = request.Nombre.Trim();
        tutor.Apellido = request.Apellido.Trim();
        tutor.CarnetId = carnetNormalizado;
        tutor.NumeroIdentificacion = idDocNormalizado;
        tutor.Genero = generoNormalizado;
        tutor.FechaNacimiento = request.FechaNacimiento;
        tutor.Direccion = request.Direccion.Trim();
        tutor.Telefono = request.Telefono?.Trim() ?? string.Empty;
        tutor.DireccionTutoria = request.DireccionTutoria.Trim();
        tutor.AnioInicio = request.AnioInicio;
        tutor.Universidad = request.Universidad.Trim();

        if (!string.IsNullOrWhiteSpace(request.FotografiaUrl))
        {
            tutor.FotografiaUrl = request.FotografiaUrl.Trim();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var materiasNombres = tutor.TutorMaterias
            .Select(tm => tm.Materia?.Nombre ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        var especialidad = materiasNombres.Count > 0
            ? string.Join(", ", materiasNombres)
            : "Sin especialidad registrada";

        var response = new ActualizarTutorAdminResponseDto(
            tutor.UsuarioId,
            tutor.Nombre,
            tutor.Apellido,
            tutor.CarnetId,
            tutor.NumeroIdentificacion,
            tutor.Genero,
            tutor.FechaNacimiento,
            tutor.Usuario.Correo,
            tutor.FotografiaUrl,
            especialidad,
            materiasNombres,
            tutor.DireccionTutoria,
            tutor.AnioInicio,
            tutor.Universidad,
            tutor.Direccion,
            tutor.Telefono,
            "Tutor actualizado exitosamente por el administrador."
        );

        return Results.Ok(response);
    }
}

