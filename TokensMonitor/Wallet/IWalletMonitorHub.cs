namespace TokensMonitor.Wallet;

public interface IWalletMonitorHub
{
    Task RefreshBalance(RefreshBalanceRequest request);
}