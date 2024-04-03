using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokensMonitor.Authentication;
using TokensMonitor.Errors;
using TokensMonitor.Models;

namespace TokensMonitor.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController(AuthenticationService authenticationService, TokensService tokensService): Controller
{
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ActionResult> Authenticate(AuthenticationRequest request)
    {
        string? validationError = request.Validate();
        if (validationError != null)
        {
            return BadRequest(validationError);
        }

        AuthenticationResult authenticationResult = await authenticationService.Authenticate(request);

        if (!authenticationResult.Success)
        {
            return BadRequest(authenticationResult.ErrorMessage);
        }

        if (authenticationResult.TokenRequest == null)
        {
            return new ObjectResult(new ErrorResponse("Cannot authenticate"))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        string? token = tokensService.Build(authenticationResult.TokenRequest);
        
        if(token == null)
        {
            return new ObjectResult(new ErrorResponse("Cannot create token"))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return Ok(new ApiResponse<string>(token));
    }
}