using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using TokensMonitor.Wallet;

namespace TokensMonitor.Notifications;

public class NotificationsSender(ILogger<NotificationsSender> logger, 
    ChannelReader<NotificationRequest> notificationsChannel,
    IHubContext<WalletMonitorHub, IWalletMonitorHub> walletMonitorHub): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NotificationsSender started");
      
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await notificationsChannel.WaitToReadAsync(stoppingToken))
            {
                await Task.Delay(500, stoppingToken);
                continue;
            }

            while (notificationsChannel.TryRead(out NotificationRequest? notification) && !stoppingToken.IsCancellationRequested)
            {
                if(notification == null) break;

                await walletMonitorHub.Clients.User(notification.UserId)
                    .RefreshBalance(new RefreshBalanceRequest(notification.ContractAddress, notification.Balance));

                if(!string.IsNullOrEmpty(notification.TelegramChannelId) && !string.IsNullOrWhiteSpace(notification.TelegramChannelId))
                    await notification.TelegramBot?.SendMessage(notification.TelegramChannelId, $"Address {notification.ContractAddress}\nETH: {notification.Balance:C4}");
            }
        }
        
        logger.LogInformation("NotificationsSender stopped");
    }
}