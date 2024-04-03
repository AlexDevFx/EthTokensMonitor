using System.Collections.Concurrent;
using System.Threading.Channels;
using TokensMonitor.Notifications;

namespace TokensMonitor.Wallet;

public class MonitorScheduler(ILogger<MonitorScheduler> logger, ChannelWriter<TokensWatcherTask> tokensWatcherChannel): IDisposable
{
    private readonly ConcurrentDictionary<string, WalletTimer> _timers = new ();

    public void NewTimer(TimeSpan period, 
        string userId,
        string address,
        IEnumerable<string> contractAddress,
        TelegramChannel telegram)
    {
        if (_timers.ContainsKey(userId)) return;
        
        var walletTimer = new WalletTimer(userId,
            address,
            contractAddress,
            telegram.ChannelId,
            !string.IsNullOrEmpty(telegram.ApiKey) && !string.IsNullOrWhiteSpace(telegram.ApiKey) ? new TelegramBot(telegram.ApiKey): null);
        var timer = new Timer(CheckBalance, walletTimer, TimeSpan.Zero, period);
        walletTimer.SetTimer(timer);
        
        _timers.TryAdd(userId, walletTimer);
    }

    public void RemoveTimer(string userId)
    {
        if (_timers.ContainsKey(userId))
        {
            if(_timers.TryRemove(userId, out var removedTimer))
                removedTimer?.Dispose();
        }
    }

    public bool IsWorkingForUser(string userId) => _timers.ContainsKey(userId);

    private void CheckBalance(object? state)
    {
        try
        {
            if (state is WalletTimer timer)
            {
                tokensWatcherChannel.TryWrite(new TokensWatcherTask(timer.Address, timer.ContractAddress, timer.UserId, timer.TelegramChannelId, timer.TelegramBot));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed schedule task for balance");
        }
    }

    public void Dispose()
    {
        foreach (var timer in _timers)
        {
            timer.Value.Dispose();
        }
    }
}