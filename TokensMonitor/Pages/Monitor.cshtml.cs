using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TokensMonitor.Authentication;
using TokensMonitor.Wallet;

namespace TokensMonitor.Pages;

[Authorize]
public class Monitor : PageModel
{
    [BindProperty]
    [Display(Name = "Address")]
    [Required]
    public string Address { get; set; }

    [BindProperty]
    [Display(Name = "Address List")]
    [Required]
    public List<SelectListItem> AddressList { get; set; } = new ();
    
    [BindProperty]
    [Display(Name = "Timer period")]
    [Range(1, 120, ErrorMessage = "The field must be a number.")]
    [Required]
    public int Period { get; set; }
    
    [BindProperty]
    [Display(Name = "Telegram API Key")]
    public string? TelegramApiKey { get; set; }
    
    [BindProperty]
    [Display(Name = "Telegram Channel Id")]
    public string? TelegramChannelId { get; set; }
    
    public bool IsWorking { get; set; }
    
    public void OnGet([FromServices]IUserContext userContext,
                        [FromServices]MonitorScheduler monitorScheduler)
    {
        Address = userContext.Address;
        Period = 1;
        IsWorking = monitorScheduler.IsWorkingForUser(userContext.UserId);
    }
}