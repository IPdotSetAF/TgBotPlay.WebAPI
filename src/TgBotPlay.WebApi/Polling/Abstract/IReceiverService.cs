namespace TgBotPlay.WebAPI.Polling.Abstract;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}
