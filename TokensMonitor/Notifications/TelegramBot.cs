using Telegram.Bot;

namespace TokensMonitor.Notifications;

public class TelegramBot(string apiKey)
{
    private readonly TelegramBotClient _bot = new (apiKey);
    public async Task SendMessage(string channelId, string message)
    {
        await _bot.SendTextMessageAsync(channelId, message);
    }
}