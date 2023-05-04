using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Controllers;

public class BaseController : ControllerBase
{
    protected readonly Guid UserId;
    
    public BaseController(IHttpContextAccessor httpContextAccessor)
    {
        UserId = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}   