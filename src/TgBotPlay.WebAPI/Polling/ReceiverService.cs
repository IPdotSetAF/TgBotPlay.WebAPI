using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotPlay.WebAPI.Polling.Abstract;

namespace TgBotPlay.WebAPI.Polling;

/// <summary>
/// Service that receives Telegram bot updates via polling using the configured update handler.
/// Composes receiver functionality with update handler implementation.
/// </summary>
public class ReceiverService(ITelegramBotClient botClient, TgBotPlayUpdateHandlerBase updateHandler, IOptions<TgBotPlayOptions> options, ILogger<ReceiverServiceBase<TgBotPlayUpdateHandlerBase>> logger)
    : ReceiverServiceBase<TgBotPlayUpdateHandlerBase>(botClient, updateHandler, options, logger);
