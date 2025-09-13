namespace TgBotPlay.WebAPI.Polling.Abstract;

/// <summary>
/// Interface for services that receive Telegram bot updates via polling.
/// </summary>
public interface IReceiverService
{
    /// <summary>
    /// Starts receiving updates from Telegram bot via polling.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token for stopping the receiver.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ReceiveAsync(CancellationToken stoppingToken);
}
