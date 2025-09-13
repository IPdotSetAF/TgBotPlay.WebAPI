using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI;

/// <summary>
/// Authorization filter for validating Telegram bot webhook requests.
/// Verifies the secret token sent by Telegram to ensure requests are legitimate.
/// </summary>
public class TgBotPlayAuthorizationFilter(IOptions<TgBotPlayOptions> options) : IAuthorizationFilter
{
    private readonly TgBotPlayOptions _settings = options.Value;

    /// <summary>
    /// Performs authorization by validating the Telegram bot secret token.
    /// </summary>
    /// <param name="context">The authorization filter context containing the HTTP request.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var botSecret = context.HttpContext.Request.Headers["X-Telegram-Bot-Api-Secret-Token"].ToString();
        if (string.IsNullOrEmpty(botSecret) || !botSecret.Equals(_settings.Secret))
        {
            context.Result = new UnauthorizedObjectResult("Invalid bot secret token");
            return;
        }
    }
}