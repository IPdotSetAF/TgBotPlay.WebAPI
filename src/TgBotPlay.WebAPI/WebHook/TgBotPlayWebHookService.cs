using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI.WebHook;

/// <summary>
/// Background service that manages the Telegram bot webhook lifecycle.
/// Automatically registers and refreshes the webhook at configured intervals.
/// </summary>
public class TgBotPlayWebHookService(
    IServiceProvider _serviceProvider,
    ILogger<TgBotPlayWebHookService> _logger,
    IOptions<TgBotPlayOptions> _options
    ) : BackgroundService, ITgBotPlayWebHookService
{
    private static CancellationToken StoppingToken;

    private readonly TgBotPlayOptions _settings = _options.Value;

    /// <summary>
    /// Executes the webhook service as a background task.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token for stopping the service.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebHook Service Started.");
        return KeepHookAlive(stoppingToken);
    }

    /// <summary>
    /// Keeps the webhook alive by periodically refreshing it.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token for stopping the service.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task KeepHookAlive(CancellationToken stoppingToken)
    {
        StoppingToken = stoppingToken;
        using var scope = _serviceProvider.CreateScope();
        var WebHookManager = scope.ServiceProvider.GetRequiredService<TgBotPlayWebHookManager>();

        try
        {
            while (!StoppingToken.IsCancellationRequested)
            {
                await WebHookManager.HookUp(StoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_settings.WebHookRefreshMinuets), StoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
        }

        _logger.LogInformation("WebHook Service Stopped.");
    }

    /// <summary>
    /// Starts the webhook service manually.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the service is already running.</exception>
    public void Start()
    {
        if (!StoppingToken.IsCancellationRequested)
            throw new InvalidOperationException("Service is already running.");

        Task.Run(() => StartAsync(new CancellationToken()));
    }

    /// <summary>
    /// Stops the webhook service and removes the webhook from Telegram.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is already stopped.</exception>
    public async Task Stop()
    {
        if (StoppingToken.IsCancellationRequested)
            throw new InvalidOperationException("Service is already stopped.");

        await StopAsync(StoppingToken);

        using var scope = _serviceProvider.CreateScope();
        var WebHookManager = scope.ServiceProvider.GetRequiredService<TgBotPlayWebHookManager>();

        await WebHookManager.HookDown(CancellationToken.None);
    }
}
