using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace edu_connect_service.Api.Shared.BackgroundTasks;

public class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    IServiceProvider serviceProvider,
    ILogger<QueuedHostedService> logger) : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<QueuedHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando tarea en segundo plano.");
            }
        }
    }
}
