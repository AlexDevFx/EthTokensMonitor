using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TokensMonitor.Authentication;

public static class KeyBuilder
{
    public static SecurityKey CreateKey(string secretKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    } 
}