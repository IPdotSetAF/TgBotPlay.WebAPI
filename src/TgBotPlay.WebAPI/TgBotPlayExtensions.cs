using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.AspNetCore;
using TgBotPlay.WebAPI.Polling;
using TgBotPlay.WebAPI.WebHook;

namespace TgBotPlay.WebAPI;

/// <summary>
/// Extension methods for configuring TgBotPlay WebAPI services in the dependency injection container.
/// </summary>
public static class TgBotPlayExtensions
{
    /// <summary>
    /// Adds TgBotPlay WebAPI services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TUpdateHandler">The type of update handler that implements <see cref="TgBotPlayUpdateHandlerBase"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">The configuration action for <see cref="TgBotPlayOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
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

    /// <summary>
    /// Adds TgBotPlay health check to the specified <see cref="IHealthChecksBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
    /// <param name="name">The name of the health check. Default is "TgBotPlay".</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> so that additional calls can be chained.</returns>
    public static IHealthChecksBuilder AddTgBotPlayHealth(this IHealthChecksBuilder builder, string name = "TgBotPlay")
    {
        builder.AddCheck<TgBotPlayHealthCheck>(name);
        return builder;
    }

    /// <summary>
    /// Configures and registers the TgBotPlay settings in the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="options">The configuration action for <see cref="TgBotPlayOptions"/>.</param>
    /// <returns>The configured <see cref="TgBotPlayOptions"/> instance.</returns>
    private static TgBotPlayOptions ConfigureSettings(this IServiceCollection services, Action<TgBotPlayOptions> options)
    {
        var settings = new TgBotPlayOptions();
        options.Invoke(settings);

        services.TryAddSingleton(Options.Create(settings));

        return settings;
    }
}
