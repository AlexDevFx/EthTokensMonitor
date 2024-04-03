using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokensMonitor.Authentication;
using TokensMonitor.Models;

namespace TokensMonitor.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class SigninController(AuthMessageService authMessageService): Controller
{
    [HttpPost("message")]
    public IActionResult NewMessage([FromBody] NewMessageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        (string message, string signature) = authMessageService.NewSignedMessage(request.Address);

        return Ok(new ApiResponse<AuthenticationRequest>(new(message, signature)));
    }
}

public class NewMessageRequest
{
    [RegularExpression("^(0x)?[0-9a-fA-F]{40}$")]
    public string Address { get; set; }
}