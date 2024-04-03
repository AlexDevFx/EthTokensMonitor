using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nethereum.Blazor;
using Nethereum.Signer;
using Nethereum.Siwe;
using Nethereum.UI;
using Nethereum.Util;
using TokensMonitor.Configuration;

namespace TokensMonitor.Authentication;

public class AuthMessageService(
    SiweMessageService siweMessageService,
    SelectedEthereumHostProviderService selectedHostProviderService,
    IOptionsSnapshot<MonitorAppConfig> optionsSnapshot)
    : EthereumAuthenticationStateProvider(selectedHostProviderService)
{
    private readonly string _signatureKey = optionsSnapshot.Value.Auth.SignatureKey;

    public (string Message, string Signature) NewSignedMessage([FromBody] string address)
    {
        var message = new AuthSiweMessage();
        DateTime currentTime = DateTime.UtcNow;
        message.SetExpirationTime(currentTime.AddMinutes(30));
        message.SetIssuedAt(currentTime);
        message.Address = address.ConvertToEthereumChecksumAddress();

        string siweMessage = siweMessageService.BuildMessageToSign(message);
        var messageSigner = new EthereumMessageSigner();

        string signature = messageSigner.EncodeUTF8AndSign(siweMessage, new EthECKey(_signatureKey));

        return (siweMessage, signature);
    }
}