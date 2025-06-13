using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace TgBotPlay.WebAPI;

public class TgBotPlayAuthorizationFilter(IOptions<TgBotPlayOptions> options) : IAuthorizationFilter
{
    private readonly TgBotPlayOptions _settings = options.Value;

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