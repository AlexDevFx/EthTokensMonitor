using TokensMonitor.Authentication;
using TokensMonitor.Wallet;

namespace TokensMonitor.Configuration;

public class MonitorAppConfig
{
    public Settings Auth { get; protected set; }
    public InfuraSettings Infura { get; protected set; }
}