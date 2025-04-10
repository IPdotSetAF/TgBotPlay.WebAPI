namespace TgBotPlay.WebAPI;

public class TgBotPlayOptions
{
    public string? Token { get; private set; }
    public string? Secret { get; private set; }
    public string? Host { get; private set; }
    public TgBotPlayConnectionMethod ConnectionMethod { get; private set; } = TgBotPlayConnectionMethod.POLLING;

    public string ControllerRouteTemplate { get; private set; } = "TgBotPlay/[action]";
    public string ControllerName { get; private set; } = "TgBotPlay";

    public double PollingSeconds { get; private set; } = 5d;
    public double WebHookRefreshMinuets { get; private set; } = 60d;

    public string WebHookUrl =>
        $"{Host}/{ControllerRouteTemplate.Replace("[action]", Token)}";

    #region Setters

    public TgBotPlayOptions WithToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException("token can not be null or empty.", nameof(token));
        Token = token;

        return this;
    }

    public TgBotPlayOptions AsPollingClient()
    {
        ConnectionMethod = TgBotPlayConnectionMethod.POLLING;

        return this;
    }

    public TgBotPlayOptions AsWebHookClient(string host, string? secret = null)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentNullException("host can not be null or empty.", nameof(host));
        Host = host;

        if (!string.IsNullOrWhiteSpace(secret))
            Secret = secret;

        ConnectionMethod = TgBotPlayConnectionMethod.WEB_HOOK;

        return this;
    }

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

    public TgBotPlayOptions WithPollingInterval(double seconds = 5d)
    {
        PollingSeconds = seconds;

        return this;
    }

    public TgBotPlayOptions WithWebHookRefreshInterval(double minuets = 60d)
    {
        WebHookRefreshMinuets = minuets;

        return this;
    }

    #endregion
}

public enum TgBotPlayConnectionMethod
{
    POLLING,
    WEB_HOOK
}