# Webhook Setup

Webhooks are the recommended way to receive updates from Telegram in production environments. This guide covers setting up and managing webhooks with TgBotPlay.WebAPI.

## What is a Webhook?

A webhook is an HTTP endpoint that Telegram calls whenever your bot receives an update. Instead of your bot constantly polling Telegram for new messages, Telegram pushes updates directly to your server.

**Advantages of Webhooks:**
- More efficient than polling
- Real-time updates
- Better for production environments
- Lower server resource usage

**Requirements:**
- HTTPS endpoint (Telegram requires SSL)
- Publicly accessible URL
- Valid SSL certificate

## Basic Webhook Setup

### 1. Configure Webhook Mode

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
);
```

### 2. Ensure HTTPS is Enabled

Make sure your application is configured for HTTPS:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps("path/to/certificate.pfx", "password");
    });
});

var app = builder.Build();

// Force HTTPS redirect
app.UseHttpsRedirection();
```

### 3. Run Your Application

```bash
dotnet run
```

TgBotPlay will automatically:
- Register the webhook with Telegram
- Set up the webhook endpoint
- Start the webhook management service

## Webhook Endpoints

TgBotPlay automatically creates the following endpoints:

### Main Webhook Endpoint
- **URL**: `POST /TgBotPlay/` (or your custom route)
- **Purpose**: Receives updates from Telegram
- **Authentication**: Uses secret token for security

### Webhook Management Endpoints
- **URL**: `POST /TgBotPlay/HookUp`
- **Purpose**: Manually start/refresh the webhook
- **Usage**: Call this endpoint to ensure webhook is active

- **URL**: `POST /TgBotPlay/HookDown`
- **Purpose**: Manually stop the webhook
- **Usage**: Call this endpoint to deactivate the webhook

## Custom Controller Routes

You can customize the webhook endpoints:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("TelegramBot", "api/telegram/[action]")
);
```

This creates:
- `POST /api/telegram/` - Main webhook endpoint
- `POST /api/telegram/HookUp` - Start webhook
- `POST /api/telegram/HookDown` - Stop webhook

## Webhook Configuration Options

### Refresh Interval

Set how often the webhook is refreshed:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithWebHookRefreshInterval(30) // Refresh every 30 minutes
);
```

### Drop Pending Updates

Control whether to drop pending updates on startup:

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .SetDropPendingUpdates(false) // Keep pending updates
);
```

## Development with ngrok

For local development, you can use ngrok to create a secure tunnel:

### 1. Install ngrok

```bash
# Windows (Chocolatey)
choco install ngrok

# macOS (Homebrew)
brew install ngrok

# Linux
# Download from https://ngrok.com/download
```

### 2. Start ngrok

```bash
ngrok http 5000 --host-header=localhost:5000
```

### 3. Configure for Development

```csharp
var builder = WebApplication.CreateBuilder(args);

// Get ngrok URL from environment or configuration
var ngrokUrl = Environment.GetEnvironmentVariable("NGROK_URL") ?? "https://your-ngrok-url.ngrok.io";

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient(ngrokUrl)
    .WithWebHookRefreshInterval(5) // Refresh more frequently in development
);
```

### 4. Set Webhook Manually (Optional)

You can also set the webhook manually using the Telegram API:

```bash
curl -X POST "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/setWebhook" \
     -H "Content-Type: application/json" \
     -d '{"url": "https://your-ngrok-url.ngrok.io/TgBotPlay/"}'
```

## Production Deployment

### 1. Configure HTTPS

Ensure your production server has a valid SSL certificate:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.UseHttps("path/to/certificate.pfx", "password");
    });
});
```

### 2. Set Environment Variables

```bash
export TELEGRAM_BOT_TOKEN="your_bot_token"
export WEBHOOK_URL="https://yourdomain.com"
```

### 3. Configure for Production

```csharp
var builder = WebApplication.CreateBuilder(args);

var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
var webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken(botToken)
    .AsWebHookClient(webhookUrl)
    .WithWebHookRefreshInterval(60) // Refresh every hour in production
    .SetDropPendingUpdates(false) // Keep pending updates in production
);
```

## Webhook Security

TgBotPlay automatically generates a secret token based on your bot token for webhook authentication. This helps ensure that only Telegram can send updates to your webhook endpoint.

### Secret Token Generation

The secret token is automatically generated from your bot token using SHA1 hashing:

```csharp
// This happens automatically when you call WithToken()
var secret = botToken
    .StringToBytes()
    .Sha1Encrypt()
    .BytesToString();
```

### Custom Security

You can add additional security measures:

```csharp
// Add IP whitelist filter
builder.Services.AddScoped<TgBotPlayAuthorizationFilter>();

// Or create a custom authorization filter
public class CustomWebhookAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Add your custom authorization logic
        var request = context.HttpContext.Request;
        var clientIp = request.HttpContext.Connection.RemoteIpAddress;
        
        // Check if IP is from Telegram's range
        if (!IsTelegramIp(clientIp))
        {
            context.Result = new UnauthorizedResult();
        }
    }
    
    private bool IsTelegramIp(IPAddress ip)
    {
        // Implement IP whitelist logic
        return true; // Simplified for example
    }
}
```

## Health Checks

Monitor your webhook status with health checks:

```csharp
builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth();

// In your pipeline
app.MapHealthChecks("/health");
```

The health check will verify:
- Bot connectivity
- Webhook configuration
- Webhook URL match

## Troubleshooting Webhooks

### Common Issues

1. **Webhook not receiving updates**
   - Check if your server is accessible from the internet
   - Verify HTTPS is properly configured
   - Check firewall settings

2. **SSL certificate issues**
   - Ensure your SSL certificate is valid
   - Check certificate chain
   - Verify domain matches certificate

3. **Webhook URL mismatch**
   - Check the webhook URL in Telegram
   - Verify your application is running on the correct port
   - Ensure the route is correct

### Debugging

Enable detailed logging:

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Check webhook status:

```bash
curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getWebhookInfo"
```

### Manual Webhook Management

You can manually manage webhooks:

```csharp
// Get webhook info
var webhookInfo = await bot.GetWebhookInfo();

// Set webhook
await bot.SetWebhookAsync("https://yourdomain.com/TgBotPlay/");

// Delete webhook
await bot.DeleteWebhookAsync();
```

## Webhook vs Polling

| Feature | Webhook | Polling |
|---------|---------|---------|
| **Efficiency** | High | Low |
| **Real-time** | Yes | No |
| **Setup Complexity** | Medium | Low |
| **Production Ready** | Yes | Limited |
| **Resource Usage** | Low | High |
| **HTTPS Required** | Yes | No |
| **Public URL Required** | Yes | No |

## Best Practices

1. **Use HTTPS**: Always use HTTPS in production
2. **Monitor Health**: Set up health checks and monitoring
3. **Handle Errors**: Implement proper error handling
4. **Log Everything**: Log webhook events for debugging
5. **Test Thoroughly**: Test webhook setup in staging environment
6. **Backup Plan**: Have a polling fallback for critical bots
7. **Rate Limiting**: Implement rate limiting if needed
8. **Security**: Use proper authentication and authorization
