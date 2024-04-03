using System.Numerics;
using Microsoft.Extensions.Options;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Web3;
using TokensMonitor.Configuration;

namespace TokensMonitor.Wallet;

public class WalletService(IOptionsSnapshot<MonitorAppConfig> optionsSnapshot, ILogger<WalletService> logger)
{
    private readonly Web3 _web3 = new Web3($"https://mainnet.infura.io/v3/{optionsSnapshot.Value.Infura.ApiKey}");

    public async Task<decimal> TokenBalance(string address, string contractAddress)
    {
        try
        {
            var balance = await _web3.Eth.GetContractQueryHandler<BalanceOfFunction>()
                .QueryAsync<BigInteger>(contractAddress, new BalanceOfFunction() {Owner = address});
            return Web3.Convert.FromWei(balance, 18);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Getting balance is failed");
            return 0m;
        }
    }
}