namespace edu_connect_service.Api.Features.Tutores.EstadisticasDashboard;

public record TutorDashboardStatsResponseDto(
    int SesionesPendientes,
    int PendientesHoy,
    int SesionesAtendidasMes,
    int SesionesCanceladas
);

