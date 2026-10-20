namespace edu_connect_service.Api.Shared.BackgroundTasks;

public static class BackgroundTaskExtensions
{
    public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<QueuedHostedService>();
        return services;
    }
}
