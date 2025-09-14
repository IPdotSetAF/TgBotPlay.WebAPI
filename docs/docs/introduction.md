# Introduction

TgBotPlay.WebAPI is a powerful .NET package that simplifies the creation of Telegram bots in ASP.NET Core WebAPI applications. It provides a unified interface for both **Polling** and **WebHook** workflows with minimal configuration and maximum flexibility.

## What is TgBotPlay.WebAPI?

TgBotPlay.WebAPI is designed to eliminate the complexity of setting up Telegram bots in .NET applications. Instead of manually configuring webhooks, handling updates, and managing bot lifecycle, TgBotPlay.WebAPI provides:

- **One-line setup** with dependency injection
- **Reflection-based handler discovery** - only implement what you need
- **Automatic webhook management** with health checks
- **Flexible configuration** with fluent API
- **Production-ready** error handling and logging

## Key Features

### Easy Setup
Register all bot services with a single extension method for `IServiceCollection`. No complex configuration required.

### Unified Handler Base
Implement your bot logic by inheriting from `TgBotPlayUpdateHandlerBase`. Only handle the events you care about - the framework automatically discovers and registers your handlers.

### Flexible Connection Methods
Switch between Polling and WebHook modes by changing a single option. No code changes required when switching between development and production environments.

### Automatic Event Subscription
Only the handler methods you implement are subscribed to Telegram events. No manual registration or complex event wiring needed.

### WebAPI Integration
Out-of-the-box API endpoints for Telegram WebHook with configurable controller routes and names.

### Health Checks
Built-in health checks for bot connectivity and webhook status monitoring.

### Customizable Options
Easily customize polling intervals, webhook refresh intervals, endpoint URLs, and security settings.

## Architecture Overview

TgBotPlay.WebAPI is built on top of the official [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library and integrates seamlessly with ASP.NET Core's dependency injection system.

### Core Components

- **TgBotPlayUpdateHandlerBase**: Base class for implementing bot logic
- **TgBotPlayOptions**: Fluent configuration API
- **TgBotPlayController**: Automatic webhook endpoint management
- **TgBotPlayWebHookService**: Background service for webhook lifecycle
- **PollingService**: Background service for polling updates
- **TgBotPlayHealthCheck**: Health monitoring integration

### How It Works

1. **Reflection-based Handler Registration**: The framework scans your handler class for methods starting with "On" and automatically maps them to Telegram update types.

2. **WebHook Management**: When using webhook mode, the framework automatically registers webhook endpoints with Telegram and refreshes them at configured intervals.

3. **Automatic Dependency Injection**: All dependencies are provided via DI. Simply inject what you need in your handler or controllers.

4. **Proper Scope Management**: The framework creates and manages proper scopes for both webhook and polling workflows.

## Supported .NET Versions

- .NET 6.0+
- .NET 7.0+
- .NET 8.0+
- .NET 9.0+

## Dependencies

- [Telegram.Bot](https://www.nuget.org/packages/Telegram.Bot/) - Official Telegram Bot API client
- ASP.NET Core WebAPI
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Logging

## License

This project is open source and available under the [LGPL-2.1 License](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/blob/main/LICENSE).

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/blob/main/CONTRIBUTING.md) for details on how to contribute to this project.

## Support

- [Documentation](https://github.com/IPdotSetAF/TgBotPlay.WebAPI)
- [Issue Tracker](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/issues)
- [Discussions](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/discussions)