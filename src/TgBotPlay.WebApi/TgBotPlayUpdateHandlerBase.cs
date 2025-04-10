using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotPlay.WebAPI;

/// <summary>
/// Base class for handling Telegram Bot updates using reflection-based method discovery.
/// Implement any of the following methods in your derived class to handle specific update types:
/// </summary>
/// <remarks>
/// Available handler methods:
/// <list type="bullet">
/// <item><description><c>protected Task OnMessage(Message message)</c> - Regular messages</description></item>
/// <item><description><c>protected Task OnEditedMessage(Message message)</c> - Edited messages</description></item>
/// <item><description><c>protected Task OnChannelPost(Message message)</c> - Channel posts</description></item>
/// <item><description><c>protected Task OnEditedChannelPost(Message message)</c> - Edited channel posts</description></item>
/// <item><description><c>protected Task OnBusinessConnection(BusinessConnection connection)</c> - Business connections</description></item>
/// <item><description><c>protected Task OnBusinessMessage(BusinessMessage message)</c> - Business messages</description></item>
/// <item><description><c>protected Task OnEditedBusinessMessage(BusinessMessage message)</c> - Edited business messages</description></item>
/// <item><description><c>protected Task OnDeletedBusinessMessages(DeletedBusinessMessages messages)</c> - Deleted business messages</description></item>
/// <item><description><c>protected Task OnMessageReaction(MessageReaction reaction)</c> - Message reactions</description></item>
/// <item><description><c>protected Task OnMessageReactionCount(MessageReactionCount reactionCount)</c> - Message reaction counts</description></item>
/// <item><description><c>protected Task OnInlineQuery(InlineQuery query)</c> - Inline queries</description></item>
/// <item><description><c>protected Task OnChosenInlineResult(ChosenInlineResult result)</c> - Chosen inline results</description></item>
/// <item><description><c>protected Task OnCallbackQuery(CallbackQuery query)</c> - Callback queries</description></item>
/// <item><description><c>protected Task OnShippingQuery(ShippingQuery query)</c> - Shipping queries</description></item>
/// <item><description><c>protected Task OnPreCheckoutQuery(PreCheckoutQuery query)</c> - Pre-checkout queries</description></item>
/// <item><description><c>protected Task OnPurchasedPaidMedia(PurchasedPaidMedia media)</c> - Purchased paid media</description></item>
/// <item><description><c>protected Task OnPoll(Poll poll)</c> - Polls</description></item>
/// <item><description><c>protected Task OnPollAnswer(PollAnswer answer)</c> - Poll answers</description></item>
/// <item><description><c>protected Task OnMyChatMember(ChatMember member)</c> - My chat member updates</description></item>
/// <item><description><c>protected Task OnChatMember(ChatMember member)</c> - Chat member updates</description></item>
/// <item><description><c>protected Task OnChatJoinRequest(ChatJoinRequest request)</c> - Chat join requests</description></item>
/// <item><description><c>protected Task OnChatBoost(ChatBoost boost)</c> - Chat boosts</description></item>
/// <item><description><c>protected Task OnRemovedChatBoost(RemovedChatBoost boost)</c> - Removed chat boosts</description></item>
/// </list>
/// 
/// Example implementation:
/// <code>
/// public class MyBotHandler : TgBotPlayUpdateHandlerBase
/// {
///     public MyBotHandler(ILogger&lt;MyBotHandler&gt; logger) : base(logger) { }
///     
///     protected async Task OnMessage(Message message)
///     {
///         // Handle regular messages
///     }
///     
///     protected async Task OnCallbackQuery(CallbackQuery query)
///     {
///         // Handle callback queries
///     }
/// }
/// </code>
/// </remarks>
public class TgBotPlayUpdateHandlerBase : IUpdateHandler
{
    protected readonly ILogger _logger;
    private static Dictionary<UpdateType, MethodInfo>? Handlers = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="TgBotPlayUpdateHandlerBase"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected TgBotPlayUpdateHandlerBase(ILogger logger)
    {
        _logger = logger;
        if (Handlers == null)
        {
            Handlers = new Dictionary<UpdateType, MethodInfo>();
            InitializeHandlers();
        }
    }

    private void InitializeHandlers()
    {
        var type = GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (var method in methods)
        {
            if (method.Name.StartsWith("On") && method.GetParameters().Length == 1)
            {
                var updateTypeName = method.Name[2..]; // Remove "On" prefix
                if (Enum.TryParse<UpdateType>(updateTypeName, out var updateType))
                {
                    Handlers![updateType] = method;
                }
            }
        }
    }

    /// <summary>
    /// Handles errors that occur during update processing.
    /// </summary>
    /// <param name="botClient">The bot client instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="source">The source of the error.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public virtual async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        _logger.LogError(errorMessage);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    /// <summary>
    /// Handles incoming updates from Telegram.
    /// </summary>
    /// <param name="botClient">The bot client instance.</param>
    /// <param name="update">The update to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [DebuggerStepThrough]
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updateType = update.Type;
            if (Handlers!.TryGetValue(updateType, out var handler))
            {
                var updateProperty = typeof(Update).GetProperty(updateType.ToString());
                if (updateProperty != null)
                {
                    var updateValue = updateProperty.GetValue(update);
                    if (updateValue != null)
                    {
                        await (Task)handler.Invoke(this, [updateValue])!;
                        return;
                    }
                }
            }

            _logger.LogWarning($"Update type {updateType} is not implemented");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(botClient, ex, HandleErrorSource.HandleUpdateError, cancellationToken);
        }
    }

    /// <summary>
    /// Gets the collection of update types that have implemented handlers.
    /// </summary>
    /// <returns>An enumerable of implemented update types.</returns>
    public IEnumerable<UpdateType> GetImplementedUpdateTypes()
    {
        return Handlers!.Keys;
    }
}
