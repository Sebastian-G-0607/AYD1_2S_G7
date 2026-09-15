namespace edu_connect_service.Api.Features.Administrador.Reportes;

public static class ReportesEndpoints
{
    public static void MapReportes(this IEndpointRouteBuilder app)
    {
        app.MapTutoresMasAtenciones();
        app.MapMateriasMayorDemanda();
        app.MapResumenReportes();
    }
}
