using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace TokensMonitor.Wallet;


public class WalletMonitorHub: Hub<IWalletMonitorHub>
{
    public void Start(StartRequest request, 
        [FromServices]MonitorScheduler monitorScheduler, 
        [FromServices]ILogger<WalletMonitorHub> logger)
    {
        try
        {
            monitorScheduler.NewTimer(request.Period, 
                Context.UserIdentifier,
                request.Address,
                request.AddressList,
                request.Telegram);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cannot call refreshBalance");
        }
    }

    [Authorize]
    public void Stop([FromServices]MonitorScheduler monitorScheduler, 
        [FromServices] ILogger<WalletMonitorHub> logger)
    {
        try
        {
            monitorScheduler.RemoveTimer(Context.UserIdentifier);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cannot call refreshBalance");
        }
    }
}