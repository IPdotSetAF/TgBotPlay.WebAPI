# API Reference

This document provides a comprehensive reference for all TgBotPlay.WebAPI classes, methods, and configuration options.

## Namespaces

- `TgBotPlay.WebAPI` - Main namespace containing core classes
- `TgBotPlay.WebAPI.Polling` - Polling-related classes
- `TgBotPlay.WebAPI.WebHook` - Webhook-related classes
- `TgBotPlay.WebAPI.Utils` - Utility classes

## Core Classes

### TgBotPlayUpdateHandlerBase

Base class for implementing Telegram bot update handlers.

```csharp
public abstract class TgBotPlayUpdateHandlerBase : IUpdateHandler
```

#### Constructor

```csharp
protected TgBotPlayUpdateHandlerBase(ILogger logger)
```

**Parameters:**
- `logger` (ILogger): The logger instance to use for logging

#### Methods

##### HandleUpdateAsync

```csharp
public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
```

Handles incoming updates from Telegram.

**Parameters:**
- `botClient` (ITelegramBotClient): The bot client instance
- `update` (Update): The update to handle
- `cancellationToken` (CancellationToken): The cancellation token

##### HandleErrorAsync

```csharp
public virtual async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
```

Handles errors that occur during update processing.

**Parameters:**
- `botClient` (ITelegramBotClient): The bot client instance
- `exception` (Exception): The exception that occurred
- `source` (HandleErrorSource): The source of the error
- `cancellationToken` (CancellationToken): The cancellation token

##### GetImplementedUpdateTypes

```csharp
public IEnumerable<UpdateType> GetImplementedUpdateTypes()
```

Gets the collection of update types that have implemented handlers.

**Returns:** An enumerable of implemented update types

#### Protected Methods

All handler methods are protected and should be overridden in derived classes:

- `protected Task OnMessage(Message message)`
- `protected Task OnEditedMessage(Message message)`
- `protected Task OnChannelPost(Message message)`
- `protected Task OnEditedChannelPost(Message message)`
- `protected Task OnBusinessConnection(BusinessConnection connection)`
- `protected Task OnBusinessMessage(BusinessMessage message)`
- `protected Task OnEditedBusinessMessage(BusinessMessage message)`
- `protected Task OnDeletedBusinessMessages(DeletedBusinessMessages messages)`
- `protected Task OnMessageReaction(MessageReaction reaction)`
- `protected Task OnMessageReactionCount(MessageReactionCount reactionCount)`
- `protected Task OnInlineQuery(InlineQuery query)`
- `protected Task OnChosenInlineResult(ChosenInlineResult result)`
- `protected Task OnCallbackQuery(CallbackQuery query)`
- `protected Task OnShippingQuery(ShippingQuery query)`
- `protected Task OnPreCheckoutQuery(PreCheckoutQuery query)`
- `protected Task OnPurchasedPaidMedia(PurchasedPaidMedia media)`
- `protected Task OnPoll(Poll poll)`
- `protected Task OnPollAnswer(PollAnswer answer)`
- `protected Task OnMyChatMember(ChatMember member)`
- `protected Task OnChatMember(ChatMember member)`
- `protected Task OnChatJoinRequest(ChatJoinRequest request)`
- `protected Task OnChatBoost(ChatBoost boost)`
- `protected Task OnRemovedChatBoost(RemovedChatBoost boost)`

### TgBotPlayOptions

Configuration options for TgBotPlay WebAPI integration.

```csharp
public class TgBotPlayOptions
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Token` | `string?` | `null` | Telegram bot token (required) |
| `Host` | `string?` | `null` | Host URL for webhook configuration |
| `ConnectionMethod` | `TgBotPlayConnectionMethod` | `POLLING` | Connection method |
| `ControllerRouteTemplate` | `string` | `"TgBotPlay/[action]"` | Route template for controller actions |
| `ControllerName` | `string` | `"TgBotPlay"` | Name of the controller |
| `PollingSeconds` | `double` | `5.0` | Polling interval in seconds |
| `WebHookRefreshMinuets` | `double` | `60.0` | Webhook refresh interval in minutes |
| `DropPendingUpdates` | `bool` | `false` | Whether to drop pending updates on startup |
| `Secret` | `string?` | `null` | Secret token for webhook authentication |
| `WebHookUrl` | `string` | Computed | Complete webhook URL (read-only) |

#### Methods

##### WithToken

```csharp
public TgBotPlayOptions WithToken(string token)
```

Sets the Telegram bot token and generates the secret for webhook authentication.

**Parameters:**
- `token` (string): The Telegram bot token

**Returns:** The current instance for method chaining

**Throws:** `ArgumentNullException` if token is null or empty

##### AsPollingClient

```csharp
public TgBotPlayOptions AsPollingClient()
```

Configures the bot to use polling for receiving updates.

**Returns:** The current instance for method chaining

##### AsWebHookClient

```csharp
public TgBotPlayOptions AsWebHookClient(string host)
```

Configures the bot to use webhook for receiving updates.

**Parameters:**
- `host` (string): The host URL where the webhook will be registered

**Returns:** The current instance for method chaining

**Throws:** `ArgumentNullException` if host is null or empty

##### WithController

```csharp
public TgBotPlayOptions WithController(string name, string template)
```

Configures the controller name and route template.

**Parameters:**
- `name` (string): The name of the controller
- `template` (string): The route template that must contain '[action]' placeholder

**Returns:** The current instance for method chaining

**Throws:** `ArgumentNullException` if name or template is null/empty, or template doesn't contain '[action]'

##### WithPollingInterval

```csharp
public TgBotPlayOptions WithPollingInterval(double seconds = 5d)
```

Sets the polling interval for receiving updates.

**Parameters:**
- `seconds` (double): The polling interval in seconds (default: 5)

**Returns:** The current instance for method chaining

##### WithWebHookRefreshInterval

```csharp
public TgBotPlayOptions WithWebHookRefreshInterval(double minuets = 60d)
```

Sets the webhook refresh interval.

**Parameters:**
- `minuets` (double): The refresh interval in minutes (default: 60)

**Returns:** The current instance for method chaining

##### SetDropPendingUpdates

```csharp
public TgBotPlayOptions SetDropPendingUpdates(bool dropPendingChanges = false)
```

Sets whether to drop pending updates on startup.

**Parameters:**
- `dropPendingChanges` (bool): True to drop pending updates, false otherwise (default: false)

**Returns:** The current instance for method chaining

### TgBotPlayConnectionMethod

Specifies the connection method used for receiving Telegram bot updates.

```csharp
public enum TgBotPlayConnectionMethod
```

#### Values

- `POLLING` - Uses polling to periodically check for new updates
- `WEB_HOOK` - Uses webhook to receive updates directly from Telegram

## Extension Methods

### TgBotPlayExtensions

Extension methods for configuring TgBotPlay WebAPI services.

#### AddTgBotPlay

```csharp
public static IServiceCollection AddTgBotPlay<TUpdateHandler>(
    this IServiceCollection services, 
    Action<TgBotPlayOptions> options) 
    where TUpdateHandler : TgBotPlayUpdateHandlerBase
```

Adds TgBotPlay WebAPI services to the specified `IServiceCollection`.

**Parameters:**
- `services` (IServiceCollection): The service collection to add services to
- `options` (Action<TgBotPlayOptions>): The configuration action for options

**Returns:** The service collection so that additional calls can be chained

**Type Parameters:**
- `TUpdateHandler`: The type of update handler that implements `TgBotPlayUpdateHandlerBase`

#### AddTgBotPlayHealth

```csharp
public static IHealthChecksBuilder AddTgBotPlayHealth(
    this IHealthChecksBuilder builder, 
    string name = "TgBotPlay")
```

Adds TgBotPlay health check to the specified `IHealthChecksBuilder`.

**Parameters:**
- `builder` (IHealthChecksBuilder): The health checks builder to add the health check to
- `name` (string): The name of the health check (default: "TgBotPlay")

**Returns:** The health checks builder so that additional calls can be chained

## Controller Classes

### TgBotPlayController

Controller for handling Telegram bot updates via webhook.

```csharp
[AllowAnonymous]
[ServiceFilter(typeof(TgBotPlayAuthorizationFilter))]
public class TgBotPlayController : ControllerBase
```

#### Constructor

```csharp
public TgBotPlayController(
    ITelegramBotClient bot,
    IOptions<TgBotPlayOptions> options,
    TgBotPlayUpdateHandlerBase updateHandler,
    ITgBotPlayWebHookService webHookService)
```

#### Actions

##### Update

```csharp
[HttpPost]
public async Task<IActionResult> Update(
    [FromBody] Update update,
    CancellationToken cancellationToken)
```

Handles incoming Telegram bot updates via webhook.

**Parameters:**
- `update` (Update): The Telegram update received from the webhook
- `cancellationToken` (CancellationToken): The cancellation token

**Returns:** An `IActionResult` indicating success

##### HookUp

```csharp
[HttpPost("HookUp")]
public async Task<IActionResult> HookUp(CancellationToken cancellationToken)
```

Starts the webhook service to begin receiving updates.

**Parameters:**
- `cancellationToken` (CancellationToken): The cancellation token

**Returns:** An `IActionResult` indicating success

##### HookDown

```csharp
[HttpPost("HookDown")]
public async Task<IActionResult> HookDown(CancellationToken cancellationToken)
```

Stops the webhook service and removes the webhook from Telegram.

**Parameters:**
- `cancellationToken` (CancellationToken): The cancellation token

**Returns:** An `IActionResult` indicating success

## Background Services

### TgBotPlayWebHookService

Background service that manages the Telegram bot webhook lifecycle.

```csharp
public class TgBotPlayWebHookService : BackgroundService, ITgBotPlayWebHookService
```

#### Constructor

```csharp
public TgBotPlayWebHookService(
    IServiceProvider serviceProvider,
    ILogger<TgBotPlayWebHookService> logger,
    IOptions<TgBotPlayOptions> options)
```

#### Methods

##### Start

```csharp
public void Start()
```

Starts the webhook service manually.

**Throws:** `InvalidOperationException` when the service is already running

##### Stop

```csharp
public async Task Stop()
```

Stops the webhook service and removes the webhook from Telegram.

**Returns:** A task representing the asynchronous operation

**Throws:** `InvalidOperationException` when the service is already stopped

### PollingService

Background service that handles Telegram bot polling for receiving updates.

```csharp
public class PollingService : PollingServiceBase<ReceiverService>
```

#### Constructor

```csharp
public PollingService(
    IServiceProvider serviceProvider,
    ILogger<PollingService> logger,
    IOptions<TgBotPlayOptions> options)
```

## Health Checks

### TgBotPlayHealthCheck

Health check implementation for TgBotPlay WebAPI integration.

```csharp
public class TgBotPlayHealthCheck : IHealthCheck
```

#### Constructor

```csharp
public TgBotPlayHealthCheck(
    ITelegramBotClient bot,
    IOptions<TgBotPlayOptions> options)
```

#### Methods

##### CheckHealthAsync

```csharp
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
```

Performs the health check by verifying bot connectivity and webhook configuration.

**Parameters:**
- `context` (HealthCheckContext): The health check context
- `cancellationToken` (CancellationToken): The cancellation token

**Returns:** A `HealthCheckResult` indicating the health status

## Interfaces

### ITgBotPlayWebHookService

Interface for webhook service management.

```csharp
public interface ITgBotPlayWebHookService
```

#### Methods

- `void Start()`
- `Task Stop()`

## Utility Classes

### TgBotPlayAuthorizationFilter

Authorization filter for webhook endpoints.

```csharp
public class TgBotPlayAuthorizationFilter : IAuthorizationFilter
```

### ControllerRoutingConvention

Convention for controller routing.

```csharp
public class ControllerRoutingConvention : IControllerModelConvention
```

## Error Handling

### HandleErrorSource

Specifies the source of an error in the update handling process.

```csharp
public enum HandleErrorSource
```

#### Values

- `HandleUpdateError` - Error occurred while handling an update
- `ReceiveError` - Error occurred while receiving updates
- `GeneralError` - General error

## Configuration Examples

### Basic Polling

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(5)
);
```

### Basic Webhook

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithWebHookRefreshInterval(60)
);
```

### Custom Controller

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("TelegramBot", "api/telegram/[action]")
);
```

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth("MyBot");
```

## Dependencies

TgBotPlay.WebAPI depends on the following packages:

- `Telegram.Bot` - Official Telegram Bot API client
- `Microsoft.Extensions.DependencyInjection` - Dependency injection
- `Microsoft.Extensions.Hosting` - Background services
- `Microsoft.Extensions.Logging` - Logging
- `Microsoft.Extensions.Options` - Configuration
- `Microsoft.AspNetCore.Mvc` - Web API controllers
- `Microsoft.Extensions.Diagnostics.HealthChecks` - Health checks
