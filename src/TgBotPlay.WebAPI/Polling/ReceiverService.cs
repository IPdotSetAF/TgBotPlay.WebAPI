using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotPlay.WebAPI.Polling.Abstract;

namespace TgBotPlay.WebAPI.Polling;

// Compose Receiver and UpdateHandler implementation
public class ReceiverService(ITelegramBotClient botClient, TgBotPlayUpdateHandlerBase updateHandler, IOptions<TgBotPlayOptions> options, ILogger<ReceiverServiceBase<TgBotPlayUpdateHandlerBase>> logger)
    : ReceiverServiceBase<TgBotPlayUpdateHandlerBase>(botClient, updateHandler, options, logger);
