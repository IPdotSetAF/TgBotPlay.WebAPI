---
_layout: landing
---

# TgBotPlay.WebAPI

<div style="text-align: center; margin: 2rem 0;">
  <img src="images/logo.svg" alt="TgBotPlay.WebAPI Logo" width="200" style="margin-bottom: 1rem;">
</div>

<div style="text-align: center; font-size: 1.2rem; color: #666; margin-bottom: 2rem;">
  Effortlessly bootstrap Telegram Bots in .NET WebAPI projects
</div>

<div style="text-align: center; margin: 2rem 0;">
  <a href="https://www.nuget.org/packages/TgBotPlay.WebAPI" style="display: inline-block; background: #0078d4; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; margin: 0 10px; font-weight: bold;">
    📦 Install NuGet Package
  </a>
  <a href="https://github.com/IPdotSetAF/TgBotPlay.WebAPI" style="display: inline-block; background: #24292e; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; margin: 0 10px; font-weight: bold;">
    🐙 View on GitHub
  </a>
</div>

---

## 🚀 Quick Start

Get your Telegram bot running in minutes with just a few lines of code:

```csharp
// 1. Install the package
// dotnet add package TgBotPlay.WebAPI

// 2. Create your bot handler
public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;
    
    public MyBotHandler(ITelegramBotClient bot, ILogger<MyBotHandler> logger) : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        await _bot.SendTextMessageAsync(message.Chat.Id, $"Echo: {message.Text}");
    }
}

// 3. Register in Program.cs
builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
);
```

[**Get Started →**](docs/getting-started.md)

---

## ✨ Key Features

<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1.5rem; margin: 2rem 0;">

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">🎯 Unified Handler Base</h3>
  <p>Implement your bot logic by inheriting from <code>TgBotPlayUpdateHandlerBase</code>. Only handle the events you care about - the framework automatically discovers and registers your handlers.</p>
</div>

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">🔄 Flexible Connection Methods</h3>
  <p>Switch between <strong>Polling</strong> and <strong>WebHook</strong> modes by changing a single option. No code changes required when switching between development and production environments.</p>
</div>

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">🌐 WebAPI Integration</h3>
  <p>Out-of-the-box API endpoints for Telegram WebHook with configurable controller routes and names. Perfect for ASP.NET Core WebAPI applications.</p>
</div>

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">🏥 Health Checks</h3>
  <p>Built-in health checks for bot connectivity and webhook status monitoring. Keep your bot healthy and monitor its performance.</p>
</div>

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">⚙️ Easy Configuration</h3>
  <p>Fluent API for configuration. Easily customize polling intervals, webhook refresh intervals, endpoint URLs, and security settings.</p>
</div>

<div style="border: 1px solid #e1e5e9; border-radius: 8px; padding: 1.5rem;">
  <h3 style="margin-top: 0; color: #0078d4;">🔍 Automatic Discovery</h3>
  <p>Reflection-based handler registration. Only the handler methods you implement are subscribed to Telegram events. No manual registration needed.</p>
</div>

</div>

---

## 📚 Documentation

<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1rem; margin: 2rem 0;">

<div style="border-left: 4px solid #0078d4; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/getting-started.md">🚀 Getting Started</a></h4>
  <p style="margin-bottom: 0; color: #666;">Install, configure, and run your first bot in minutes.</p>
</div>

<div style="border-left: 4px solid #28a745; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/configuration.md">⚙️ Configuration</a></h4>
  <p style="margin-bottom: 0; color: #666;">Complete guide to all configuration options and settings.</p>
</div>

<div style="border-left: 4px solid #ffc107; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/handlers.md">🎯 Update Handlers</a></h4>
  <p style="margin-bottom: 0; color: #666;">Learn how to implement handlers for different update types.</p>
</div>

<div style="border-left: 4px solid #dc3545; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/webhook.md">🌐 Webhook Setup</a></h4>
  <p style="margin-bottom: 0; color: #666;">Configure webhooks for production environments.</p>
</div>

<div style="border-left: 4px solid #6f42c1; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/polling.md">📡 Polling Setup</a></h4>
  <p style="margin-bottom: 0; color: #666;">Set up polling for development and simple deployments.</p>
</div>

<div style="border-left: 4px solid #17a2b8; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/examples.md">💡 Examples</a></h4>
  <p style="margin-bottom: 0; color: #666;">Practical examples for common bot scenarios.</p>
</div>

<div style="border-left: 4px solid #fd7e14; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/api-reference.md">📖 API Reference</a></h4>
  <p style="margin-bottom: 0; color: #666;">Complete API documentation and reference.</p>
</div>

<div style="border-left: 4px solid #6c757d; padding-left: 1rem;">
  <h4 style="margin-top: 0;"><a href="docs/troubleshooting.md">🔧 Troubleshooting</a></h4>
  <p style="margin-bottom: 0; color: #666;">Common issues and their solutions.</p>
</div>

</div>

---

## 🎯 Why Choose TgBotPlay.WebAPI?

<div style="display: flex; align-items: center; margin: 2rem 0; padding: 1.5rem; background: #f8f9fa; border-radius: 8px;">

<div style="flex: 1;">
  <h3 style="margin-top: 0;">Built for .NET Developers</h3>
  <p>TgBotPlay.WebAPI is designed specifically for .NET developers who want to build Telegram bots quickly and efficiently. It integrates seamlessly with ASP.NET Core's dependency injection system and follows .NET best practices.</p>
  
  <ul style="margin-bottom: 0;">
    <li>✅ <strong>Zero Configuration</strong> - Get started with minimal setup</li>
    <li>✅ <strong>Production Ready</strong> - Built for real-world applications</li>
    <li>✅ <strong>Well Tested</strong> - Comprehensive test coverage</li>
    <li>✅ <strong>Active Development</strong> - Regular updates and improvements</li>
  </ul>
</div>

</div>

---

## 🚀 Supported .NET Versions

<div style="text-align: center; margin: 2rem 0;">

<div style="display: inline-flex; gap: 1rem; flex-wrap: wrap; justify-content: center; align-items: center;">

<div style="background: #512bd4; color: white; padding: 8px 16px; border-radius: 20px; font-weight: bold;">
  .NET 6.0+
</div>

<div style="background: #512bd4; color: white; padding: 8px 16px; border-radius: 20px; font-weight: bold;">
  .NET 7.0+
</div>

<div style="background: #512bd4; color: white; padding: 8px 16px; border-radius: 20px; font-weight: bold;">
  .NET 8.0+
</div>

<div style="background: #512bd4; color: white; padding: 8px 16px; border-radius: 20px; font-weight: bold;">
  .NET 9.0+
</div>

</div>

</div>

---

## 🤝 Community & Support

<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; margin: 2rem 0;">

<div style="text-align: center; padding: 1rem;">
  <div style="font-size: 2rem; margin-bottom: 0.5rem;">🐛</div>
  <h4 style="margin-top: 0;"><a href="https://github.com/IPdotSetAF/TgBotPlay.WebAPI/issues">Report Issues</a></h4>
  <p style="margin-bottom: 0; color: #666; font-size: 0.9rem;">Found a bug? Let us know!</p>
</div>

<div style="text-align: center; padding: 1rem;">
  <div style="font-size: 2rem; margin-bottom: 0.5rem;">💬</div>
  <h4 style="margin-top: 0;"><a href="https://github.com/IPdotSetAF/TgBotPlay.WebAPI/discussions">Discussions</a></h4>
  <p style="margin-bottom: 0; color: #666; font-size: 0.9rem;">Ask questions and share ideas</p>
</div>

<div style="text-align: center; padding: 1rem;">
  <div style="font-size: 2rem; margin-bottom: 0.5rem;">📖</div>
  <h4 style="margin-top: 0;"><a href="https://github.com/IPdotSetAF/TgBotPlay.WebAPI">Documentation</a></h4>
  <p style="margin-bottom: 0; color: #666; font-size: 0.9rem;">Complete documentation and guides</p>
</div>

<div style="text-align: center; padding: 1rem;">
  <div style="font-size: 2rem; margin-bottom: 0.5rem;">🤝</div>
  <h4 style="margin-top: 0;"><a href="https://github.com/IPdotSetAF/TgBotPlay.WebAPI/blob/main/CONTRIBUTING.md">Contribute</a></h4>
  <p style="margin-bottom: 0; color: #666; font-size: 0.9rem;">Help improve the project</p>
</div>

</div>

---

## 📄 License

This project is open source and available under the [MIT License](https://github.com/IPdotSetAF/TgBotPlay.WebAPI/blob/main/LICENSE).

---

<div style="text-align: center; margin: 3rem 0; padding: 2rem; background: #f8f9fa; border-radius: 8px;">

<h3 style="margin-top: 0;">Ready to build your Telegram bot?</h3>
<p style="margin-bottom: 1.5rem; color: #666;">Get started with TgBotPlay.WebAPI today and build amazing Telegram bots in minutes!</p>

<a href="docs/getting-started.md" style="display: inline-block; background: #0078d4; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 1.1rem;">
  🚀 Start Building Now
</a>

</div>