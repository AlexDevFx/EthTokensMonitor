using TokensMonitor.Notifications;

namespace TokensMonitor.Wallet;

public class WalletTimer(
    string userId,
    string address,
    IEnumerable<string> contractAddress,
    string? telegramChannelId,
    TelegramBot? telegramBot): IDisposable, IAsyncDisposable
{
    public void SetTimer(Timer timer)
    {
        Timer = timer;
    }

    public Timer Timer { get; private set; }
    public string UserId { get; } = userId;
    public string Address { get; } = address;
    public string TelegramChannelId { get; } = telegramChannelId;
    public IEnumerable<string> ContractAddress { get; } = contractAddress;
    public TelegramBot? TelegramBot { get; } = telegramBot;

    public void Dispose()
    {
        Timer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Timer.DisposeAsync();
    }
}