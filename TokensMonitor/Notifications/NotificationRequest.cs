namespace TokensMonitor.Notifications;

public record NotificationRequest(string UserId, string ContractAddress, decimal Balance, string TelegramChannelId, TelegramBot? TelegramBot);