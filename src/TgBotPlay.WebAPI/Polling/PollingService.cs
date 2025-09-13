using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TgBotPlay.WebAPI.Polling.Abstract;

namespace TgBotPlay.WebAPI.Polling;

/// <summary>
/// Background service that handles Telegram bot polling for receiving updates.
/// Composes polling functionality with receiver service implementation.
/// </summary>
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger, IOptions<TgBotPlayOptions> options)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger, options);
