using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotPlay.WebAPI.Utils;

namespace TgBotPlay.WebAPI.HealthChecks;

public class TgBotPlayHealthCheck(
    ITelegramBotClient _bot,
    IOptions<TgBotPlayOptions> _options) : IHealthCheck
{
    private readonly TgBotPlayOptions _settings = _options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if bot is connected and can receive updates
            var me = await _bot.GetMe(cancellationToken);
            if (me == null)
            {
                return HealthCheckResult.Degraded("Bot is not properly connected to Telegram");
            }

            // Check webhook status if using webhook mode
            if (_settings.ConnectionMethod == TgBotPlayConnectionMethod.WEB_HOOK)
            {
                var webhookInfo = await _bot.GetWebhookInfo(cancellationToken);
                if (webhookInfo == null || string.IsNullOrEmpty(webhookInfo.Url))
                {
                    return HealthCheckResult.Degraded("Webhook is not properly configured");
                }

                if (!webhookInfo.Url.Equals(_settings.WebHookUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return HealthCheckResult.Degraded($"Webhook URL mismatch. Expected: {UrlUtils.ObfuscateWebHookUrl(_settings.WebHookUrl)}, Actual: {UrlUtils.ObfuscateWebHookUrl(webhookInfo.Url)}");
                }
            }

            return HealthCheckResult.Healthy($"Bot {me.Username} is connected and ready to receive updates");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to connect to Telegram", ex);
        }
    }
}