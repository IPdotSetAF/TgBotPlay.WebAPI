# Getting Started

This guide will help you get up and running with TgBotPlay.WebAPI in just a few minutes.

## Prerequisites

Before you begin, make sure you have:

- [.NET 6.0 or later](https://dotnet.microsoft.com/download) installed
- A Telegram bot token (get one from [@BotFather](https://t.me/botfather))
- An ASP.NET Core WebAPI project (or create a new one)

## Installation

### 1. Install the NuGet Package

Add the TgBotPlay.WebAPI package to your project:

```bash
dotnet add package TgBotPlay.WebAPI
```

Or via Package Manager Console:

```powershell
Install-Package TgBotPlay.WebAPI
```

### 2. Create Your Bot Handler

Create a class that inherits from `TgBotPlayUpdateHandlerBase`:

```csharp
using TgBotPlay.WebAPI;
using Telegram.Bot.Types;

public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    public MyBotHandler(ILogger<MyBotHandler> logger) : base(logger) { }

    protected async Task OnMessage(Message message)
    {
        // Handle incoming messages
        _logger.LogInformation("Received message: {MessageText}", message.Text);
        
        // Echo the message back
        // Note: You'll need to inject ITelegramBotClient to send messages
    }

    protected async Task OnCallbackQuery(CallbackQuery query)
    {
        // Handle callback queries from inline keyboards
        _logger.LogInformation("Received callback: {CallbackData}", query.Data);
    }
}
```

### 3. Register TgBotPlay in Program.cs

Add TgBotPlay to your service collection:

```csharp
using TgBotPlay.WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add TgBotPlay services
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN_HERE")
    .AsPollingClient() // or .AsWebHookClient("https://yourdomain.com")
);

// Add other services
builder.Services.AddControllers();

var app = builder.Build();

// Configure the pipeline
app.MapControllers();
app.Run();
```

### 4. Run Your Bot

Start your application:

```bash
dotnet run
```

Your bot is now running and will respond to messages!

## Quick Examples

### Basic Message Handler

```csharp
public class EchoBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public EchoBotHandler(ITelegramBotClient bot, ILogger<EchoBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text is not null)
        {
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Echo: {message.Text}"
            );
        }
    }
}
```

### Inline Keyboard Handler

```csharp
protected async Task OnMessage(Message message)
{
    if (message.Text == "/start")
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Option 1", "opt1") },
            new[] { InlineKeyboardButton.WithCallbackData("Option 2", "opt2") }
        });

        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Choose an option:",
            replyMarkup: keyboard
        );
    }
}

protected async Task OnCallbackQuery(CallbackQuery query)
{
    await _bot.AnswerCallbackQueryAsync(query.Id, $"You selected: {query.Data}");
    
    await _bot.SendTextMessageAsync(
        chatId: query.Message.Chat.Id,
        text: $"You chose: {query.Data}"
    );
}
```

## Configuration Options

### Polling vs Webhook

**For Development (Polling):**
```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(5) // seconds
);
```

**For Production (Webhook):**
```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithWebHookRefreshInterval(60) // minutes
);
```

### Custom Controller Routes

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("TelegramBot", "api/telegram/[action]")
);
```

This creates endpoints:
- `POST /api/telegram/` - Main webhook endpoint
- `POST /api/telegram/HookUp` - Start webhook
- `POST /api/telegram/HookDown` - Stop webhook

## Health Checks

Add health checks to monitor your bot:

```csharp
builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth();

// In your pipeline
app.MapHealthChecks("/health");
```

## Next Steps

- Learn about [Configuration Options](configuration.md)
- Explore [Update Handlers](handlers.md)
- Set up [Webhook](webhook.md) or [Polling](polling.md)
- Check out [Examples](examples.md) for more advanced usage

## Troubleshooting

If you encounter issues:

1. **Bot not responding**: Check your bot token and ensure the bot is running
2. **Webhook issues**: Verify your domain is accessible and HTTPS is enabled
3. **Handler not called**: Ensure your method names start with "On" and match Telegram update types

For more help, see our [Troubleshooting Guide](troubleshooting.md).