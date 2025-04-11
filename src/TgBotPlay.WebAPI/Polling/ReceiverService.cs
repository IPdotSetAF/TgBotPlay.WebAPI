using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotPlay.WebAPI.Polling.Abstract;

namespace TgBotPlay.WebAPI.Polling;

// Compose Receiver and UpdateHandler implementation
public class ReceiverService(ITelegramBotClient botClient, TgBotPlayUpdateHandlerBase updateHandler, ILogger<ReceiverServiceBase<TgBotPlayUpdateHandlerBase>> logger)
    : ReceiverServiceBase<TgBotPlayUpdateHandlerBase>(botClient, updateHandler, logger);
