using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace TgBotPlay.WebAPI.WebHook;

/// <summary>
/// Manages Telegram bot webhook registration and removal.
/// Handles the communication with Telegram API for webhook lifecycle management.
/// </summary>
public class TgBotPlayWebHookManager(
    ITelegramBotClient _bot,
    TgBotPlayUpdateHandlerBase _updateHandler,
    ILogger<TgBotPlayWebHookManager> _logger,
    IOptions<TgBotPlayOptions> _options)
{
    private TgBotPlayOptions _settings = _options.Value;

    /// <summary>
    /// Registers the webhook with Telegram API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Removes the webhook from Telegram API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
