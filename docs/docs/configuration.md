# Configuration

TgBotPlay.WebAPI provides a comprehensive configuration system through the `TgBotPlayOptions` class. This guide covers all available configuration options and how to use them.

## Basic Configuration

### Required Configuration

The only required configuration is the bot token:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN_HERE")
);
```

### Connection Methods

Choose between polling and webhook for receiving updates:

#### Polling (Development)
```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
);
```

#### Webhook (Production)
```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
);
```

## Complete Configuration Reference

### TgBotPlayOptions Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Token` | `string?` | `null` | Telegram bot token (required) |
| `Host` | `string?` | `null` | Host URL for webhook configuration |
| `ConnectionMethod` | `TgBotPlayConnectionMethod` | `POLLING` | Connection method (POLLING or WEB_HOOK) |
| `ControllerRouteTemplate` | `string` | `"TgBotPlay/[action]"` | Route template for controller actions |
| `ControllerName` | `string` | `"TgBotPlay"` | Name of the controller |
| `PollingSeconds` | `double` | `5.0` | Polling interval in seconds |
| `WebHookRefreshMinuets` | `double` | `60.0` | Webhook refresh interval in minutes |
| `DropPendingUpdates` | `bool` | `false` | Whether to drop pending updates on startup |
| `Secret` | `string?` | `null` | Secret token for webhook authentication (auto-generated) |
| `WebHookUrl` | `string` | Computed | Complete webhook URL (read-only) |

### Configuration Methods

#### WithToken(string token)
Sets the Telegram bot token and automatically generates a secret for webhook authentication.

```csharp
options.WithToken("1234567890:ABCdefGHIjklMNOpqrsTUVwxyz")
```

**Parameters:**
- `token` (string): The Telegram bot token

**Throws:** `ArgumentNullException` if token is null or empty

#### AsPollingClient()
Configures the bot to use polling for receiving updates.

```csharp
options.AsPollingClient()
```

#### AsWebHookClient(string host)
Configures the bot to use webhook for receiving updates.

```csharp
options.AsWebHookClient("https://yourdomain.com")
```

**Parameters:**
- `host` (string): The host URL where the webhook will be registered

**Throws:** `ArgumentNullException` if host is null or empty

#### WithController(string name, string template)
Configures the controller name and route template.

```csharp
options.WithController("TelegramBot", "api/telegram/[action]")
```

**Parameters:**
- `name` (string): The name of the controller
- `template` (string): The route template that must contain '[action]' placeholder

**Throws:** `ArgumentNullException` if name or template is null/empty, or template doesn't contain '[action]'

#### WithPollingInterval(double seconds)
Sets the polling interval for receiving updates.

```csharp
options.WithPollingInterval(10) // 10 seconds
```

**Parameters:**
- `seconds` (double): The polling interval in seconds (default: 5)

#### WithWebHookRefreshInterval(double minutes)
Sets the webhook refresh interval.

```csharp
options.WithWebHookRefreshInterval(30) // 30 minutes
```

**Parameters:**
- `minutes` (double): The refresh interval in minutes (default: 60)

#### SetDropPendingUpdates(bool dropPendingChanges)
Sets whether to drop pending updates on startup.

```csharp
options.SetDropPendingUpdates(true)
```

**Parameters:**
- `dropPendingChanges` (bool): True to drop pending updates, false otherwise (default: false)

## Configuration Examples

### Development Configuration

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(5)
    .SetDropPendingUpdates(true) // Drop pending updates in development
);
```

### Production Configuration

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("TelegramBot", "api/telegram/[action]")
    .WithWebHookRefreshInterval(60)
    .SetDropPendingUpdates(false) // Keep pending updates in production
);
```

### Custom Controller Routes

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("BotController", "webhook/telegram/[action]")
);
```

This creates the following endpoints:
- `POST /webhook/telegram/` - Main webhook endpoint
- `POST /webhook/telegram/HookUp` - Start webhook
- `POST /webhook/telegram/HookDown` - Stop webhook

### Environment-Based Configuration

```csharp
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddTgBotPlay<MyBotHandler>(options =>
{
    options.WithToken(builder.Configuration["TelegramBot:Token"]);
    
    if (isDevelopment)
    {
        options.AsPollingClient()
               .WithPollingInterval(3)
               .SetDropPendingUpdates(true);
    }
    else
    {
        options.AsWebHookClient(builder.Configuration["TelegramBot:Host"])
               .WithWebHookRefreshInterval(30);
    }
});
```

## Configuration from appsettings.json

You can also configure TgBotPlay using `appsettings.json`:

```json
{
  "TelegramBot": {
    "Token": "YOUR_BOT_TOKEN",
    "ConnectionMethod": "WEB_HOOK",
    "Host": "https://yourdomain.com",
    "PollingInterval": 5,
    "WebHookRefreshInterval": 60,
    "DropPendingUpdates": false
  }
}
```

Then use it in your configuration:

```csharp
var botConfig = builder.Configuration.GetSection("TelegramBot");

builder.Services.AddTgBotPlay<MyBotHandler>(options =>
{
    options.WithToken(botConfig["Token"]);
    
    if (botConfig["ConnectionMethod"] == "WEB_HOOK")
    {
        options.AsWebHookClient(botConfig["Host"]);
    }
    else
    {
        options.AsPollingClient();
    }
    
    options.WithPollingInterval(double.Parse(botConfig["PollingInterval"] ?? "5"));
    options.WithWebHookRefreshInterval(double.Parse(botConfig["WebHookRefreshInterval"] ?? "60"));
    options.SetDropPendingUpdates(bool.Parse(botConfig["DropPendingUpdates"] ?? "false"));
});
```

## Security Considerations

### Webhook Security

When using webhooks, TgBotPlay automatically generates a secret token based on your bot token for webhook authentication. This helps ensure that only Telegram can send updates to your webhook endpoint.

### Token Security

Never commit your bot token to source control. Use:
- Environment variables
- Azure Key Vault
- User Secrets (for development)
- Configuration providers

```csharp
// Using environment variables
options.WithToken(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"));

// Using user secrets (development)
options.WithToken(builder.Configuration["TelegramBot:Token"]);
```

## Validation

TgBotPlay validates configuration at startup:

- Bot token is required and non-empty
- Host URL is required for webhook mode
- Controller template must contain '[action]' placeholder
- Polling interval must be positive
- Webhook refresh interval must be positive

If validation fails, an `ArgumentNullException` or `ArgumentException` will be thrown during service registration.
