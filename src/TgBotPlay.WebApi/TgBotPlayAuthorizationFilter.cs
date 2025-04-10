using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI;

public class TgBotPlayAuthorizationFilter(IOptions<TgBotPlayOptions> options) : IAuthorizationFilter
{
    private readonly TgBotPlayOptions _settings = options.Value;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var botToken = context.RouteData.Values["botToken"]?.ToString();
        if (string.IsNullOrEmpty(botToken) || !botToken.Equals(_settings.Token))
        {
            context.Result = new UnauthorizedObjectResult("Invalid bot token");
            return;
        }

        if (_settings.Secret != null)
        {
            var botSecret = context.HttpContext.Request.Headers["X-Telegram-Bot-Api-Secret-Token"].ToString();
            if (string.IsNullOrEmpty(botSecret) || !botSecret.Equals(_settings.Secret))
            {
                context.Result = new UnauthorizedObjectResult("Invalid bot secret token");
                return;
            }
        }
    }
}