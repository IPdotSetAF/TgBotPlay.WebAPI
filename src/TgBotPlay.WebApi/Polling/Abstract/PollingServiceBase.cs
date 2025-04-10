using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI.Polling.Abstract;

/// <summary>An abstract class to compose Polling background service and Receiver implementation classes</summary>
/// <remarks>See more: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#consuming-a-scoped-service-in-a-background-task</remarks>
/// <typeparam name="TReceiverService">Receiver implementation class</typeparam>
public abstract class PollingServiceBase<TReceiverService>(
    IServiceProvider serviceProvider, 
    ILogger<PollingServiceBase<TReceiverService>> _logger,
    IOptions<TgBotPlayOptions> _options
    ) : BackgroundService where TReceiverService : IReceiverService
{
    private readonly TgBotPlayOptions _settings = _options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting polling service");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        // Make sure we receive updates until Cancellation Requested
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create new IServiceScope on each iteration. This way we can leverage benefits
                // of Scoped TReceiverService and typed HttpClient - we'll grab "fresh" instance each time
                using var scope = serviceProvider.CreateScope();
                var receiver = scope.ServiceProvider.GetRequiredService<TReceiverService>();

                await receiver.ReceiveAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Polling failed with exception: {Exception}", ex);
                // Cooldown if something goes wrong
                await Task.Delay(TimeSpan.FromSeconds(_settings.PollingSeconds), stoppingToken);
            }
        }
    }
}
