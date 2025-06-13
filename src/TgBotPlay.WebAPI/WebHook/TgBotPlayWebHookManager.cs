using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace TgBotPlay.WebAPI.WebHook;

public class TgBotPlayWebHookManager(
    ITelegramBotClient _bot,
    TgBotPlayUpdateHandlerBase _updateHandler,
    ILogger<TgBotPlayWebHookManager> _logger,
    IOptions<TgBotPlayOptions> _options)
{
    private TgBotPlayOptions _settings = _options.Value;

    public async Task HookUp(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Setting WebHook: {_settings.WebHookUrl}");
        try
        {
            await _bot.SetWebhook(
                        url: _settings.WebHookUrl,
                        allowedUpdates: _updateHandler.GetImplementedUpdateTypes(),
                        secretToken: _settings.Secret,
                        cancellationToken: cancellationToken,
                        dropPendingUpdates: _settings.DropPendingUpdates);
            _logger.LogInformation($"WebHook Successfully Set!");
        }
        catch (Exception)
        {
            _logger.LogInformation($"Failed to set WebHook!");
        }
    }

    public async Task HookDown(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing WebHook");
        try
        {
            await _bot.DeleteWebhook(cancellationToken: cancellationToken);
            _logger.LogInformation("Removed WebHook Successfully!");
        }
        catch (Exception)
        {
            _logger.LogInformation($"Failed to remove WebHook!");
        }
    }
}
