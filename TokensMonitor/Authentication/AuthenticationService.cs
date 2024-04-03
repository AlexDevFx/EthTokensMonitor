using System.Text;
using Nethereum.Signer;
using Nethereum.Siwe;
using Nethereum.Siwe.Core;

namespace TokensMonitor.Authentication;

public class AuthenticationService(SiweMessageService siweMessageService)
{

    public async Task<AuthenticationResult> Authenticate(AuthenticationRequest request)
    {
        AuthenticationResult result = new(request.Validate(), null);

        if (!result.Success) 
            return result;
        
        var siweMessage = SiweMessageParser.Parse(request.EncodedMessage);
        var signature = request.Signature;
        var validUser = await siweMessageService.IsUserAddressRegistered(siweMessage);
        
        if (!validUser) 
            return new("User is invalid", null);
        
        /*if (!await siweMessageService.IsMessageSignatureValid(siweMessage, signature)) 
            return new("Signature is invalid", null);*/
        
        if (!siweMessageService.HasMessageDateStartedAndNotExpired(siweMessage))
            return new("Token is expired", null);

        return new(null, new NewTokenRequest(siweMessage.Address, request.Signature, DateTime.UtcNow, DateTime.Parse(siweMessage.ExpirationTime)));
    }
}