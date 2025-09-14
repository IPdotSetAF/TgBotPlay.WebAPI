# Troubleshooting

This guide helps you diagnose and resolve common issues when using TgBotPlay.WebAPI.

## Common Issues

### Bot Not Responding

**Symptoms:**
- Bot doesn't respond to messages
- No logs showing received updates
- Bot appears offline

**Possible Causes:**
1. Incorrect bot token
2. Bot not running
3. Network connectivity issues
4. Configuration errors

**Solutions:**

1. **Verify Bot Token**
   ```csharp
   // Test bot token
   var bot = new TelegramBotClient("YOUR_BOT_TOKEN");
   var me = await bot.GetMe();
   Console.WriteLine($"Bot: @{me.Username}");
   ```

2. **Check Configuration**
   ```csharp
   // Ensure proper configuration
   builder.Services.AddTgBotPlay<MyBotHandler>(options => options
       .WithToken("YOUR_BOT_TOKEN") // Make sure this is correct
       .AsPollingClient() // or .AsWebHookClient("https://yourdomain.com")
   );
   ```

3. **Enable Debug Logging**
   ```csharp
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```

4. **Check Network Connectivity**
   ```bash
   # Test if you can reach Telegram API
   curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getMe"
   ```

### Webhook Issues

**Symptoms:**
- Webhook not receiving updates
- 404 errors in logs
- Webhook URL mismatch

**Possible Causes:**
1. HTTPS not configured
2. Webhook URL not accessible
3. Incorrect webhook URL
4. SSL certificate issues

**Solutions:**

1. **Verify HTTPS Configuration**
   ```csharp
   // Ensure HTTPS is properly configured
   builder.WebHost.ConfigureKestrel(options =>
   {
       options.ListenAnyIP(443, listenOptions =>
       {
           listenOptions.UseHttps("path/to/certificate.pfx", "password");
       });
   });
   ```

2. **Check Webhook URL**
   ```bash
   # Check current webhook info
   curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getWebhookInfo"
   ```

3. **Test Webhook Endpoint**
   ```bash
   # Test if your webhook endpoint is accessible
   curl -X POST "https://yourdomain.com/TgBotPlay/" \
        -H "Content-Type: application/json" \
        -d '{"update_id": 1, "message": {"message_id": 1, "from": {"id": 1, "is_bot": false, "first_name": "Test"}, "chat": {"id": 1, "first_name": "Test", "type": "private"}, "date": 1234567890, "text": "test"}}'
   ```

4. **Set Webhook Manually**
   ```bash
   # Set webhook manually
   curl -X POST "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/setWebhook" \
        -H "Content-Type: application/json" \
        -d '{"url": "https://yourdomain.com/TgBotPlay/"}'
   ```

### Handler Not Called

**Symptoms:**
- Bot receives updates but handlers aren't called
- No logs from handler methods
- Updates are received but not processed

**Possible Causes:**
1. Incorrect method names
2. Method not properly overridden
3. Handler not registered
4. Exception in handler

**Solutions:**

1. **Check Method Names**
   ```csharp
   // Method names must start with "On" and match update types
   protected async Task OnMessage(Message message) // ✓ Correct
   protected async Task OnCallbackQuery(CallbackQuery query) // ✓ Correct
   
   // These won't work:
   protected async Task HandleMessage(Message message) // ✗ Wrong name
   protected async Task OnCustomUpdate(Message message) // ✗ Wrong name
   ```

2. **Verify Method Override**
   ```csharp
   public class MyBotHandler : TgBotPlayUpdateHandlerBase
   {
       // Make sure methods are protected and async
       protected async Task OnMessage(Message message)
       {
           // Your implementation
       }
   }
   ```

3. **Check Handler Registration**
   ```csharp
   // Ensure handler is properly registered
   builder.Services.AddTgBotPlay<MyBotHandler>(options => options
       .WithToken("YOUR_BOT_TOKEN")
       .AsPollingClient()
   );
   ```

4. **Add Exception Handling**
   ```csharp
   protected async Task OnMessage(Message message)
   {
       try
       {
           // Your handler logic
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error in OnMessage handler");
       }
   }
   ```

### Polling Issues

**Symptoms:**
- Polling service not starting
- High resource usage
- Missing updates

**Possible Causes:**
1. Polling interval too short
2. Network issues
3. Bot token issues
4. Service not registered

**Solutions:**

1. **Adjust Polling Interval**
   ```csharp
   // Use appropriate polling interval
   builder.Services.AddTgBotPlay<MyBotHandler>(options => options
       .WithToken("YOUR_BOT_TOKEN")
       .AsPollingClient()
       .WithPollingInterval(5) // 5 seconds is usually good
   );
   ```

2. **Check Service Registration**
   ```csharp
   // Ensure polling service is registered
   builder.Services.AddTgBotPlay<MyBotHandler>(options => options
       .WithToken("YOUR_BOT_TOKEN")
       .AsPollingClient() // This registers the polling service
   );
   ```

3. **Monitor Resource Usage**
   ```csharp
   // Add logging to monitor polling
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Information);
   ```

### Health Check Issues

**Symptoms:**
- Health checks failing
- Bot appears unhealthy
- Monitoring alerts

**Possible Causes:**
1. Bot connectivity issues
2. Webhook configuration problems
3. Health check not registered
4. Network issues

**Solutions:**

1. **Register Health Checks**
   ```csharp
   builder.Services.AddHealthChecks()
       .AddTgBotPlayHealth();
   ```

2. **Check Health Check Endpoint**
   ```bash
   # Test health check endpoint
   curl "https://yourdomain.com/health"
   ```

3. **Enable Health Check Logging**
   ```csharp
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```

## Debugging Techniques

### Enable Detailed Logging

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add specific logging for TgBotPlay
builder.Logging.AddFilter("TgBotPlay", LogLevel.Debug);
```

### Check Bot Status

```csharp
// Add a controller to check bot status
[ApiController]
[Route("api/[controller]")]
public class BotStatusController : ControllerBase
{
    private readonly ITelegramBotClient _bot;

    public BotStatusController(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var me = await _bot.GetMe();
            return Ok(new { Status = "Connected", Bot = me.Username });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "Error", Message = ex.Message });
        }
    }
}
```

### Test Webhook Manually

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ITelegramBotClient _bot;

    public TestController(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    [HttpPost("test-webhook")]
    public async Task<IActionResult> TestWebhook()
    {
        try
        {
            var webhookInfo = await _bot.GetWebhookInfo();
            return Ok(webhookInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

## Performance Issues

### High Memory Usage

**Symptoms:**
- Memory usage keeps growing
- Application becomes slow
- Out of memory errors

**Solutions:**

1. **Check for Memory Leaks**
   ```csharp
   // Dispose of resources properly
   protected async Task OnMessage(Message message)
   {
       using var scope = _serviceProvider.CreateScope();
       var service = scope.ServiceProvider.GetRequiredService<IMyService>();
       // Use service
   }
   ```

2. **Monitor Memory Usage**
   ```csharp
   // Add memory monitoring
   builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
   ```

### High CPU Usage

**Symptoms:**
- CPU usage is high
- Application is slow
- System becomes unresponsive

**Solutions:**

1. **Optimize Polling Interval**
   ```csharp
   // Use appropriate polling interval
   .WithPollingInterval(10) // Increase if CPU usage is high
   ```

2. **Optimize Handler Logic**
   ```csharp
   // Use efficient algorithms and data structures
   protected async Task OnMessage(Message message)
   {
       // Avoid expensive operations in handlers
       // Use caching where appropriate
       // Process messages asynchronously
   }
   ```

## Security Issues

### Unauthorized Webhook Access

**Symptoms:**
- Unauthorized requests to webhook
- Security warnings in logs
- Bot receiving spam

**Solutions:**

1. **Verify Webhook Security**
   ```csharp
   // TgBotPlay automatically generates secret tokens
   // Make sure your webhook URL is secure
   ```

2. **Add IP Whitelisting**
   ```csharp
   // Add custom authorization filter
   public class TelegramIpFilter : IAuthorizationFilter
   {
       public void OnAuthorization(AuthorizationFilterContext context)
       {
           var clientIp = context.HttpContext.Connection.RemoteIpAddress;
           // Check if IP is from Telegram's range
       }
   }
   ```

### Bot Token Security

**Symptoms:**
- Bot token exposed in logs
- Unauthorized bot access
- Security warnings

**Solutions:**

1. **Use Environment Variables**
   ```csharp
   var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
   ```

2. **Use User Secrets (Development)**
   ```csharp
   // In appsettings.Development.json
   {
     "TelegramBot": {
       "Token": "your_bot_token_here"
     }
   }
   ```

3. **Use Azure Key Vault (Production)**
   ```csharp
   builder.Configuration.AddAzureKeyVault(
       "https://your-keyvault.vault.azure.net/",
       new DefaultAzureCredential());
   ```

## Getting Help

### Enable Debug Logging

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Check Telegram Bot API Status

```bash
# Check if Telegram API is working
curl "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getMe"
```

### Common Error Messages

| Error | Cause | Solution |
|-------|-------|----------|
| `ArgumentNullException: token can not be null or empty` | Bot token not provided | Provide valid bot token |
| `ArgumentNullException: host can not be null or empty` | Webhook host not provided | Provide webhook host URL |
| `ArgumentNullException: Controller template must contain '[action]'` | Invalid controller template | Use template with '[action]' placeholder |
| `InvalidOperationException: Service is already running` | Service already started | Check if service is already running |
| `ApiRequestException: 401 Unauthorized` | Invalid bot token | Check bot token |
| `ApiRequestException: 400 Bad Request` | Invalid webhook URL | Check webhook URL format |

### Contact Support

If you're still experiencing issues:

1. Check the [GitHub Issues](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/issues)
2. Create a new issue with:
   - Description of the problem
   - Steps to reproduce
   - Logs and error messages
   - Configuration details
3. Join the [Discussions](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/discussions) for community help