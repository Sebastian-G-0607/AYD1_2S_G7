using edu_connect_service.Api.Data;
using edu_connect_service.Api.Models;
using edu_connect_service.Api.Shared.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.Features.Tutores.RegistrarTutor;

public static class RegistrarTutorEndpoint
{
    public static void MapRegistrarTutor(this IEndpointRouteBuilder app)
    {
        app.MapPost("/registro", HandleAsync)
            .DisableAntiforgery()
            .Accepts<RegistrarTutorRequestDto>("multipart/form-data")
            .Produces<TutorResponseDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("/registros", HandleAsync)
            .DisableAntiforgery()
            .Accepts<RegistrarTutorRequestDto>("multipart/form-data")
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] RegistrarTutorRequestDto request,
        edu_connect_serviceContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Nombre obligatorio",
                detail: "El nombre es obligatorio y no puede estar vacío."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Apellido))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Apellido obligatorio",
                detail: "El apellido es obligatorio y no puede estar vacío."
            );
        }

        var carnet = !string.IsNullOrWhiteSpace(request.CarnetId) ? request.CarnetId.Trim() : (request.Carnet?.Trim() ?? string.Empty);
        if (!CarnetValidator.IsValid(carnet))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Carnet inválido",
                detail: "El carnet debe ser numérico y contener entre 6 y 10 dígitos."
            );
        }

        var dpi = !string.IsNullOrWhiteSpace(request.NumeroIdentificacion) ? request.NumeroIdentificacion.Trim() : (request.Dpi?.Trim() ?? string.Empty);
        if (!DpiValidator.IsValid(dpi))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Documento de identificación inválido",
                detail: "El DPI / Documento de identificación debe ser numérico y contener exactamente 13 dígitos."
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

        if (!TelefonoValidator.IsValid(request.Telefono))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Teléfono inválido",
                detail: "El teléfono debe ser numérico y contener exactamente 8 dígitos."
            );
        }

        if (request.FechaNacimiento == default)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento obligatoria",
                detail: "La fecha de nacimiento es obligatoria y debe tener formato YYYY-MM-DD."
            );
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.FechaNacimiento > today)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fecha de nacimiento inválida",
                detail: "La fecha de nacimiento no puede ser una fecha futura."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Direccion))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección obligatoria",
                detail: "La dirección de residencia es obligatoria y no puede estar vacía."
            );
        }

        if (request.Fotografia is null || request.Fotografia.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fotografía obligatoria",
                detail: "La fotografía de perfil es obligatoria para tutores."
            );
        }

        if (!ImageFileValidator.IsValidImage(request.Fotografia))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fotografía inválida",
                detail: "El archivo de fotografía debe ser una imagen válida (.jpg, .jpeg, .png, .webp)."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Universidad))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Universidad obligatoria",
                detail: "La universidad es obligatoria y no puede estar vacía."
            );
        }

        var currentYear = DateTime.UtcNow.Year;
        if (request.AnioInicio < 1980 || request.AnioInicio > currentYear)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Año de inicio inválido",
                detail: $"El año de inicio debe ser un año numérico de 4 dígitos entre 1980 y el año actual ({currentYear})."
            );
        }

        if (string.IsNullOrWhiteSpace(request.DireccionTutoria))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dirección de tutoría obligatoria",
                detail: "La dirección de tutoría (salón o edificio) es obligatoria y no puede estar vacía."
            );
        }

        var materiasIds = (request.MateriasIds.Count > 0 ? request.MateriasIds : request.Materias) ?? [];
        if (materiasIds.Count == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Materias obligatorias",
                detail: "Debe seleccionar al menos una materia como especialidad."
            );
        }

        var existingMateriaIds = await dbContext.Materias
            .Where(m => materiasIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var missingMaterias = materiasIds.Except(existingMateriaIds).ToList();
        if (missingMaterias.Count > 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Materias inexistentes",
                detail: $"Las siguientes materias no existen en el sistema: {string.Join(", ", missingMaterias)}"
            );
        }

        if (request.DiasAtencion is not null && request.DiasAtencion.Count > 0 && request.DiasAtencion.Any(d => d < 1 || d > 7))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Día de atención inválido",
                detail: "Los días de atención deben estar entre 1 (Lunes) y 7 (Domingo)."
            );
        }

        if (request.HoraInicio.HasValue && request.HoraFin.HasValue && request.HoraFin.Value <= request.HoraInicio.Value)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Rango de horario inválido",
                detail: "La hora de fin debe ser posterior a la hora de inicio."
            );
        }

        if (!EmailValidator.IsValid(request.Correo))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Correo inválido",
                detail: "El formato del correo electrónico no es válido."
            );
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña obligatoria",
                detail: "La contraseña es obligatoria."
            );
        }

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword) || request.Password != request.ConfirmPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseñas no coinciden",
                detail: "La contraseña y la confirmación de contraseña no coinciden exactamente."
            );
        }

        if (!PasswordValidator.IsValid(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña inválida",
                detail: "La contraseña debe tener un mínimo de 8 caracteres, al menos 1 letra mayúscula, 1 letra minúscula y 1 número."
            );
        }

        var normalizedEmail = request.Correo.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Usuarios.AnyAsync(u => u.Correo == normalizedEmail, cancellationToken);
        if (emailExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Correo duplicado",
                detail: "El correo electrónico ya se encuentra registrado en el sistema."
            );
        }

        var carnetExists = await dbContext.Tutores.AnyAsync(t => t.CarnetId == carnet, cancellationToken);
        if (carnetExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "El carnet universitario ya se encuentra registrado."
            );
        }

        var identificationExists = await dbContext.Tutores.AnyAsync(t => t.NumeroIdentificacion == dpi, cancellationToken);
        if (identificationExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Número de identificación duplicado",
                detail: "El número de identificación del tutor ya se encuentra registrado."
            );
        }

        var rolTutor = await dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == "Tutor", cancellationToken);
        if (rolTutor is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "El rol 'Tutor' no está configurado en el sistema."
            );
        }

        var estadoPendiente = await dbContext.EstadosUsuarios.FirstOrDefaultAsync(e => e.Nombre == "PENDIENTE", cancellationToken)
            ?? await dbContext.EstadosUsuarios.FirstOrDefaultAsync(e => e.Nombre == "APROBADO", cancellationToken);

        if (estadoPendiente is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración incompleta",
                detail: "No se encontró un estado de usuario configurado en el sistema."
            );
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var usuario = new Usuario
        {
            Correo = normalizedEmail,
            PasswordHash = passwordHash,
            RolId = rolTutor.Id,
            EstadoId = estadoPendiente.Id,
            FechaRegistro = DateTime.UtcNow
        };

        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Implementar lógica de guardado en almacenamiento de objetos (S3 / Oracle Object Storage).
        var fotografiaUrl = $"/uploads/tutores/{Guid.NewGuid():N}.jpg";

        var tutor = new Tutor
        {
            UsuarioId = usuario.Id,
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            CarnetId = carnet,
            NumeroIdentificacion = dpi,
            Genero = generoNormalizado,
            Direccion = request.Direccion.Trim(),
            Telefono = request.Telefono.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            FotografiaUrl = fotografiaUrl,
            DireccionTutoria = request.DireccionTutoria.Trim(),
            AnioInicio = request.AnioInicio,
            Universidad = request.Universidad.Trim(),
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        };

        dbContext.Tutores.Add(tutor);

        var distinctMateriaIds = materiasIds.Distinct().ToList();
        foreach (var materiaId in distinctMateriaIds)
        {
            dbContext.TutoresMaterias.Add(new TutorMateria
            {
                TutorId = tutor.UsuarioId,
                MateriaId = materiaId
            });
        }

        var distinctDias = request.DiasAtencion?.Distinct().ToList() ?? [];
        foreach (var dia in distinctDias)
        {
            dbContext.TutoresDiasAtencion.Add(new TutorDiaAtencion
            {
                TutorId = tutor.UsuarioId,
                DiaSemana = dia
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new TutorResponseDto(
            tutor.UsuarioId,
            tutor.Nombre,
            tutor.Apellido,
            tutor.CarnetId,
            tutor.NumeroIdentificacion,
            tutor.Genero,
            tutor.Direccion,
            tutor.Telefono,
            tutor.FechaNacimiento,
            tutor.FotografiaUrl,
            tutor.DireccionTutoria,
            tutor.AnioInicio,
            tutor.Universidad,
            tutor.HoraInicio,
            tutor.HoraFin,
            usuario.Correo,
            rolTutor.Nombre,
            estadoPendiente.Nombre,
            usuario.FechaRegistro,
            distinctDias,
            distinctMateriaIds
        );

        return Results.Created($"/api/tutores/{tutor.UsuarioId}", response);
    }
}
