# Examples

This section provides practical examples of using TgBotPlay.WebAPI for common bot scenarios.

## Basic Echo Bot

A simple bot that echoes back any message it receives.

```csharp
using TgBotPlay.WebAPI;
using Telegram.Bot;
using Telegram.Bot.Types;

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

## Command-Based Bot

A bot that responds to specific commands.

```csharp
public class CommandBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public CommandBotHandler(ITelegramBotClient bot, ILogger<CommandBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text is not null)
        {
            var command = message.Text.Split(' ')[0].ToLower();
            
            switch (command)
            {
                case "/start":
                    await HandleStartCommand(message);
                    break;
                case "/help":
                    await HandleHelpCommand(message);
                    break;
                case "/info":
                    await HandleInfoCommand(message);
                    break;
                default:
                    await HandleUnknownCommand(message);
                    break;
            }
        }
    }

    private async Task HandleStartCommand(Message message)
    {
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome! Use /help to see available commands."
        );
    }

    private async Task HandleHelpCommand(Message message)
    {
        var helpText = """
            Available commands:
            /start - Start the bot
            /help - Show this help
            /info - Show bot information
            """;
            
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: helpText
        );
    }

    private async Task HandleInfoCommand(Message message)
    {
        var info = $"Chat ID: {message.Chat.Id}\n" +
                  $"User ID: {message.From?.Id}\n" +
                  $"Username: @{message.From?.Username}";
                  
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: info
        );
    }

    private async Task HandleUnknownCommand(Message message)
    {
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Unknown command. Use /help to see available commands."
        );
    }
}
```

## Inline Keyboard Bot

A bot that uses inline keyboards for user interaction.

```csharp
public class InlineKeyboardBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public InlineKeyboardBotHandler(ITelegramBotClient bot, ILogger<InlineKeyboardBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/start")
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Option 1", "opt1") },
                new[] { InlineKeyboardButton.WithCallbackData("Option 2", "opt2") },
                new[] { InlineKeyboardButton.WithCallbackData("Option 3", "opt3") }
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
        
        var response = query.Data switch
        {
            "opt1" => "You chose Option 1!",
            "opt2" => "You chose Option 2!",
            "opt3" => "You chose Option 3!",
            _ => "Unknown option selected."
        };

        await _bot.SendTextMessageAsync(
            chatId: query.Message.Chat.Id,
            text: response
        );
    }
}
```

## Reply Keyboard Bot

A bot that uses reply keyboards for user interaction.

```csharp
public class ReplyKeyboardBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public ReplyKeyboardBotHandler(ITelegramBotClient bot, ILogger<ReplyKeyboardBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/start")
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] { "Button 1", "Button 2" },
                new[] { "Button 3", "Button 4" },
                new[] { "Remove Keyboard" }
            });

            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose a button:",
                replyMarkup: keyboard
            );
        }
        else if (message.Text == "Remove Keyboard")
        {
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Keyboard removed",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
        else
        {
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"You pressed: {message.Text}"
            );
        }
    }
}
```

## Inline Query Bot

A bot that handles inline queries for inline mode.

```csharp
public class InlineQueryBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public InlineQueryBotHandler(ITelegramBotClient bot, ILogger<InlineQueryBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnInlineQuery(InlineQuery query)
    {
        var results = new InlineQueryResult[]
        {
            new InlineQueryResultArticle("1", "Hello", new InputTextMessageContent("Hello!")),
            new InlineQueryResultArticle("2", "World", new InputTextMessageContent("World!")),
            new InlineQueryResultArticle("3", "Test", new InputTextMessageContent("Test message"))
        };

        await _bot.AnswerInlineQueryAsync(query.Id, results);
    }

    protected async Task OnChosenInlineResult(ChosenInlineResult result)
    {
        await _bot.SendTextMessageAsync(
            chatId: result.From.Id,
            text: $"You chose: {result.ResultId}"
        );
    }
}
```

## Poll Bot

A bot that creates and handles polls.

```csharp
public class PollBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public PollBotHandler(ITelegramBotClient bot, ILogger<PollBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/poll")
        {
            var pollOptions = new InputPollOption[]
            {
                new InputPollOption("Option 1"),
                new InputPollOption("Option 2"),
                new InputPollOption("Option 3")
            };

            await _bot.SendPollAsync(
                chatId: message.Chat.Id,
                question: "What is your favorite option?",
                options: pollOptions,
                isAnonymous: false
            );
        }
    }

    protected async Task OnPoll(Poll poll)
    {
        _logger.LogInformation("Poll received: {Question}", poll.Question);
    }

    protected async Task OnPollAnswer(PollAnswer answer)
    {
        if (answer.User != null)
        {
            await _bot.SendTextMessageAsync(
                chatId: answer.User.Id,
                text: "Thank you for voting!"
            );
        }
    }
}
```

## File Upload Bot

A bot that handles file uploads and sends files.

```csharp
public class FileUploadBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public FileUploadBotHandler(ITelegramBotClient bot, ILogger<FileUploadBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/photo")
        {
            await SendPhoto(message);
        }
        else if (message.Text == "/document")
        {
            await SendDocument(message);
        }
        else if (message.Document != null)
        {
            await HandleDocumentUpload(message);
        }
        else if (message.Photo != null)
        {
            await HandlePhotoUpload(message);
        }
    }

    private async Task SendPhoto(Message message)
    {
        await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
        
        // Send a photo from file
        using var fileStream = new FileStream("path/to/photo.jpg", FileMode.Open, FileAccess.Read);
        await _bot.SendPhotoAsync(
            chatId: message.Chat.Id,
            photo: fileStream,
            caption: "Here's a photo!"
        );
    }

    private async Task SendDocument(Message message)
    {
        await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadDocument);
        
        // Send a document from file
        using var fileStream = new FileStream("path/to/document.pdf", FileMode.Open, FileAccess.Read);
        await _bot.SendDocumentAsync(
            chatId: message.Chat.Id,
            document: fileStream,
            caption: "Here's a document!"
        );
    }

    private async Task HandleDocumentUpload(Message message)
    {
        var document = message.Document;
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"Document received: {document.FileName} ({document.FileSize} bytes)"
        );
    }

    private async Task HandlePhotoUpload(Message message)
    {
        var photo = message.Photo.Last(); // Get highest resolution
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"Photo received: {photo.FileSize} bytes"
        );
    }
}
```

## Database Integration Bot

A bot that integrates with a database using Entity Framework.

```csharp
public class DatabaseBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;
    private readonly ApplicationDbContext _context;

    public DatabaseBotHandler(
        ITelegramBotClient bot, 
        ApplicationDbContext context,
        ILogger<DatabaseBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
        _context = context;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/register")
        {
            await RegisterUser(message);
        }
        else if (message.Text == "/profile")
        {
            await ShowProfile(message);
        }
    }

    private async Task RegisterUser(Message message)
    {
        var user = new User
        {
            TelegramId = message.From.Id,
            Username = message.From.Username,
            FirstName = message.From.FirstName,
            LastName = message.From.LastName,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "User registered successfully!"
        );
    }

    private async Task ShowProfile(Message message)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.TelegramId == message.From.Id);

        if (user != null)
        {
            var profile = $"Username: @{user.Username}\n" +
                         $"Name: {user.FirstName} {user.LastName}\n" +
                         $"Registered: {user.RegisteredAt:yyyy-MM-dd}";
                         
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: profile
            );
        }
        else
        {
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "User not found. Use /register to create an account."
            );
        }
    }
}
```

## Multi-Language Bot

A bot that supports multiple languages.

```csharp
public class MultiLanguageBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<MultiLanguageBotHandler> _localizer;

    public MultiLanguageBotHandler(
        ITelegramBotClient bot,
        IStringLocalizer<MultiLanguageBotHandler> localizer,
        ILogger<MultiLanguageBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
        _localizer = localizer;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text == "/start")
        {
            var welcomeText = _localizer["WelcomeMessage"];
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: welcomeText
            );
        }
        else if (message.Text == "/help")
        {
            var helpText = _localizer["HelpMessage"];
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: helpText
            );
        }
    }
}
```

## Error Handling Bot

A bot with comprehensive error handling.

```csharp
public class ErrorHandlingBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public ErrorHandlingBotHandler(ITelegramBotClient bot, ILogger<ErrorHandlingBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        try
        {
            if (message.Text == "/error")
            {
                throw new Exception("Simulated error for testing");
            }
            
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Received: {message.Text}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message: {MessageText}", message.Text);
            
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Sorry, an error occurred while processing your message."
            );
        }
    }

    public override async Task HandleErrorAsync(
        ITelegramBotClient botClient, 
        Exception exception, 
        HandleErrorSource source, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Bot error from source: {Source}", source);
        
        // Custom error handling logic
        if (exception is ApiRequestException apiException)
        {
            _logger.LogError("Telegram API Error: {ErrorCode} - {Message}", 
                apiException.ErrorCode, apiException.Message);
        }
        
        await base.HandleErrorAsync(botClient, exception, source, cancellationToken);
    }
}
```

## Configuration Examples

### Development Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsPollingClient()
    .WithPollingInterval(2)
    .SetDropPendingUpdates(true)
);

var app = builder.Build();
app.Run();
```

### Production Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTgBotPlay<MyBotHandler>(options => options
    .WithToken("YOUR_BOT_TOKEN")
    .AsWebHookClient("https://yourdomain.com")
    .WithController("TelegramBot", "api/telegram/[action]")
    .WithWebHookRefreshInterval(60)
    .SetDropPendingUpdates(false)
);

builder.Services.AddHealthChecks()
    .AddTgBotPlayHealth();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

### Environment-Based Configuration

```csharp
var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();
var botToken = builder.Configuration["TelegramBot:Token"];
var webhookUrl = builder.Configuration["TelegramBot:WebhookUrl"];

builder.Services.AddTgBotPlay<MyBotHandler>(options =>
{
    options.WithToken(botToken);
    
    if (isDevelopment)
    {
        options.AsPollingClient()
               .WithPollingInterval(3)
               .SetDropPendingUpdates(true);
    }
    else
    {
        options.AsWebHookClient(webhookUrl)
               .WithWebHookRefreshInterval(30);
    }
});

var app = builder.Build();
app.Run();
```

## Best Practices

1. **Use dependency injection** for services
2. **Handle errors gracefully** with try-catch blocks
3. **Log appropriately** using structured logging
4. **Validate input** before processing
5. **Use async/await** for all operations
6. **Keep handlers focused** on single responsibilities
7. **Test your handlers** with unit tests
8. **Use configuration** for environment-specific settings
9. **Implement health checks** for monitoring
10. **Follow security best practices** for webhooks
