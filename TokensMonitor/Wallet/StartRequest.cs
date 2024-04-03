namespace TokensMonitor.Wallet;

public class StartRequest
{
    public string Address { get; set; }
    public List<string> AddressList { get; set; }
    public int PeriodMinutes { get; set; }
    public TelegramChannel Telegram { get; set; }
    public TimeSpan Period => TimeSpan.FromMinutes(PeriodMinutes);
}