using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TgBotPlay.WebAPI.Polling.Abstract;

namespace TgBotPlay.WebAPI.Polling;

// Compose Polling and ReceiverService implementations
public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger, IOptions<TgBotPlayOptions> options)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger, options);
