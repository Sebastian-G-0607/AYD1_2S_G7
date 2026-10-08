using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador.Reportes;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace edu_connect_service.Api.UnitTests.Features.Administrador.Reportes;

public class CalificacionTutoresEndpointTests
{
    private static edu_connect_serviceContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new edu_connect_serviceContext(options);
    }

    private static Tutor AddTutor(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        EstadoUsuario estado,
        Rol rol,
        params Materia[] materias)
    {
        var usuario = new Usuario
        {
            Id = id,
            Correo = $"tutor{id}@educonnect.test",
            PasswordHash = "hash",
            RolId = rol.Id,
            Rol = rol,
            EstadoId = estado.Id,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow
        };
        var tutor = new Tutor
        {
            UsuarioId = id,
            Usuario = usuario,
            Nombre = nombre,
            Apellido = "Prueba",
            CarnetId = $"T-{id}",
            NumeroIdentificacion = $"{id:D13}",
            Genero = "otro",
            Direccion = "Zona 1",
            Telefono = "55550000",
            FotografiaUrl = "tutor.png",
            DireccionTutoria = "USAC",
            Universidad = "USAC"
        };
        foreach (var materia in materias)
        {
            tutor.TutorMaterias.Add(new TutorMateria { Tutor = tutor, Materia = materia });
        }

        context.Add(tutor);
        return tutor;
    }

    private static void AddCalificaciones(
        edu_connect_serviceContext context,
        Tutor tutor,
        Estudiante estudiante,
        Materia materia,
        EstadoSesion atendida,
        int primerIdSesion,
        params int[] estrellas)
    {
        var idSesion = primerIdSesion;
        foreach (var valor in estrellas)
        {
            context.Add(new CalificacionTutor
            {
                Estrellas = valor,
                FechaCreacion = DateTime.UtcNow,
                Sesion = new Sesion
                {
                    Id = idSesion++,
                    Tutor = tutor,
                    TutorId = tutor.UsuarioId,
                    Estudiante = estudiante,
                    EstudianteId = estudiante.UsuarioId,
                    Materia = materia,
                    MateriaId = materia.Id,
                    Estado = atendida,
                    EstadoId = atendida.Id,
                    FechaSesion = new DateOnly(2026, 10, 1),
                    HoraInicio = new TimeOnly(10, 0),
                    Motivo = "Repaso"
                }
            });
        }
    }

    [Theory]
    [InlineData(new int[] { }, null)]
    [InlineData(new[] { 5 }, 5.0)]
    [InlineData(new[] { 4, 5 }, 4.5)]
    [InlineData(new[] { 0, 5, 5 }, 3.33)]
    [InlineData(new[] { 0, 0 }, 0.0)]
    public void CalcularPromedio_RetornaPromedioRedondeadoONulo(int[] estrellas, double? esperado)
    {
        Assert.Equal(esperado, CalificacionTutoresEndpoint.CalcularPromedio(estrellas));
    }

    [Fact]
    public async Task HandleAsync_TutoresActivosConPromedioYSinCalificar()
    {
        using var context = CreateContext();
        var rol = new Rol { Id = 1, Nombre = "Tutor" };
        var aprobado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var materiaA = new Materia { Id = 1, Nombre = "Física" };
        var materiaB = new Materia { Id = 2, Nombre = "Química" };
        var atendida = new EstadoSesion { Id = 1, Nombre = "ATENDIDA" };
        var estudianteUsuario = new Usuario
        {
            Id = 50,
            Correo = "est@educonnect.test",
            PasswordHash = "hash",
            RolId = rol.Id,
            Rol = rol,
            EstadoId = aprobado.Id,
            Estado = aprobado,
            FechaRegistro = DateTime.UtcNow
        };
        var estudiante = new Estudiante
        {
            UsuarioId = 50,
            Usuario = estudianteUsuario,
            Nombre = "Est",
            Apellido = "Prueba",
            Carnet = "E-50",
            Genero = "otro",
            Direccion = "Zona 1",
            Telefono = "55551111"
        };
        var conNotas = AddTutor(context, 10, "Ana", aprobado, rol, materiaA, materiaB);
        AddTutor(context, 11, "Beto", aprobado, rol);
        context.AddRange(materiaA, materiaB, atendida, estudiante);
        AddCalificaciones(context, conNotas, estudiante, materiaA, atendida, 100, 5, 4, 3);
        await context.SaveChangesAsync();

        var result = await CalificacionTutoresEndpoint.HandleAsync(context, CancellationToken.None);

        var ok = Assert.IsType<Ok<List<CalificacionTutorReporteDto>>>(result);
        var filas = ok.Value!;
        Assert.Equal(2, filas.Count);
        Assert.Equal("Ana Prueba", filas[0].NombreCompleto);
        Assert.Equal(4.0, filas[0].PromedioCalificacion);
        Assert.Equal(3, filas[0].TotalCalificaciones);
        Assert.Contains("Física", filas[0].Especialidad);
        Assert.Equal("Beto Prueba", filas[1].NombreCompleto);
        Assert.Null(filas[1].PromedioCalificacion);
        Assert.Equal(0, filas[1].TotalCalificaciones);
        Assert.Equal("Sin especialidad registrada", filas[1].Especialidad);
    }

    [Fact]
    public async Task HandleAsync_ExcluyeTutoresNoActivos()
    {
        using var context = CreateContext();
        var rol = new Rol { Id = 1, Nombre = "Tutor" };
        var aprobado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var inactivo = new EstadoUsuario { Id = 2, Nombre = "INACTIVO" };
        AddTutor(context, 10, "Activo", aprobado, rol);
        AddTutor(context, 11, "Baja", inactivo, rol);
        await context.SaveChangesAsync();

        var result = await CalificacionTutoresEndpoint.HandleAsync(context, CancellationToken.None);

        var ok = Assert.IsType<Ok<List<CalificacionTutorReporteDto>>>(result);
        var fila = Assert.Single(ok.Value!);
        Assert.Equal("Activo Prueba", fila.NombreCompleto);
    }
}
