using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TokensMonitor.Authentication;
using TokensMonitor.Configuration;

namespace TokensMonitor.Pages;

public class SignIn : PageModel
{
    [Required]
    [BindProperty]
    public string Message { get; set; }
    
    [Required]
    [BindProperty]
    public string Signature { get; set; }
    
    public string? Error { get; set; }

    public bool HasError => !string.IsNullOrEmpty(Error);
    
    public void OnGet([FromQuery]string message, [FromQuery] string signature)
    {
        Message = message.Replace("\n", "<br/>");
        Signature = signature;
    }

    public async Task<IActionResult> OnPost([FromServices]AuthenticationService authenticationService, 
        [FromServices]TokensService tokensService)
    {
        AuthenticationResult authenticationResult = await authenticationService.Authenticate(new AuthenticationRequest(Message, Signature));

        if (!authenticationResult.Success)
        {
            Error = authenticationResult.ErrorMessage;
            return Page();
        }
        
        string? token = tokensService.Build(authenticationResult.TokenRequest);
        return RedirectToPage("Monitor", new { access_token = token });
    }
}