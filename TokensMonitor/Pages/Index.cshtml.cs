using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TokensMonitor.Authentication;

namespace TokensMonitor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    
    [Display(Name = "ERC20 Wallet Address")]
    [Required(ErrorMessage = "Please fill address")]
    [RegularExpression("^(0x)?[0-9a-fA-F]{40}$", ErrorMessage = "Address is invalid")]
    [BindProperty]
    public string Address { get; set; }
    
    public string? Errors { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        
    }

    public IActionResult OnPost([FromServices] AuthMessageService authMessageService)
    {
        (string message, string signature) = authMessageService.NewSignedMessage(Address);
        
        return RedirectToPage("SignIn", new { message, signature});
    }
}