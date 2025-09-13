using TgBotPlay.WebAPI;

namespace TgBotPlay.WebAPI.Example;

public class BotConfiguration
{
    public string Token { get; init; } = default!;
    public TgBotPlayConnectionMethod ConnectionMethod { get; init; } = TgBotPlayConnectionMethod.POLLING;
    public Uri Host { get; init; } = default!;
}
