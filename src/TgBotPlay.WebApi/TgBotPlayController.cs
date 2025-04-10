using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotPlay.WebAPI.WebHook;

namespace TgBotPlay.WebAPI;

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

    [HttpPost("{botToken}")]
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

    [HttpPost("{botToken}/HookUp")]
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

    [HttpPost("{botToken}/HookDown")]
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
