using TokensMonitor.Notifications;

namespace TokensMonitor.Wallet;

public record TokensWatcherTask(string Address, IEnumerable<string> AddressList, string UserId, string TelegramChannelId, TelegramBot? TelegramBot);