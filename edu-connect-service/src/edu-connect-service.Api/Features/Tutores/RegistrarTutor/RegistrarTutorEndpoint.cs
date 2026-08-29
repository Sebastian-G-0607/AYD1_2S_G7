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
        if (request.Password != request.ConfirmPassword)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseñas no coinciden",
                detail: "La contraseña y la confirmación de contraseña no coinciden."
            );
        }

        if (!PasswordValidator.IsValid(request.Password))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Contraseña inválida",
                detail: "La contraseña debe tener un mínimo de 8 caracteres, incluyendo al menos una letra minúscula, una mayúscula y un número."
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

        if (request.Fotografia is null || request.Fotografia.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Fotografía obligatoria",
                detail: "La fotografía de perfil es obligatoria para tutores."
            );
        }

        var emailExists = await dbContext.Usuarios.AnyAsync(u => u.Correo == request.Correo, cancellationToken);
        if (emailExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Correo duplicado",
                detail: "El correo electrónico ya se encuentra registrado en el sistema."
            );
        }

        var carnetExists = await dbContext.Tutores.AnyAsync(t => t.CarnetId == request.CarnetId, cancellationToken);
        if (carnetExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Carnet duplicado",
                detail: "El carnet o ID ya se encuentra registrado."
            );
        }

        var identificationExists = await dbContext.Tutores.AnyAsync(t => t.NumeroIdentificacion == request.NumeroIdentificacion, cancellationToken);
        if (identificationExists)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Número de identificación duplicado",
                detail: "El número de identificación del tutor ya se encuentra registrado."
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

        if (request.MateriasIds is null || request.MateriasIds.Count == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Materias requeridas",
                detail: "Debe seleccionar al menos una materia como especialidad."
            );
        }

        var existingMateriaIds = await dbContext.Materias
            .Where(m => request.MateriasIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var missingMaterias = request.MateriasIds.Except(existingMateriaIds).ToList();
        if (missingMaterias.Count > 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Materias inexistentes",
                detail: $"Las siguientes materias no existen: {string.Join(", ", missingMaterias)}"
            );
        }

        if (request.DiasAtencion is not null && request.DiasAtencion.Any(d => d < 1 || d > 7))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Día de atención inválido",
                detail: "Los días de atención deben estar entre 1 (Lunes) y 7 (Domingo)."
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
            Correo = request.Correo,
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
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            CarnetId = request.CarnetId,
            NumeroIdentificacion = request.NumeroIdentificacion,
            Genero = generoNormalizado,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            FechaNacimiento = request.FechaNacimiento,
            FotografiaUrl = fotografiaUrl,
            DireccionTutoria = request.DireccionTutoria,
            AnioInicio = request.AnioInicio,
            Universidad = request.Universidad,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        };

        dbContext.Tutores.Add(tutor);

        var distinctMateriaIds = request.MateriasIds.Distinct().ToList();
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
