# Polling Setup

Polling is a simple way to receive updates from Telegram by periodically checking for new messages. It's ideal for development and testing, but can also be used in production for simple bots.

## What is Polling?

Polling is a method where your bot repeatedly asks Telegram's servers for new updates. Your bot makes HTTP requests to the `getUpdates` API endpoint at regular intervals to check for new messages.

**Advantages of Polling:**
- Simple to set up
- No HTTPS required
- Works behind firewalls
- Good for development

**Disadvantages of Polling:**
- Less efficient than webhooks
- Higher server resource usage
- Not real-time (depends on polling interval)
- Can miss updates if bot is offline

## Basic Polling Setup

### 1. Configure Polling Mode

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
);
```

### 2. Run Your Application

```bash
dotnet run
```

TgBotPlay will automatically:
- Start the polling service
- Begin checking for updates
- Process updates as they arrive

## Polling Configuration Options

### Polling Interval

Set how often to check for updates:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(5) // Check every 5 seconds
);
```

**Recommended intervals:**
- **Development**: 1-3 seconds
- **Testing**: 5-10 seconds
- **Production**: 10-30 seconds

### Drop Pending Updates

Control whether to drop pending updates on startup:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .SetDropPendingUpdates(true) // Drop pending updates on startup
);
```

## Complete Polling Example

```csharp
using TgBotPlay.WebAPI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class PollingBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public PollingBotHandler(ITelegramBotClient bot, ILogger<PollingBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text is not null)
        {
            _logger.LogInformation("Received message: {Text}", message.Text);
            
            // Echo the message back
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Echo: {message.Text}"
            );
        }
    }

    protected async Task OnCallbackQuery(CallbackQuery query)
    {
        _logger.LogInformation("Received callback: {Data}", query.Data);
        
        await _bot.AnswerCallbackQueryAsync(query.Id, $"You selected: {query.Data}");
    }
}
```

## Development Configuration

For development, use shorter polling intervals:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(2) // Check every 2 seconds
    .SetDropPendingUpdates(true) // Drop pending updates in development
);

var app = builder.Build();
app.Run();
```

## Production Configuration

For production, use longer polling intervals to reduce server load:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(15) // Check every 15 seconds
    .SetDropPendingUpdates(false) // Keep pending updates in production
);

var app = builder.Build();
app.Run();
```

## Environment-Based Configuration

Configure polling based on environment:

```csharp
var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddTgBotPlay<MyBotHandler>(options =>
{
    options.WithToken("YOUR_BOT_TOKEN");
    
    if (isDevelopment)
    {
        options.AsPollingClient()
               .WithPollingInterval(2)
               .SetDropPendingUpdates(true);
    }
    else
    {
        options.AsPollingClient()
               .WithPollingInterval(15)
               .SetDropPendingUpdates(false);
    }
});

var app = builder.Build();
app.Run();
```

## Polling Service Lifecycle

The polling service runs as a background service and automatically:

1. **Starts** when the application starts
2. **Polls** for updates at the configured interval
3. **Processes** updates through your handlers
4. **Stops** when the application shuts down

### Manual Control

You can control the polling service manually:

```csharp
public class PollingController : ControllerBase
{
    private readonly PollingService _pollingService;

    public PollingController(PollingService pollingService)
    {
        _pollingService = pollingService;
    }

    [HttpPost("start-polling")]
    public IActionResult StartPolling()
    {
        _pollingService.Start();
        return Ok("Polling started");
    }

    [HttpPost("stop-polling")]
    public IActionResult StopPolling()
    {
        _pollingService.Stop();
        return Ok("Polling stopped");
    }
}
```

## Error Handling

The polling service includes built-in error handling:

```csharp
public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    public MyBotHandler(ILogger<MyBotHandler> logger) : base(logger) { }

    protected async Task OnMessage(Message message)
    {
        try
        {
            // Your message handling logic
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message: {MessageText}", message.Text);
        }
    }

    // Override error handling if needed
    public override async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling error occurred");
        
        // Custom error handling
        if (exception is ApiRequestException apiException)
        {
            _logger.LogError("Telegram API Error: {ErrorCode}", apiException.ErrorCode);
        }
        
        await base.HandleErrorAsync(botClient, exception, source, cancellationToken);
    }
}
```

## Health Checks

Monitor your polling service with health checks:

```csharp
builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth();

// In your pipeline
app.MapHealthChecks("/health");
```

The health check will verify:
- Bot connectivity
- Polling service status
- Last update received

## Performance Considerations

### Polling Interval Impact

| Interval | Resource Usage | Responsiveness | API Calls |
|----------|----------------|----------------|-----------|
| 1 second | High | Very High | 3,600/hour |
| 5 seconds | Medium | High | 720/hour |
| 15 seconds | Low | Medium | 240/hour |
| 30 seconds | Very Low | Low | 120/hour |

### Best Practices

1. **Choose appropriate interval**: Balance responsiveness with resource usage
2. **Handle errors gracefully**: Don't let exceptions crash the polling service
3. **Log appropriately**: Use structured logging for debugging
4. **Monitor performance**: Track polling performance and errors
5. **Use connection pooling**: The framework handles this automatically
6. **Consider rate limits**: Telegram has rate limits for API calls

## Switching Between Polling and Webhook

You can easily switch between polling and webhook modes:

```csharp
var builder = WebApplication.CreateBuilder(args);

var useWebhook = builder.Configuration.GetValue<bool>("UseWebhook");
var webhookUrl = builder.Configuration["WebhookUrl"];

builder.Services.AddTgBotPlay<MyBotHandler>(options =>
{
    options.WithToken("YOUR_BOT_TOKEN");
    
    if (useWebhook && !string.IsNullOrEmpty(webhookUrl))
    {
        options.AsWebHookClient(webhookUrl);
    }
    else
    {
        options.AsPollingClient()
               .WithPollingInterval(5);
    }
});
```

## Troubleshooting Polling

### Common Issues

1. **Bot not receiving updates**
   - Check if the bot is running
   - Verify the bot token is correct
   - Check network connectivity

2. **High resource usage**
   - Increase polling interval
   - Check for memory leaks in handlers
   - Monitor CPU usage

3. **Missing updates**
   - Check if bot was offline
   - Verify polling interval is appropriate
   - Check for exceptions in handlers

### Debugging

Enable detailed logging:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Check bot status:

```csharp
var me = await bot.GetMe();
Console.WriteLine($"Bot: @{me.Username}");
```

## Polling vs Webhook

| Feature | Polling | Webhook |
|---------|---------|---------|
| **Setup** | Simple | Complex |
| **HTTPS Required** | No | Yes |
| **Public URL Required** | No | Yes |
| **Resource Usage** | High | Low |
| **Responsiveness** | Delayed | Real-time |
| **Development** | Easy | Medium |
| **Production** | Limited | Recommended |
| **Scalability** | Limited | High |

## When to Use Polling

**Use Polling when:**
- Developing and testing
- Simple bots with low traffic
- Behind firewalls or NAT
- No HTTPS available
- Quick prototyping

**Don't use Polling when:**
- High-traffic production bots
- Real-time response required
- Resource-constrained environments
- Multiple bot instances needed
