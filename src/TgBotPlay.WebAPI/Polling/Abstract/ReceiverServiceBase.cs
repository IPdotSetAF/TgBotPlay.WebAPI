using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TgBotPlay.WebAPI.Polling.Abstract;

/// <summary>An abstract class to compose Receiver Service and Update Handler classes</summary>
/// <typeparam name="TUpdateHandler">Update Handler to use in Update Receiver</typeparam>
public abstract class ReceiverServiceBase<TUpdateHandler>(ITelegramBotClient botClient, TUpdateHandler updateHandler, IOptions<TgBotPlayOptions> options, ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
    : IReceiverService where TUpdateHandler : TgBotPlayUpdateHandlerBase
{
    private TgBotPlayOptions _settings = options.Value;

    /// <summary>
    /// Starts receiving updates from Telegram bot using the configured update handler.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token for stopping the receiver.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        // ToDo: we can inject ReceiverOptions through IOptions container
        var receiverOptions = new ReceiverOptions() { DropPendingUpdates = _settings.DropPendingUpdates, AllowedUpdates = updateHandler.GetImplementedUpdateTypes().ToArray()};

        var me = await botClient.GetMe(stoppingToken);
        logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "My Awesome Bot");

        // Delete any WebHook
        await botClient.DeleteWebhook(dropPendingUpdates: true, stoppingToken);

        // Start receiving updates
        await botClient.ReceiveAsync(updateHandler, receiverOptions, stoppingToken);
    }
}
