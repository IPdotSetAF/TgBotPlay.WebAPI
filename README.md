# TgBotPlay.WebAPI

<img src="logo/logo-shape.svg" width=150>

A package for effortlessly bootstrapping Telegram Bots in .NET WebAPI projects.  
Supports both **Polling** and **WebHook** workflows with a unified interface and minimal configuration.

---

## Features

- **Easy Setup**: Register all bot services with a single extension method for `IServiceCollection`.
- **Unified Handler Base**: Implement your bot logic by inheriting from `TgBotPlayUpdateHandlerBase`, handling only the events you care about.
- **Flexible Connection Methods**: Switch between Polling and WebHook modes by changing an option—no code changes required.
- **Automatic Reflection-based Event Subscription**: Only the handler methods you implement are subscribed—no manual registration.
- **WebAPI Integration**: Out-of-the-box API endpoints for Telegram WebHook, configurable controller route and name.
- **Health Checks**: (If implemented in your project) for bot and webhook status.
- **Customizable Options**: Easily customize polling intervals, webhook refresh intervals, endpoint URLs, and security.

---

## Installation

Install the NuGet package:

```sh
dotnet add package TgBotPlay.WebAPI
```

---

## Quick Start

### 1. Implement Your Bot Logic

Create a class that inherits from `TgBotPlayUpdateHandlerBase` and override only the update handlers you need:
```csharp
public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    public MyBotHandler(ILogger<MyBotHandler> logger) : base(logger) { }

    protected async Task OnMessage(Message message)
    {
        // Handle incoming messages
    }

    protected async Task OnCallbackQuery(CallbackQuery query)
    {
        // Handle callback queries
    }
}
```

Available handler methods include:
- `OnMessage(Message message)`
- `OnEditedMessage(Message message)`
- `OnChannelPost(Message message)`
- `OnEditedChannelPost(Message message)`
- `OnBusinessConnection(BusinessConnection connection)`
- `OnBusinessMessage(BusinessMessage message)`
- `OnEditedBusinessMessage(BusinessMessage message)`
- `OnDeletedBusinessMessages(DeletedBusinessMessages messages)`
- `OnMessageReaction(MessageReaction reaction)`
- `OnMessageReactionCount(MessageReactionCount reactionCount)`
- `OnInlineQuery(InlineQuery query)`
- (and more—see base class for the full list)

---

### 2. Register TgBotPlay in your `Program.cs` or `Startup.cs`

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("telegram bot token")
    .AsPollingClient()
    // Customize other options as needed
);
```

---

## WebAPI Routing 

TgBotPlay automatically sets up controller endpoints for receiving updates:

- `POST TgBotPlay/` — Main webhook endpoint for Telegram updates
- `POST TgBotPlay/HookUp` — Start webhook service
- `POST TgBotPlay/HookDown` — Stop webhook service

The controller route and endpoint name can be customized using `WithController(...)` in options (Defaults to `TgBotPlay`).

---

## Switching Between Polling and WebHook

- **Polling**: `.AsPollingClient()`
- **Webhook**: `.AsWebHookClient("<host name(ex. https://xyz.com)>")`

No other code changes are required!

---

## Advanced Configuration

- `WithController(string name, string template)` — Customize controller name and route template.
- `WithPollingInterval(double seconds = 5d)` — Set polling interval in seconds.
- `WithWebHookRefreshInterval(double minutes = 60d)` — Set how often webhook is refreshed.
- `SetDropPendingUpdates(bool dropPendingChanges = false)` — Set how pending messages are processed when bot starts.

---

## Health Checks

You can simply add TgBotPlay Health Checks like this in your `Program.cs` or `Startup.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth();
```

---

## How It Works

- **Reflection-based Handler Registration**:  
  Only the update handler methods you implement in your handler class are registered for Telegram events.
- **WebHook Management**:  
  WebHook endpoints are exposed via ASP.NET controllers and can be invoked for health and lifecycle management.
- **Automatic Dependency Injection**:  
  All dependencies are provided via DI; simply inject what you need in your handler or controllers.
- **Proper Scope Management**:
  Proper scopes are created and managed for webhook and polling workflows internally.

---

## Example

### Webhook

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("telegram bot token")
    .AsWebHookClient("host name(ex. https://xyz.com)")
    .WithController("TgBotPlay", "TgBotPlay/[action]")
    .WithWebHookRefreshInterval(30) //minuets
);
```

### Polling

```csharp
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("telegram bot token")
    .AsPollingClient()
    .WithPollingInterval(5) //seconds
);
```

---

## Contribution

- You can open Issues for any bug report or feature request.
- You are free to contribute to this project by following these steps:
   1. Fork this Repo.
   2. Create a new branch for your feature/bugfix in your forked Repo.
   3. Commit your changes to the new branch you just made.
   4. Create a pull request from your branch into the `main` branch of This Repo([https://github.com/IPdotSetAF/TgBotPlay.WebAPI](https://github.com/IPdotSetAF/TgBotPlay.WebAPI)).

---

## Credits

- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)  
- Built and maintained by [IPdotSetAF](https://github.com/IPdotSetAF)
