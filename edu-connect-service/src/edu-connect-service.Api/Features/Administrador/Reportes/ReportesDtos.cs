namespace edu_connect_service.Api.Features.Administrador.Reportes;

public record TutorAtencionesReporteDto(
    int TutorId,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string Carnet,
    string Correo,
    string? FotografiaUrl,
    int TotalSesionesAtendidas,
    int TotalEstudiantesAtendidos
);

public record MateriaDemandaReporteDto(
    int MateriaId,
    string NombreMateria,
    int TotalSesiones,
    int SesionesAtendidas,
    int SesionesPendientes,
    int SesionesCanceladas,
    double PorcentajeDemanda
);

public record ReportesResumenDto(
    int TotalSesiones,
    int TotalSesionesAtendidas,
    int TotalSesionesPendientes,
    int TotalSesionesCanceladas,
    double TasaEfectividad,
    int TotalTutoresConAtenciones,
    int TotalMateriasConDemanda,
    string? TutorTopNombre,
    int TutorTopAtenciones,
    string? MateriaTopNombre,
    int MateriaTopSesiones
);

public record EstudianteSesionesReporteDto(
    int EstudianteId,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string Carnet,
    string Correo,
    string? FotografiaUrl,
    int TotalSesionesProgramadas,
    int SesionesAtendidas,
    int SesionesCanceladas,
    int SesionesPendientes
);

public record MateriaCancelacionReporteDto(
    int MateriaId,
    string NombreMateria,
    int TotalSesiones,
    int SesionesCanceladas,
    int SesionesAtendidas,
    int SesionesPendientes,
    double TasaCancelacion
);

public record MateriaAsistenciaVsCancelacionReporteDto(
    int MateriaId,
    string NombreMateria,
    int TotalSesiones,
    int SesionesAtendidas,
    int SesionesCanceladas,
    int SesionesPendientes,
    double TasaAsistencia,
    double TasaCancelacion
);

public record EstudianteCalificacionReporteDto(
    int EstudianteId,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    string Carnet,
    string Correo,
    string? FotografiaUrl,
    int TotalSesionesAtendidas,
    int TotalEvaluaciones,
    double PromedioCalificacion
);

