using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokensMonitor.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MonitorController: Controller
{
    
}