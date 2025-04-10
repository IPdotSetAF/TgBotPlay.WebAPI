namespace TgBotPlay.WebAPI.WebHook;

public interface ITgBotPlayWebHookService
{
    void Start();
    Task Stop();
}
