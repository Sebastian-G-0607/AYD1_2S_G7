using edu_connect_service.Api.Data;
using edu_connect_service.Api.Features.Administrador.Reportes;
using edu_connect_service.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace edu_connect_service.Api.UnitTests.Features.Administrador.Reportes;

public class EstudiantesCalificacionesReporteTests
{
    private static edu_connect_serviceContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<edu_connect_serviceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new edu_connect_serviceContext(options);
        SeedBaseData(context);
        return context;
    }

    private static void SeedBaseData(edu_connect_serviceContext context)
    {
        var rolTutor = new Rol { Id = 1, Nombre = "Tutor" };
        var rolEstudiante = new Rol { Id = 2, Nombre = "Estudiante" };
        var rolAdmin = new Rol { Id = 3, Nombre = "Administrador" };
        context.Roles.AddRange(rolTutor, rolEstudiante, rolAdmin);

        var estadoAprobado = new EstadoUsuario { Id = 1, Nombre = "APROBADO" };
        var estadoInactivo = new EstadoUsuario { Id = 2, Nombre = "INACTIVO" };
        var estadoPendiente = new EstadoUsuario { Id = 3, Nombre = "PENDIENTE" };
        context.EstadosUsuarios.AddRange(estadoAprobado, estadoInactivo, estadoPendiente);

        var estadoSesionPendiente = new EstadoSesion { Id = 1, Nombre = "PENDIENTE" };
        var estadoSesionAtendida = new EstadoSesion { Id = 2, Nombre = "ATENDIDA" };
        var estadoSesionCancelada = new EstadoSesion { Id = 3, Nombre = "CANCELADA_TUTOR" };
        context.EstadosSesiones.AddRange(estadoSesionPendiente, estadoSesionAtendida, estadoSesionCancelada);

        var materia1 = new Materia { Id = 1, Nombre = "Matemática Básica" };
        context.Materias.Add(materia1);

        context.SaveChanges();
    }

    private static Estudiante CrearEstudiantePrueba(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        string apellido,
        string carnet,
        string correo,
        string estadoNombre = "APROBADO")
    {
        var estado = context.EstadosUsuarios.First(e => e.Nombre == estadoNombre);
        var rolEstudiante = context.Roles.First(r => r.Nombre == "Estudiante");

        var usuario = new Usuario
        {
            Id = id,
            Correo = correo,
            PasswordHash = "hash123",
            RolId = rolEstudiante.Id,
            Rol = rolEstudiante,
            EstadoId = estado.Id,
            Estado = estado,
            CorreoValidado = true,
            FechaRegistro = DateTime.UtcNow
        };

        var estudiante = new Estudiante
        {
            UsuarioId = id,
            Usuario = usuario,
            Nombre = nombre,
            Apellido = apellido,
            Carnet = carnet,
            Genero = "masculino",
            Direccion = "Guatemala",
            Telefono = "12345678",
            FechaNacimiento = new DateOnly(2001, 1, 1),
            FotografiaUrl = "https://example.com/foto.jpg"
        };

        context.Usuarios.Add(usuario);
        context.Estudiantes.Add(estudiante);
        context.SaveChanges();

        return estudiante;
    }

    private static Tutor CrearTutorPrueba(
        edu_connect_serviceContext context,
        int id,
        string nombre,
        string apellido,
        string carnet,
        string correo)
    {
        var estado = context.EstadosUsuarios.First(e => e.Nombre == "APROBADO");
        var rolTutor = context.Roles.First(r => r.Nombre == "Tutor");

        var usuario = new Usuario
        {
            Id = id,
            Correo = correo,
            PasswordHash = "hash123",
            RolId = rolTutor.Id,
            Rol = rolTutor,
            EstadoId = estado.Id,
            Estado = estado,
            FechaRegistro = DateTime.UtcNow
        };

        var tutor = new Tutor
        {
            UsuarioId = id,
            Usuario = usuario,
            Nombre = nombre,
            Apellido = apellido,
            CarnetId = carnet,
            NumeroIdentificacion = "1234567890101",
            Genero = "masculino",
            Direccion = "Guatemala",
            Telefono = "55551234",
            FechaNacimiento = new DateOnly(1995, 5, 20),
            FotografiaUrl = "https://example.com/tutor.jpg",
            DireccionTutoria = "Edificio T3",
            Universidad = "USAC"
        };

        context.Usuarios.Add(usuario);
        context.Tutores.Add(tutor);
        context.SaveChanges();

        return tutor;
    }

    private static Sesion CrearSesionPrueba(
        edu_connect_serviceContext context,
        int sesionId,
        int estudianteId,
        int tutorId,
        string estadoNombre = "ATENDIDA",
        int? calificacionEstrellas = null)
    {
        var estado = context.EstadosSesiones.First(e => e.Nombre == estadoNombre);
        var materia = context.Materias.First();

        var sesion = new Sesion
        {
            Id = sesionId,
            EstudianteId = estudianteId,
            TutorId = tutorId,
            MateriaId = materia.Id,
            EstadoId = estado.Id,
            Estado = estado,
            FechaSesion = DateOnly.FromDateTime(DateTime.UtcNow),
            HoraInicio = new TimeOnly(10, 0),
            HoraFin = new TimeOnly(11, 0),
            Motivo = "Tutoría de refuerzo",
            FechaCreacion = DateTime.UtcNow
        };

        if (calificacionEstrellas.HasValue)
        {
            sesion.CalificacionEstudiante = new CalificacionEstudiante
            {
                Id = sesionId,
                SesionId = sesionId,
                Sesion = sesion,
                Estrellas = calificacionEstrellas.Value,
                Comentario = "Buen desempeño",
                FechaCreacion = DateTime.UtcNow
            };
        }

        context.Sesiones.Add(sesion);
        context.SaveChanges();

        return sesion;
    }

    [Fact]
    public async Task HandleAsync_ConMultiplesCalificaciones_CalculaPromedioYTotalesCorrectamente()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 100, "Carlos", "Tutor", "T-100", "tutor@test.com");
        CrearEstudiantePrueba(context, 1, "Ana", "Gomez", "202200001", "ana@test.com");

        CrearSesionPrueba(context, 10, 1, 100, "ATENDIDA", calificacionEstrellas: 5);
        CrearSesionPrueba(context, 11, 1, 100, "ATENDIDA", calificacionEstrellas: 4);
        CrearSesionPrueba(context, 12, 1, 100, "ATENDIDA", calificacionEstrellas: 3);

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Single(lista);

        var dto = lista[0];
        Assert.Equal(1, dto.EstudianteId);
        Assert.Equal("Ana Gomez", dto.NombreCompleto);
        Assert.Equal("202200001", dto.Carnet);
        Assert.Equal(3, dto.TotalSesionesAtendidas);
        Assert.Equal(3, dto.TotalEvaluaciones);
        Assert.Equal(4.0, dto.PromedioCalificacion);
    }

    [Fact]
    public async Task HandleAsync_EstudianteActivoSinSesiones_ApareceConPromedioCero()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 2, "Mario", "Lopez", "202200002", "mario@test.com");

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Single(lista);

        var dto = lista[0];
        Assert.Equal(2, dto.EstudianteId);
        Assert.Equal("Mario Lopez", dto.NombreCompleto);
        Assert.Equal(0, dto.TotalSesionesAtendidas);
        Assert.Equal(0, dto.TotalEvaluaciones);
        Assert.Equal(0.0, dto.PromedioCalificacion);
    }

    [Fact]
    public async Task HandleAsync_SesionesAtendidasSinCalificacion_CuentaSesionesPeroCalificacionesCero()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 100, "Carlos", "Tutor", "T-100", "tutor@test.com");
        CrearEstudiantePrueba(context, 3, "Lucia", "Morales", "202200003", "lucia@test.com");

        CrearSesionPrueba(context, 20, 3, 100, "ATENDIDA", calificacionEstrellas: null);
        CrearSesionPrueba(context, 21, 3, 100, "ATENDIDA", calificacionEstrellas: null);

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Single(lista);

        var dto = lista[0];
        Assert.Equal(3, dto.EstudianteId);
        Assert.Equal(2, dto.TotalSesionesAtendidas);
        Assert.Equal(0, dto.TotalEvaluaciones);
        Assert.Equal(0.0, dto.PromedioCalificacion);
    }

    [Fact]
    public async Task HandleAsync_SesionesNoAtendidas_NoLasTomaEnCuentaParaElCalculo()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 100, "Carlos", "Tutor", "T-100", "tutor@test.com");
        CrearEstudiantePrueba(context, 4, "Elena", "Rivas", "202200004", "elena@test.com");

        CrearSesionPrueba(context, 30, 4, 100, "ATENDIDA", calificacionEstrellas: 5);
        CrearSesionPrueba(context, 31, 4, 100, "PENDIENTE", calificacionEstrellas: 1);
        CrearSesionPrueba(context, 32, 4, 100, "CANCELADA_TUTOR", calificacionEstrellas: 1);

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Single(lista);

        var dto = lista[0];
        Assert.Equal(1, dto.TotalSesionesAtendidas);
        Assert.Equal(1, dto.TotalEvaluaciones);
        Assert.Equal(5.0, dto.PromedioCalificacion);
    }

    [Fact]
    public async Task HandleAsync_EstudiantesNoAprobados_SonExcluidosDelReporte()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 5, "Estudiante Activo", "Aprobado", "202200005", "activo@test.com", "APROBADO");
        CrearEstudiantePrueba(context, 6, "Estudiante Inactivo", "Baja", "202200006", "inactivo@test.com", "INACTIVO");
        CrearEstudiantePrueba(context, 7, "Estudiante Pendiente", "Espera", "202200007", "pendiente@test.com", "PENDIENTE");

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Single(lista);
        Assert.Equal(5, lista[0].EstudianteId);
    }

    [Fact]
    public async Task HandleAsync_ConParametroLimit_LimitaCantidadDeResultados()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 10, "Estudiante 1", "Uno", "202200010", "uno@test.com");
        CrearEstudiantePrueba(context, 11, "Estudiante 2", "Dos", "202200011", "dos@test.com");
        CrearEstudiantePrueba(context, 12, "Estudiante 3", "Tres", "202200012", "tres@test.com");

        // Act
        var result = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: 2,
            search: null,
            orden: null,
            CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(result);
        var lista = okResult.Value;
        Assert.NotNull(lista);
        Assert.Equal(2, lista.Count);
    }

    [Fact]
    public async Task HandleAsync_ConParametroSearch_FiltraPorNombreCarnetOCorreo()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearEstudiantePrueba(context, 20, "Roberto", "Alvarado", "202200020", "roberto@test.com");
        CrearEstudiantePrueba(context, 21, "Sofia", "Castillo", "202200021", "sofia@test.com");
        CrearEstudiantePrueba(context, 22, "Diego", "Mendez", "202200022", "diego@test.com");

        // Act - Filtrar por carnet
        var resultCarnet = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: "202200021",
            orden: null,
            CancellationToken.None);

        // Assert
        var okCarnet = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(resultCarnet);
        Assert.Single(okCarnet.Value!);
        Assert.Equal("Sofia Castillo", okCarnet.Value![0].NombreCompleto);

        // Act - Filtrar por correo
        var resultCorreo = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: "diego@test.com",
            orden: null,
            CancellationToken.None);

        var okCorreo = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(resultCorreo);
        Assert.Single(okCorreo.Value!);
        Assert.Equal(22, okCorreo.Value![0].EstudianteId);
    }

    [Fact]
    public async Task HandleAsync_ConOrdenAscendenteYDescendente_OrdenaCorrectamente()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        CrearTutorPrueba(context, 100, "Carlos", "Tutor", "T-100", "tutor@test.com");

        CrearEstudiantePrueba(context, 30, "Estudiante Bajo", "Bajo", "202200030", "bajo@test.com");
        CrearEstudiantePrueba(context, 31, "Estudiante Alto", "Alto", "202200031", "alto@test.com");

        CrearSesionPrueba(context, 40, 30, 100, "ATENDIDA", calificacionEstrellas: 2);
        CrearSesionPrueba(context, 41, 31, 100, "ATENDIDA", calificacionEstrellas: 5);

        // Act - Por defecto (Descendente)
        var resultDesc = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: null,
            CancellationToken.None);

        var okDesc = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(resultDesc);
        var listaDesc = okDesc.Value!;
        Assert.Equal(31, listaDesc[0].EstudianteId);
        Assert.Equal(30, listaDesc[1].EstudianteId);

        // Act - Ascendente
        var resultAsc = await EstudiantesCalificacionesEndpoint.HandleAsync(
            context,
            limit: null,
            search: null,
            orden: "asc",
            CancellationToken.None);

        var okAsc = Assert.IsType<Ok<List<EstudianteCalificacionReporteDto>>>(resultAsc);
        var listaAsc = okAsc.Value!;
        Assert.Equal(30, listaAsc[0].EstudianteId);
        Assert.Equal(31, listaAsc[1].EstudianteId);
    }
}
