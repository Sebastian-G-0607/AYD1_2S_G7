using edu_connect_service.Api.Shared.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace edu_connect_service.Api.UnitTests.Shared.BackgroundTasks;

public class BackgroundTaskQueueTests
{
    [Fact]
    public async Task QueueBackgroundWorkItemAsync_WithNullItem_ThrowsArgumentNullException()
    {
        var queue = new BackgroundTaskQueue(10);
        await Assert.ThrowsAsync<ArgumentNullException>(() => queue.QueueBackgroundWorkItemAsync(null!).AsTask());
    }

    [Fact]
    public async Task QueueAndDequeue_ReturnsWorkItemSuccessfully()
    {
        var queue = new BackgroundTaskQueue(10);
        var executed = false;

        await queue.QueueBackgroundWorkItemAsync((sp, ct) =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        var workItem = await queue.DequeueAsync(CancellationToken.None);
        Assert.NotNull(workItem);

        var services = new ServiceCollection().BuildServiceProvider();
        await workItem(services, CancellationToken.None);
        Assert.True(executed);
    }

    [Fact]
    public async Task QueuedHostedService_ExecutesWorkItemFromQueue()
    {
        var queue = new BackgroundTaskQueue(10);
        var services = new ServiceCollection().BuildServiceProvider();
        var loggerMock = new Mock<ILogger<QueuedHostedService>>();
        var executed = false;

        await queue.QueueBackgroundWorkItemAsync((sp, ct) =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        var hostedService = new QueuedHostedService(queue, services, loggerMock.Object);
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        await hostedService.StartAsync(cts.Token);
        await Task.Delay(50);
        await hostedService.StopAsync(CancellationToken.None);

        Assert.True(executed);
    }
}
