using TgBotPlay.WebAPI.Utils;

namespace TgBotPlay.WebAPI;

/// <summary>
/// Configuration options for TgBotPlay WebAPI integration.
/// Provides fluent API for configuring bot connection, routing, and behavior settings.
/// </summary>
public class TgBotPlayOptions
{
    /// <summary>
    /// Gets the Telegram bot token for authentication.
    /// </summary>
    public string? Token { get; private set; }
    
    /// <summary>
    /// Gets the host URL for webhook configuration.
    /// </summary>
    public string? Host { get; private set; }
    
    /// <summary>
    /// Gets the connection method used for receiving updates (polling or webhook).
    /// </summary>
    public TgBotPlayConnectionMethod ConnectionMethod { get; private set; } = TgBotPlayConnectionMethod.POLLING;

    /// <summary>
    /// Gets the route template for the controller actions.
    /// </summary>
    public string ControllerRouteTemplate { get; private set; } = "TgBotPlay/[action]";
    
    /// <summary>
    /// Gets the name of the controller.
    /// </summary>
    public string ControllerName { get; private set; } = "TgBotPlay";

    /// <summary>
    /// Gets the polling interval in seconds.
    /// </summary>
    public double PollingSeconds { get; private set; } = 5d;
    
    /// <summary>
    /// Gets the webhook refresh interval in minutes.
    /// </summary>
    public double WebHookRefreshMinuets { get; private set; } = 60d;

    /// <summary>
    /// Gets a value indicating whether to drop pending updates on startup.
    /// </summary>
    public bool DropPendingUpdates { get; private set; } = false;

    /// <summary>
    /// Gets the secret token for webhook authentication.
    /// </summary>
    public string? Secret { get; private set; }
    
    /// <summary>
    /// Gets the complete webhook URL constructed from host and route template.
    /// </summary>
    public string WebHookUrl =>
        $"{Host}/{ControllerRouteTemplate.Replace("[action]", "")}";

    #region Setters

    /// <summary>
    /// Sets the Telegram bot token and generates the secret for webhook authentication.
    /// </summary>
    /// <param name="token">The Telegram bot token.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when token is null or empty.</exception>
    public TgBotPlayOptions WithToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException("token can not be null or empty.", nameof(token));
        Token = token;

        Secret = token
            .StringToBytes()
            .Sha1Encrypt()
            .BytesToString();

        return this;
    }

    /// <summary>
    /// Configures the bot to use polling for receiving updates.
    /// </summary>
    /// <returns>The current instance for method chaining.</returns>
    public TgBotPlayOptions AsPollingClient()
    {
        ConnectionMethod = TgBotPlayConnectionMethod.POLLING;

        return this;
    }

    /// <summary>
    /// Configures the bot to use webhook for receiving updates.
    /// </summary>
    /// <param name="host">The host URL where the webhook will be registered.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when host is null or empty.</exception>
    public TgBotPlayOptions AsWebHookClient(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentNullException("host can not be null or empty.", nameof(host));
        Host = host;

        ConnectionMethod = TgBotPlayConnectionMethod.WEB_HOOK;

        return this;
    }

    /// <summary>
    /// Configures the controller name and route template.
    /// </summary>
    /// <param name="name">The name of the controller.</param>
    /// <param name="template">The route template that must contain '[action]' placeholder.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name or template is null/empty, or template doesn't contain '[action]'.</exception>
    public TgBotPlayOptions WithController(string name, string template)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException("Controller name can not be null or empty.", nameof(name));
        ControllerName = name;

        if (string.IsNullOrWhiteSpace(template) || !template.Contains(value: "[action]", StringComparison.Ordinal))
            throw new ArgumentNullException("Controller template can not be null or empty and must contain '[action]'", nameof(template));
        ControllerRouteTemplate = template;

        return this;
    }

    /// <summary>
    /// Sets the polling interval for receiving updates.
    /// </summary>
    /// <param name="seconds">The polling interval in seconds. Default is 5 seconds.</param>
    /// <returns>The current instance for method chaining.</returns>
    public TgBotPlayOptions WithPollingInterval(double seconds = 5d)
    {
        PollingSeconds = seconds;

        return this;
    }

    /// <summary>
    /// Sets the webhook refresh interval.
    /// </summary>
    /// <param name="minuets">The refresh interval in minutes. Default is 60 minutes.</param>
    /// <returns>The current instance for method chaining.</returns>
    public TgBotPlayOptions WithWebHookRefreshInterval(double minuets = 60d)
    {
        WebHookRefreshMinuets = minuets;

        return this;
    }

    /// <summary>
    /// Sets whether to drop pending updates on startup.
    /// </summary>
    /// <param name="dropPendingChanges">True to drop pending updates, false otherwise. Default is false.</param>
    /// <returns>The current instance for method chaining.</returns>
    public TgBotPlayOptions SetDropPendingUpdates(bool dropPendingChanges = false)
    {
        DropPendingUpdates = dropPendingChanges;
        
        return this;
    }

    #endregion
}

/// <summary>
/// Specifies the connection method used for receiving Telegram bot updates.
/// </summary>
public enum TgBotPlayConnectionMethod
{
    /// <summary>
    /// Uses polling to periodically check for new updates from Telegram.
    /// </summary>
    POLLING,
    
    /// <summary>
    /// Uses webhook to receive updates directly from Telegram via HTTP POST.
    /// </summary>
    WEB_HOOK
}