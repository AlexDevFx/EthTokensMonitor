using Microsoft.Extensions.Primitives;

namespace TokensMonitor.Authentication;

public class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TokensService tokensService, IUserContext userContext)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out StringValues authorizationValues))
        {
            string? token = authorizationValues[0]?.Split(" ")[^1];
            var validationResult = tokensService.ReadToken(token);
            if (validationResult.Token !=null && validationResult.Error == null)
            {
                userContext.Address = validationResult.Token.Address;
                userContext.UserId = validationResult.Token.UserId;
            }
        }
        
        await next(context);
    }
}