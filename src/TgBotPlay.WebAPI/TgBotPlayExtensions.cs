using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.AspNetCore;
using TgBotPlay.WebAPI.Polling;
using TgBotPlay.WebAPI.WebHook;

namespace TgBotPlay.WebAPI;

public static class TgBotPlayExtensions
{
    public static IServiceCollection AddTgBotPlay<TUpdateHandler>(this IServiceCollection services, Action<TgBotPlayOptions> options) where TUpdateHandler : TgBotPlayUpdateHandlerBase
    {
        var settings = services.ConfigureSettings(options);

        services.AddHttpClient("TgBotPlayClient").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
                    httpClient => new TelegramBotClient(settings.Token!, httpClient));

        services.AddScoped<TgBotPlayUpdateHandlerBase, TUpdateHandler>();

        if (settings.ConnectionMethod == TgBotPlayConnectionMethod.WEB_HOOK)
        {
            services.AddScoped<TgBotPlayWebHookManager>();
            services.AddSingleton<ITgBotPlayWebHookService, TgBotPlayWebHookService>();
            services.AddHostedService(p => (TgBotPlayWebHookService)p.GetRequiredService<ITgBotPlayWebHookService>());
        }
        else
        {
            services.AddScoped<ReceiverService>();
            services.AddHostedService<PollingService>();
        }

        services.ConfigureTelegramBotMvc();
        services.AddControllers(opts =>
            opts.Conventions.Add(new ControllerRoutingConvention(typeof(TgBotPlayController), settings.ControllerRouteTemplate, settings.ControllerName))
        );

        services.AddScoped<TgBotPlayAuthorizationFilter>();

        return services;
    }

    public static IHealthChecksBuilder AddTgBotPlayHealth(this IHealthChecksBuilder builder, string name = "TgBotPlay")
    {
        builder.AddCheck<TgBotPlayHealthCheck>(name);
        return builder;
    }

    private static TgBotPlayOptions ConfigureSettings(this IServiceCollection services, Action<TgBotPlayOptions> options)
    {
        var settings = new TgBotPlayOptions();
        options.Invoke(settings);

        services.TryAddSingleton(Options.Create(settings));

        return settings;
    }
}
