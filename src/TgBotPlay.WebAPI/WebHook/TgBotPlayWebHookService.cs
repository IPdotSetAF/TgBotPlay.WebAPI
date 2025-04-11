using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI.WebHook;

public class TgBotPlayWebHookService(
    IServiceProvider _serviceProvider,
    ILogger<TgBotPlayWebHookService> _logger,
    IOptions<TgBotPlayOptions> _options
    ) : BackgroundService, ITgBotPlayWebHookService
{
    private static CancellationToken StoppingToken;

    private readonly TgBotPlayOptions _settings = _options.Value;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebHook Service Started.");
        return KeepHookAlive(stoppingToken);
    }

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

    public void Start()
    {
        if (!StoppingToken.IsCancellationRequested)
            throw new InvalidOperationException("Service is already running.");

        Task.Run(() => StartAsync(new CancellationToken()));
    }

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
