using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotPlay.WebAPI.WebHook;

namespace TgBotPlay.WebAPI;

/// <summary>
/// Controller for handling Telegram bot updates via webhook.
/// Provides endpoints for receiving updates and managing webhook lifecycle.
/// </summary>
[AllowAnonymous]
[ServiceFilter(typeof(TgBotPlayAuthorizationFilter))]
public class TgBotPlayController(
    ITelegramBotClient _bot,
    IOptions<TgBotPlayOptions> _options,
    TgBotPlayUpdateHandlerBase _updateHandler,
    ITgBotPlayWebHookService _webHookService
    ) : ControllerBase
{
    private readonly TgBotPlayOptions _settings = _options.Value;

    /// <summary>
    /// Handles incoming Telegram bot updates via webhook.
    /// </summary>
    /// <param name="update">The Telegram update received from the webhook.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success.</returns>
    [HttpPost]
    public async Task<IActionResult> Update(
        [FromBody] Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            await _updateHandler.HandleUpdateAsync(_bot, update, cancellationToken);
        }
        catch (Exception exception)
        {
            await _updateHandler.HandleErrorAsync(_bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, cancellationToken);
        }

        return Ok();
    }

    /// <summary>
    /// Starts the webhook service to begin receiving updates.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> indicating the operation result.</returns>
    [HttpPost("HookUp")]
    public IActionResult HookUp()
    {
        try
        {
            _webHookService.Start();
            return Ok("HookUp Successful!");
        }
        catch (Exception ex)
        {
            return BadRequest($"HookUp Failed! : {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the webhook service to stop receiving updates.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> indicating the operation result.</returns>
    [HttpPost("HookDown")]
    public async Task<IActionResult> HookDown()
    {
        try
        {
            await _webHookService.Stop();
            return Ok("HookDown Successful!");
        }
        catch (Exception ex)
        {
            return BadRequest($"HookDown Failed! : {ex.Message}");
        }
    }
}
