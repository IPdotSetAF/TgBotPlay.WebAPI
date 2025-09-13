namespace TgBotPlay.WebAPI.WebHook;

/// <summary>
/// Service interface for managing Telegram bot webhook lifecycle.
/// Provides methods to start and stop webhook services.
/// </summary>
public interface ITgBotPlayWebHookService
{
    /// <summary>
    /// Starts the webhook service to begin receiving updates from Telegram.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stops the webhook service and removes the webhook from Telegram.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Stop();
}
