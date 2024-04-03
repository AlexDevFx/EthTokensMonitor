using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TokensMonitor.Configuration;

namespace TokensMonitor.Authentication;

public class TokensService(IOptionsSnapshot<MonitorAppConfig> optionsSnapshot)
{
    public string? Build(NewTokenRequest request)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, request.Address[2..16]),
                new Claim(TokenConsts.ClaimTypeAddress, request.Address) ,
                new Claim(TokenConsts.ClaimTypeSignature, request.Signature),
                new Claim(TokenConsts.ClaimTypeExpiry, request.ExpirationTime.ToString(CultureInfo.InvariantCulture)),
            }),


            SigningCredentials = new SigningCredentials(KeyBuilder.CreateKey(optionsSnapshot.Value.Auth.SecretKey), SecurityAlgorithms.HmacSha256)
        };
        tokenDescriptor.Expires = request.ExpirationTime;
        tokenDescriptor.IssuedAt = request.Issued;
        
       return tokenHandler.CreateToken(tokenDescriptor);
    }

    public (TokenData? Token, string? Error) ReadToken(string? token)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token)) return (null, "Token is empty");
        
        var tokenHandler = new JsonWebTokenHandler();
        
        JsonWebToken? tokenResult = tokenHandler.ReadJsonWebToken(token);

        if (tokenResult == null)
            return (null, "Cannot read token");

        var claimsResult = ExtractAndCheckClaims(tokenResult.Claims.ToList(), 
            [ClaimTypes.NameIdentifier, TokenConsts.ClaimTypeAddress, TokenConsts.ClaimTypeExpiry, TokenConsts.ClaimTypeSignature]);

        if (claimsResult.Error != null)
            return (null, claimsResult.Error);

        return (new TokenData(claimsResult.Claims[TokenConsts.ClaimTypeAddress], 
            claimsResult.Claims[TokenConsts.ClaimTypeSignature], 
            claimsResult.Claims[TokenConsts.ClaimTypeExpiry],
            claimsResult.Claims[ClaimTypes.NameIdentifier]
            ), 
            null);
    }

    private (Dictionary<string, string>? Claims, string? Error) ExtractAndCheckClaims(ICollection<Claim> tokenClaims, IEnumerable<string> claims)
    {
        Dictionary<string, string> result = new();
        
        foreach (var claim in claims)
        {
            var tokenClaim = tokenClaims?.FirstOrDefault(e => e.Type == claim);
            if (tokenClaim == null)
                return (null, $"Token doesn't contain {claim}");

            string? claimValue = tokenClaim.Value;
            
            if (string.IsNullOrEmpty(claimValue) || string.IsNullOrWhiteSpace(claimValue))
                return (null, $"Value of {claim} is empty");

            result[claim] = claimValue;
        }

        return (result, null);
    }
}

public record TokenData(string Address, string Signature, string Expired, string UserId);