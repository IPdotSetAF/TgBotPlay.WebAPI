# Update Handlers

TgBotPlay.WebAPI uses a reflection-based system to automatically discover and register your bot's update handlers. This guide explains how to implement handlers for different types of Telegram updates.

## Handler Base Class

All bot handlers must inherit from `TgBotPlayUpdateHandlerBase`:

```csharp
using TgBotPlay.WebAPI;
using Telegram.Bot.Types;

public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    public MyBotHandler(ILogger<MyBotHandler> logger) : base(logger) { }
    
    // Implement handler methods here
}
```

## Available Handler Methods

TgBotPlay.WebAPI supports all Telegram update types. Implement only the handlers you need - the framework will automatically discover and register them.

### Message Handlers

#### OnMessage(Message message)
Handles regular messages sent to the bot.

```csharp
protected async Task OnMessage(Message message)
{
    if (message.Text is not null)
    {
        _logger.LogInformation("Received message: {Text}", message.Text);
        
        // Handle text message
        if (message.Text.StartsWith("/start"))
        {
            // Handle /start command
        }
    }
}
```

#### OnEditedMessage(Message message)
Handles edited messages.

```csharp
protected async Task OnEditedMessage(Message message)
{
    _logger.LogInformation("Message edited: {Text}", message.Text);
}
```

#### OnChannelPost(Message message)
Handles messages posted in channels.

```csharp
protected async Task OnChannelPost(Message message)
{
    _logger.LogInformation("Channel post: {Text}", message.Text);
}
```

#### OnEditedChannelPost(Message message)
Handles edited channel posts.

```csharp
protected async Task OnEditedChannelPost(Message message)
{
    _logger.LogInformation("Channel post edited: {Text}", message.Text);
}
```

### Business Message Handlers

#### OnBusinessConnection(BusinessConnection connection)
Handles business connections.

```csharp
protected async Task OnBusinessConnection(BusinessConnection connection)
{
    _logger.LogInformation("Business connection: {ConnectionId}", connection.Id);
}
```

#### OnBusinessMessage(BusinessMessage message)
Handles business messages.

```csharp
protected async Task OnBusinessMessage(BusinessMessage message)
{
    _logger.LogInformation("Business message: {Text}", message.Text);
}
```

#### OnEditedBusinessMessage(BusinessMessage message)
Handles edited business messages.

```csharp
protected async Task OnEditedBusinessMessage(BusinessMessage message)
{
    _logger.LogInformation("Business message edited: {Text}", message.Text);
}
```

#### OnDeletedBusinessMessages(DeletedBusinessMessages messages)
Handles deleted business messages.

```csharp
protected async Task OnDeletedBusinessMessages(DeletedBusinessMessages messages)
{
    _logger.LogInformation("Business messages deleted: {Count}", messages.MessageIds.Length);
}
```

### Reaction Handlers

#### OnMessageReaction(MessageReaction reaction)
Handles message reactions.

```csharp
protected async Task OnMessageReaction(MessageReaction reaction)
{
    _logger.LogInformation("Message reaction: {Reaction}", reaction.Reaction);
}
```

#### OnMessageReactionCount(MessageReactionCount reactionCount)
Handles message reaction counts.

```csharp
protected async Task OnMessageReactionCount(MessageReactionCount reactionCount)
{
    _logger.LogInformation("Reaction count: {Count}", reactionCount.TotalCount);
}
```

### Query Handlers

#### OnInlineQuery(InlineQuery query)
Handles inline queries.

```csharp
protected async Task OnInlineQuery(InlineQuery query)
{
    _logger.LogInformation("Inline query: {Query}", query.Query);
    
    // Create inline query results
    var results = new InlineQueryResult[]
    {
        new InlineQueryResultArticle("1", "Result 1", new InputTextMessageContent("Hello")),
        new InlineQueryResultArticle("2", "Result 2", new InputTextMessageContent("World"))
    };
    
    // Note: You'll need to inject ITelegramBotClient to answer the query
}
```

#### OnChosenInlineResult(ChosenInlineResult result)
Handles chosen inline results.

```csharp
protected async Task OnChosenInlineResult(ChosenInlineResult result)
{
    _logger.LogInformation("Chosen inline result: {ResultId}", result.ResultId);
}
```

#### OnCallbackQuery(CallbackQuery query)
Handles callback queries from inline keyboards.

```csharp
protected async Task OnCallbackQuery(CallbackQuery query)
{
    _logger.LogInformation("Callback query: {Data}", query.Data);
    
    // Answer the callback query
    // Note: You'll need to inject ITelegramBotClient to answer the query
}
```

### Payment Handlers

#### OnShippingQuery(ShippingQuery query)
Handles shipping queries.

```csharp
protected async Task OnShippingQuery(ShippingQuery query)
{
    _logger.LogInformation("Shipping query: {QueryId}", query.Id);
}
```

#### OnPreCheckoutQuery(PreCheckoutQuery query)
Handles pre-checkout queries.

```csharp
protected async Task OnPreCheckoutQuery(PreCheckoutQuery query)
{
    _logger.LogInformation("Pre-checkout query: {QueryId}", query.Id);
}
```

#### OnPurchasedPaidMedia(PurchasedPaidMedia media)
Handles purchased paid media.

```csharp
protected async Task OnPurchasedPaidMedia(PurchasedPaidMedia media)
{
    _logger.LogInformation("Purchased paid media: {MediaId}", media.TelegramPaymentChargeId);
}
```

### Poll Handlers

#### OnPoll(Poll poll)
Handles polls.

```csharp
protected async Task OnPoll(Poll poll)
{
    _logger.LogInformation("Poll: {Question}", poll.Question);
}
```

#### OnPollAnswer(PollAnswer answer)
Handles poll answers.

```csharp
protected async Task OnPollAnswer(PollAnswer answer)
{
    _logger.LogInformation("Poll answer: {OptionIds}", string.Join(",", answer.OptionIds));
}
```

### Chat Member Handlers

#### OnMyChatMember(ChatMember member)
Handles my chat member updates.

```csharp
protected async Task OnMyChatMember(ChatMember member)
{
    _logger.LogInformation("My chat member update: {Status}", member.Status);
}
```

#### OnChatMember(ChatMember member)
Handles chat member updates.

```csharp
protected async Task OnChatMember(ChatMember member)
{
    _logger.LogInformation("Chat member update: {Status}", member.Status);
}
```

#### OnChatJoinRequest(ChatJoinRequest request)
Handles chat join requests.

```csharp
protected async Task OnChatJoinRequest(ChatJoinRequest request)
{
    _logger.LogInformation("Chat join request: {UserId}", request.From.Id);
}
```

### Boost Handlers

#### OnChatBoost(ChatBoost boost)
Handles chat boosts.

```csharp
protected async Task OnChatBoost(ChatBoost boost)
{
    _logger.LogInformation("Chat boost: {BoostId}", boost.BoostId);
}
```

#### OnRemovedChatBoost(RemovedChatBoost boost)
Handles removed chat boosts.

```csharp
protected async Task OnRemovedChatBoost(RemovedChatBoost boost)
{
    _logger.LogInformation("Chat boost removed: {BoostId}", boost.BoostId);
}
```

## Complete Handler Example

Here's a complete example showing how to implement multiple handlers:

```csharp
using TgBotPlay.WebAPI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

public class CompleteBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;

    public CompleteBotHandler(ITelegramBotClient bot, ILogger<CompleteBotHandler> logger) 
        : base(logger)
    {
        _bot = bot;
    }

    protected async Task OnMessage(Message message)
    {
        if (message.Text is not null)
        {
            switch (message.Text.Split(' ')[0])
            {
                case "/start":
                    await HandleStartCommand(message);
                    break;
                case "/help":
                    await HandleHelpCommand(message);
                    break;
                case "/keyboard":
                    await SendKeyboard(message);
                    break;
                default:
                    await HandleUnknownMessage(message);
                    break;
            }
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

    protected async Task OnInlineQuery(InlineQuery query)
    {
        var results = new InlineQueryResult[]
        {
            new InlineQueryResultArticle("1", "Option 1", new InputTextMessageContent("You selected option 1")),
            new InlineQueryResultArticle("2", "Option 2", new InputTextMessageContent("You selected option 2"))
        };

        await _bot.AnswerInlineQueryAsync(query.Id, results);
    }

    private async Task HandleStartCommand(Message message)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Help", "help") },
            new[] { InlineKeyboardButton.WithCallbackData("About", "about") }
        });

        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Welcome! Choose an option:",
            replyMarkup: keyboard
        );
    }

    private async Task HandleHelpCommand(Message message)
    {
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Available commands:\n/start - Start the bot\n/help - Show this help\n/keyboard - Show keyboard"
        );
    }

    private async Task SendKeyboard(Message message)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new[] { "Button 1", "Button 2" },
            new[] { "Button 3", "Button 4" }
        });

        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Choose a button:",
            replyMarkup: keyboard
        );
    }

    private async Task HandleUnknownMessage(Message message)
    {
        await _bot.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "I didn't understand that. Use /help to see available commands."
        );
    }
}
```

## Error Handling

The base class provides built-in error handling. You can override the `HandleErrorAsync` method to customize error handling:

```csharp
public override async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
{
    _logger.LogError(exception, "Error occurred in bot handler");
    
    // Custom error handling logic
    if (exception is ApiRequestException apiException)
    {
        _logger.LogError("Telegram API Error: {ErrorCode} - {Message}", 
            apiException.ErrorCode, apiException.Message);
    }
    
    // Call base implementation
    await base.HandleErrorAsync(botClient, exception, source, cancellationToken);
}
```

## Dependency Injection

You can inject any registered service into your handler constructor:

```csharp
public class MyBotHandler : TgBotPlayUpdateHandlerBase
{
    private readonly ITelegramBotClient _bot;
    private readonly IMyService _myService;
    private readonly IConfiguration _configuration;

    public MyBotHandler(
        ITelegramBotClient bot,
        IMyService myService,
        IConfiguration configuration,
        ILogger<MyBotHandler> logger) : base(logger)
    {
        _bot = bot;
        _myService = myService;
        _configuration = configuration;
    }

    protected async Task OnMessage(Message message)
    {
        // Use injected services
        var result = await _myService.ProcessMessage(message.Text);
        await _bot.SendTextMessageAsync(message.Chat.Id, result);
    }
}
```

## Handler Discovery

TgBotPlay.WebAPI uses reflection to discover handler methods at runtime. The discovery process:

1. Scans your handler class for methods starting with "On"
2. Matches method names to Telegram update types (e.g., "OnMessage" → `UpdateType.Message`)
3. Registers only the methods you implement
4. Ignores methods that don't match known update types

This means you only need to implement the handlers you actually use, and the framework will automatically handle the rest.

## Best Practices

1. **Use async/await**: All handler methods should be async
2. **Handle null values**: Always check for null values in update properties
3. **Log appropriately**: Use structured logging for better debugging
4. **Keep handlers focused**: Each handler should have a single responsibility
5. **Use dependency injection**: Inject services you need rather than creating them
6. **Handle errors gracefully**: Don't let exceptions crash your bot
7. **Test your handlers**: Write unit tests for your handler logic
