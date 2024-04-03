using System.Threading.Channels;
using TokensMonitor.Notifications;

namespace TokensMonitor.Wallet;

public class TokensWatcher(ILogger<TokensWatcher> logger, 
    IServiceProvider serviceProvider, 
    ChannelReader<TokensWatcherTask> tasksChannel,
    ChannelWriter<NotificationRequest> notificationsChannel): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TokensWatcher started");
        using var scope = serviceProvider.CreateScope();
        {
            var walletService = scope.ServiceProvider.GetService<WalletService>();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!await tasksChannel.WaitToReadAsync(stoppingToken))
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                while (tasksChannel.TryRead(out TokensWatcherTask? task) && !stoppingToken.IsCancellationRequested)
                {
                    if(task == null) break;
                    foreach (var contractAddress in task.AddressList)
                    {
                        TokenBalanceResult result = await TokensBalance(task.Address, contractAddress, walletService);

                        await notificationsChannel.WriteAsync(new NotificationRequest(task.UserId, contractAddress, result.Balance, task.TelegramChannelId, task.TelegramBot), stoppingToken);
                    }
                }
            }
        }
        
        logger.LogInformation("TokensWatcher stopped");
    }

    private async Task<TokenBalanceResult> TokensBalance(string address, string contractAddress, WalletService walletService)
    {
        decimal balance = await walletService.TokenBalance(address, contractAddress);

        return new(contractAddress, balance);
    }
}