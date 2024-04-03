using Nethereum.Siwe.Core;

namespace TokensMonitor.Authentication;

public class AuthSiweMessage: SiweMessage
{
    public AuthSiweMessage()
    {
        Domain = "login.xyz";
        Statement = "Login into Tokens Monitor";
        Uri = "https://login.xyz";
        Version = "1";
        ChainId = "1";
    }
}