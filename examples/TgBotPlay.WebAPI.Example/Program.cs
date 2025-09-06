using TgBotPlay.WebAPI;
using TgBotPlay.WebAPI.Example;
using TgBotPlay.WebAPI.Example.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var botConfigSection = builder.Configuration.GetSection("TelegramBot");
builder.Services.Configure<BotConfiguration>(botConfigSection);
builder.Services.AddTgBotPlay<BotUpdateHandler>(options =>
{
    var botConfiguration = builder.Configuration.GetRequiredSection("TelegramBot").Get<BotConfiguration>()!;
    options
        .WithToken(botConfiguration.Token)
        .WithController("Bot", "bot/[action]");
    if (botConfiguration.ConnectionMethod == TgBotPlayConnectionMethod.WEB_HOOK)
        options.AsWebHookClient(botConfiguration.Host.ToString());
    else
        options.AsPollingClient();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
